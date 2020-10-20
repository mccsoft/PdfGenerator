using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using MccSoft.PdfGenerator.Dto;
using MccSoft.PdfGenerator.App.Models;
using MccSoft.PdfGenerator.App.Options;
using MccSoft.PdfGenerator.App.Views.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using Document = iTextSharp.text.Document;

namespace MccSoft.PdfGenerator.App.Services
{
    public class PdfGeneratorService
    {
        /// <summary>
        /// Font size of page number digits.
        /// </summary>
        private const int _pageNumberFontSize = 11;

        /// <summary>
        /// X offset for page numbers from left bottom in portrait orientation.
        /// </summary>
        private const int _pageNumberPortraitOffsetX = 541;

        /// <summary>
        /// X offset for page numbers from left bottom in landscape orientation.
        /// </summary>
        private const int _pageNumberLandscapeOffsetX = 791;

        /// <summary>
        /// X offset for page numbers from left bottom in portrait orientation.
        /// </summary>
        private const int _pageNumberOffsetY = 40;

        private readonly string _chromePath;

        private readonly MarginOptions _landscapeMarginOptions = new MarginOptions
        {
            Top = "11mm",
            Bottom = "26mm",
            Left = "11mm",
            Right = "11mm"
        };

        private readonly MarginOptions _portraitMarginOptions = new MarginOptions
        {
            Top = "26mm",
            Bottom = "26mm",
            Left = "11mm",
            Right = "11mm"
        };

        private readonly ILogger<PdfGeneratorService> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ViewRenderService _renderService;

        public PdfGeneratorService(
            ViewRenderService renderService,
            ILoggerFactory loggerFactory,
            IOptions<ExecutableChromeOptions> chromeOptions)
        {
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<PdfGeneratorService>();
            _renderService = renderService;
            _chromePath = chromeOptions.Value?.Path;
        }

        /// <summary>
        ///     Runs pdf generating workflow.
        /// </summary>
        /// <param name="startDate">Start term date of generated report.</param>
        /// <param name="endDate">Finish term date of generated report</param>
        /// <param name="languageAbbreviation">
        /// The language abbreviation of the report.
        /// If <c>null</c> is specified, the organisation language abbreviation is used.
        /// </param>
        /// <param name="userReportCreationTime">Time when user trigger report generation</param>
        public async Task<byte[]> Execute(
            DateTime startDate,
            DateTime endDate,
            string languageAbbreviation,
            DateTimeOffset userReportCreationTime)
        {
            string operationInfo = $"{nameof(Execute)}: "
                                   + $"user's creation datetime - {userReportCreationTime:o}, "
                                   + $"report start date - {startDate:o}, report end date - {endDate:o}, "
                                   + $"requested languageAbbreviation: '{languageAbbreviation}'";

            _logger.LogInformation($"Starting {operationInfo}.");
            var reportDateRange = new DateRange(startDate, endDate);

            // Get some data for report from the other services

            // Set culture info for header and footer generation
            var cultureInfo = new CultureInfo(languageAbbreviation);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            var reportDto = new ReportDto(languageAbbreviation, userReportCreationTime, reportDateRange);

            _logger.LogInformation("Beginning PDF Generation.");
            Stopwatch pdfGenerationStopwatch = Stopwatch.StartNew();

            byte[] pdfContent = await GeneratePdf(reportDto);

            _logger.LogInformation(
                $"Successfully generated. PDF content length - {pdfContent.Length}");
            pdfGenerationStopwatch.Stop();
            _logger.LogInformation(
                "PDF Generation took "
                + $"{pdfGenerationStopwatch.Elapsed.Seconds:00}."
                + $"{pdfGenerationStopwatch.ElapsedMilliseconds:00} s");

            _logger.LogInformation($"Finished ${operationInfo}");

            return pdfContent;
        }

        /// <summary>
        /// Opens chromium browser and generates pdf from html.
        /// </summary>
        /// <param name="reportDto">Dto with all data for report generation.</param>
        /// <returns>PDF content in byte array.</returns>
        private async Task<byte[]> GeneratePdf(ReportDto reportDto)
        {
            var options = new LaunchOptions
            {
                Headless = true,
                ExecutablePath = _chromePath,
                Args = new[] {"--no-sandbox", $"--lang={reportDto.Properties.Language}"}
            };

            string htmlContent = await _renderService.RenderToStringAsync(
                "Content",
                reportDto);
            string htmlHeader = await _renderService.RenderToStringAsync(
                "Header",
                new HeaderModel("HEADER"));
            string htmlFooter = await _renderService.RenderToStringAsync(
                "Footer",
                new FooterModel(reportDto.Properties.CreatedAt));

            _logger.LogInformation($"Starting browser at the path {_chromePath}.");
            using (Browser browser = await Puppeteer.LaunchAsync(options, _loggerFactory))
            {
                var printedSections = new List<PdfSectionModel>();

                PdfSectionModel sectionPdf = await MakeSectionPdf(
                    htmlContent: htmlContent,
                    htmlHeader: htmlHeader,
                    htmlFooter: htmlFooter,
                    bundleFileName: "report",
                    isLandscape: false,
                    browser: browser);

                printedSections.Add(sectionPdf);

                byte[] finalPdf = MergePdfsAndAddPageNumbers(printedSections);

                return finalPdf;
            }
        }

        /// <summary>
        /// Generates so-called section pdf out of given bundle,
        /// which contains some number of pages with the same orientation.
        /// </summary>
        /// <param name="htmlContent">Html content of the page.</param>
        /// <param name="htmlHeader">Html header of the page.</param>
        /// <param name="htmlFooter">Html footer of the page.</param>
        /// <param name="bundleFileName">
        /// File name of the bundle,
        /// to include in page
        /// </param>
        /// <param name="isLandscape">Pdf orientation.</param>
        /// <param name="browser">Puppeteer browser.</param>
        private async Task<PdfSectionModel> MakeSectionPdf(
            string htmlContent,
            string htmlHeader,
            string htmlFooter,
            string bundleFileName,
            bool isLandscape,
            Browser browser)
        {
            _logger.LogInformation("Opening page for generating report.");
            _logger.LogInformation($"Generation of {bundleFileName} section.");
            using (Page page = await browser.NewPageAsync())
            {
                // Report doesn't renders for a long time, 5 seconds is enough.
                page.DefaultTimeout = 5000;

                page.PageError += (sender, args) =>
                {
                    // Just log the error.
                    // In case of an error the function "PdfDataAsync" will fail with timeout.
                    _logger.LogError($"Report generating is failed {args.Message}");
                };

                page.Console += (sender, args) =>
                {
                    ConsoleMessage msg = args.Message;
                    ConsoleMessageLocation location = msg.Location;
                    string loc = location == null
                        ? ""
                        : $" Location: {location.URL} line {location.LineNumber} "
                          + $"col {location.ColumnNumber}";

                    // Just log some console warning and information.
                    _logger.LogWarning(
                        $"Warning is occured while report generating. {msg.Text}{loc}");
                };

                _logger.LogInformation("Setting page content.");
                await page.SetContentAsync(htmlContent);

                _logger.LogInformation("Setting page script.");
                await page.AddScriptTagAsync(
                    new AddTagOptions {Path = $"./Template/{bundleFileName}-bundle.js"});

                _logger.LogInformation("Waiting for report.");
                await page.WaitForSelectorAsync("#report");

                _logger.LogInformation("Generating PDF.");

                byte[] pdfContentBytes = await page.PdfDataAsync(
                    new PdfOptions
                    {
                        Format = PaperFormat.A4,
                        Landscape = isLandscape,
                        DisplayHeaderFooter = true,
                        FooterTemplate = htmlFooter,
                        HeaderTemplate = htmlHeader,
                        MarginOptions =
                            isLandscape ? _landscapeMarginOptions : _portraitMarginOptions
                    });

                return new PdfSectionModel(bundleFileName, pdfContentBytes, isLandscape);
            }
        }

        /// <summary>
        /// Merges all given documents into one.
        /// Also draws page numbers for each section.
        /// Uses https://github.com/VahidN/iTextSharp.LGPLv2.Core which is single
        ///  free/open source instrument out there for .NET Core.
        ///  It is port of iTextSharp 4.1.6 (last LGP version), so if you need documentation
        ///  reference to this version.
        /// If we would need more stuff than just merging, it is suggested to move to some
        /// commercial licensed library.
        /// </summary>
        /// <param name="sectionsToMerge">Given sections</param>
        /// <returns>Array of bytes representing all pdfs merged into one.</returns>
        private byte[] MergePdfsAndAddPageNumbers(List<PdfSectionModel> sectionsToMerge)
        {
            int totalPageCount =
                CountTotalPageNumberOfSections(sectionsToMerge);
            int currentFinalPageNumber = 0;

            var finalDocument = new Document();
            using (var stream = new MemoryStream())
            {
                var pdfCopy = new PdfCopy(finalDocument, stream);
                finalDocument.Open();

                foreach (PdfSectionModel section in sectionsToMerge)
                {
                    var pdfReader = new PdfReader(section.ContentBytes);
                    int copiedDocumentTotalPages = pdfReader.NumberOfPages;

                    for (int currentCopiedDocumentPageNumber = 1;
                        currentCopiedDocumentPageNumber <= copiedDocumentTotalPages;
                        currentCopiedDocumentPageNumber++)
                    {
                        currentFinalPageNumber++;

                        PdfImportedPage importedPage = pdfCopy.GetImportedPage(
                            pdfReader,
                            currentCopiedDocumentPageNumber);

                        PdfCopy.PageStamp pageStamp = pdfCopy.CreatePageStamp(importedPage);

                        AddPageNumber(
                            pageStamp,
                            currentFinalPageNumber,
                            totalPageCount,
                            section.IsLandscape);

                        pdfCopy.AddPage(importedPage);
                    }

                    pdfCopy.FreeReader(pdfReader);
                    pdfReader.Close();
                }

                finalDocument.Close();
                return stream.GetBuffer();
            }
        }

        /// <summary>
        /// Takes stamp of some PDF document page, and writes page numbers
        /// in format " current / total " on this page.
        /// </summary>
        /// <param name="pageStamp">
        ///     Stamp that stores page within itself.
        /// </param>
        /// <param name="currentPageNumber">
        ///     Current page number that will be written on a page.
        /// </param>
        /// <param name="totalPageCount">
        ///     Total page number that will be written on a page.
        /// </param>
        /// <param name="isLandscape">
        ///     Depending on page orientation,
        ///     different X-Y offsets of the text will be used.
        /// </param>
        private void AddPageNumber(
            PdfCopy.PageStamp pageStamp,
            int currentPageNumber,
            int totalPageCount,
            bool isLandscape)
        {
            BaseFont currentNumberFont = CreateFont(isBold: true);
            BaseFont totalNumberFont = CreateFont(isBold: false);
            PdfContentByte contentOverPage = pageStamp.GetOverContent();

            contentOverPage.BeginText();
            contentOverPage.SetFontAndSize(currentNumberFont, _pageNumberFontSize);
            contentOverPage.MoveText(
                isLandscape ? _pageNumberLandscapeOffsetX : _pageNumberPortraitOffsetX,
                _pageNumberOffsetY);
            contentOverPage.ShowText($"{currentPageNumber} ");
            contentOverPage.SetFontAndSize(totalNumberFont, _pageNumberFontSize);
            contentOverPage.ShowText($"/ {totalPageCount}");
            contentOverPage.EndText();
            //Writes content to a page stamp.
            pageStamp.AlterContents();
        }

        /// <summary>
        /// Sums up all page numbers of pdf section models.
        /// </summary>
        private int CountTotalPageNumberOfSections(List<PdfSectionModel> pdfsSectionModelsToCount)
        {
            int totalPageNumber = 0;

            foreach (PdfSectionModel pdfSection in pdfsSectionModelsToCount)
            {
                PdfReader pdfReader = null;

                try
                {
                    pdfReader = new PdfReader(pdfSection.ContentBytes);
                    totalPageNumber += pdfReader.NumberOfPages;
                }
                catch (IOException ex)
                {
                    _logger.LogError(
                        "Something went wrong during reading pdf "
                        + $"\"{pdfSection.BundleName}\" exception: {ex}");
                }
                finally
                {
                    pdfReader?.Close();
                }
            }

            return totalPageNumber;
        }

        /// <summary>
        /// Creates iText BaseFont font.
        /// </summary>
        private BaseFont CreateFont(bool isBold)
        {
            return BaseFont.CreateFont(
                isBold ? BaseFont.HELVETICA_BOLD : BaseFont.HELVETICA,
                BaseFont.CP1252,
                BaseFont.NOT_EMBEDDED);
        }
    }
}