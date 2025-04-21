namespace Kromi.Application.Data.Utils
{
    public interface IAttachmentType
    {
        string MimeType { get; }
        string FriendlyName { get; }
        string Extension { get; }
    }

    public class AttachmentType : IAttachmentType
    {
        public AttachmentType(string mimeType, string friendlyName, string extension)
        {
            MimeType = mimeType;
            FriendlyName = friendlyName;
            Extension = extension;
        }

        public static IAttachmentType UnknownMime { get; } = new AttachmentType("application/octet-stream", "Unknown", string.Empty);

        public static IAttachmentType Png { get; } = new AttachmentType("image/png", "Photo", ".png");
        public static IAttachmentType Gif { get; } = new AttachmentType("image/gif", "Photo", ".gif");
        public static IAttachmentType Jpg { get; } = new AttachmentType("image/jpg", "Photo", ".jpg");
        public static IAttachmentType Tiff { get; } = new AttachmentType("image/tiff", "Photo", ".tiff");

        public static IAttachmentType Video { get; } = new AttachmentType("video/mp4", "Video", ".mp4");

        public static IAttachmentType Pdf { get; } = new AttachmentType("application/pdf", "Document", ".pdf");

        public static IAttachmentType Unknown { get; } = new AttachmentType(string.Empty, "Unknown", string.Empty);

        public string MimeType { get; }

        public string FriendlyName { get; }

        public string Extension { get; }
    }
}
