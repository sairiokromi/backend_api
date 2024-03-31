using System.IO.Compression;

namespace Kromi.Infrastructure.Utils
{
    public static class FileCompress
    {
        /// <summary>
        /// Comprime una lista de archivos y genera un arreglo de bytes con el archivo comprimido.
        /// </summary>
        /// <param name="archivos">Lista de archivos en byte[]</param>
        /// <param name="nombreArchivo">Lista de nombres que tendran los archivos</param>
        /// <returns>Arreglo de bytes con el archivo comprimido</returns>
        public static byte[]? Comprimir(IList<byte[]> archivos, IList<string> nombreArchivo)
        {
            if (archivos.Count != nombreArchivo.Count) return null;
            using (var compressedFileStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Update))
                {
                    for (var i = 0; i < archivos.Count; i++)
                    {
                        var zipEntry = zipArchive.CreateEntry(nombreArchivo[i]);

                        using (var originalFileStream = new MemoryStream(archivos[i]))
                        {
                            using (var zipEntryStream = zipEntry.Open())
                            {
                                originalFileStream.CopyTo(zipEntryStream);
                            }
                        }
                    }
                }
                return compressedFileStream.ToArray();
            }
        }
    }
}
