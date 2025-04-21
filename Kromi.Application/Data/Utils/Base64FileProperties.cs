namespace Kromi.Application.Data.Utils
{
    public class Base64FileProperties
    {
        private static readonly IDictionary<string, IAttachmentType> mimeMap =
        new Dictionary<string, IAttachmentType>(StringComparer.OrdinalIgnoreCase)
        {
            { "IVBOR", AttachmentType.Png },
            { "/9J/4", AttachmentType.Jpg },
            { "R0lGO", AttachmentType.Gif },
            { "TU0AK", AttachmentType.Tiff },
            { "JVBER", AttachmentType.Pdf },
            { "AAAAF", AttachmentType.Video }
        };

        public IAttachmentType GetBase64FileProperties(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                mimeMap.TryGetValue(value.Substring(0, 5).ToUpper(), out IAttachmentType? result);
                return result ?? AttachmentType.Unknown;
            }
            return AttachmentType.Unknown;
        }
    }
}
