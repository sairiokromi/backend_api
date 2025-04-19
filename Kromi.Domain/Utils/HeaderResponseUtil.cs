namespace Kromi.Domain.Utils
{
    public static class HeaderResponseUtil
    {
        public const string Excel = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public const string Zip = "application/zip";
        public const string Pdf = "application/pdf";
        public const string Gif = "image/gif";
        public const string Png = "image/png";
        public const string Jpeg = "image/jpg";
        public const string Jpg = "image/jpg";

        public static string GetHeader(string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                var parts = filename.Split(".");
                var ext = parts[parts.Length - 1];

                return ext.ToLower() switch
                {
                    "pdf" => Pdf,
                    "zip" => Zip,
                    "xls" => Excel,
                    "gif" => Gif,
                    "png" => Png,
                    "jpeg" => Jpeg,
                    "jpg" => Jpg,
                    _ => "text/plain",
                };
            }
            return string.Empty;
        }
    }
}
