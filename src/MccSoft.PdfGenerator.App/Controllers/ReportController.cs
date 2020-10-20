using System;
using System.Threading.Tasks;
using MccSoft.PdfGenerator.App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MccSoft.PdfGenerator.App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : Controller
    {
        private readonly PdfGeneratorService _pdfGeneratorService;

        public ReportController(PdfGeneratorService pdfGeneratorService)
        {
            _pdfGeneratorService = pdfGeneratorService;
        }

        /// <summary>
        ///     Generates pdf report and returns report's content as byte array in base64
        /// </summary>
        /// <param name="language">
        ///     Language of generated report in ISO 639-1 format (e.g. en, de, ru etc).
        /// </param>
        /// <param name="startDate">Start date of the report generation period.</param>
        /// <param name="endDate">End date of the report generation period.</param>
        /// <param name="userReportCreationTime">The user's date and time when report was created.</param>
        [HttpPost("generate")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateAndGetReport(
            [FromQuery] string language,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] DateTimeOffset userReportCreationTime)
        {
            var pdf = 
                await _pdfGeneratorService.Execute(startDate, endDate, language, userReportCreationTime);
            return File(pdf, "application/pdf");
        }
    }
}