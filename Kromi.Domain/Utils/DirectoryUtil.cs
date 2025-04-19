namespace Kromi.Domain.Utils
{
    public static class DirectoryUtil
    {
        public static void EnsurePath(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
