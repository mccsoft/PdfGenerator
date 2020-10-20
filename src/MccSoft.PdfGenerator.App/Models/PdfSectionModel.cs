namespace MccSoft.PdfGenerator.App.Models
{
    /// <summary>
    /// Section is single PDF file, that represents real section within
    /// PDF report. E.g. ExpandedStatistics, or Overview - it is sections.
    /// </summary>
    public class PdfSectionModel
    {
        public PdfSectionModel(
            string bundleName,
            byte[] contentBytes,
            bool isLandscape)
        {
            BundleName = bundleName;
            ContentBytes = contentBytes;
            IsLandscape = isLandscape;
        }

        /// <summary>
        /// Name of the bundle, from which this pdf was generated.
        /// </summary>
        public string BundleName { get; }

        /// <summary>
        /// Bytes of the generated PDF.
        /// </summary>
        public byte[] ContentBytes { get; }

        /// <summary>
        /// Tells whether this document is landscape.
        /// </summary>
        public bool IsLandscape { get; }
    }
}