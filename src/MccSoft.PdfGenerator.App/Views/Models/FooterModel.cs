using System;

namespace MccSoft.PdfGenerator.App.Views.Models
{
    public class FooterModel
    {
        public FooterModel(DateTimeOffset createdAt)
        {
            CreatedAt = createdAt;
        }

        public DateTimeOffset CreatedAt { get; }
    }
}
