using System;

namespace MccSoft.PdfGenerator.Dto
{
    public class DateRange
    {
        public DateRange(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        /// <summary>
        /// Start date of a period.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of a period.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets the period length in days.
        /// </summary>
        public int PeriodLengthInDays => (EndDate - StartDate).Days + 1;
    }
}