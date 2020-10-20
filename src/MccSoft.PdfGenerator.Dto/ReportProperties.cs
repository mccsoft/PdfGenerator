using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MccSoft.PdfGenerator.Dto
{
    public class ReportProperties
    {
        public ReportProperties(
            string language,
            DateTimeOffset createdAt)
        {
            Language = language;
            CreatedAt = createdAt;
        }

        /// <summary>
        /// The language of a report.
        /// </summary>
        public string Language { get; }

        /// <summary>
        /// The report creation date.
        /// </summary>
        public DateTimeOffset CreatedAt { get; }
    }
}