using System;

namespace MccSoft.PdfGenerator.Dto
{
    public class ReportDto
    {
        public ReportDto(
            string language,
            DateTimeOffset createdAt,
            DateRange reportDateRange)
        {
            ReportDateRange = reportDateRange;
            Properties = new ReportProperties(language, createdAt);
        }

        /// <summary>
        /// Common properties for report.
        /// </summary>
        public ReportProperties Properties { get; }

        /// <summary>
        /// Date period of report statistics.
        /// </summary>
        public DateRange ReportDateRange { get; }
    }
}