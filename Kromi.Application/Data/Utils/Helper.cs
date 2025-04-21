namespace Kromi.Application.Data.Utils
{
    public static class Helper
    {
        public static void EnsurePath(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
