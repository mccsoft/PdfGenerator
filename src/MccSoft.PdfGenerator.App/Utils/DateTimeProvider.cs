using System;

namespace MccSoft.PdfGenerator.App.Utils
{
    /// <summary>
    /// Provides a consistent value of "now" throughout a service operation.
    /// </summary>
    public class DateTimeProvider
    {
        public DateTimeOffset UtcNow { get; } = DateTimeOffset.UtcNow;
    }
}