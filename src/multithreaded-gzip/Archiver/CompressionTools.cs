using System.IO;
using System.IO.Compression;

namespace multithreaded_gzip.Archiver
{
    public static class CompressionTools
    {
        public static byte[] CompressPart(this byte[] source)
        {
            using var outputMemoryStream = new MemoryStream();
            using (var compressionStream = new GZipStream(outputMemoryStream,
                CompressionMode.Compress))
            {
                compressionStream.Write(source);
            }

            return outputMemoryStream.ToArray();
        }

        public static byte[] DecompressPart(this byte[] compressed)
        {
            using var compressedMemoryStream = new MemoryStream(compressed);
            using var decompressionStream = new GZipStream(compressedMemoryStream,
                CompressionMode.Decompress);
            using var decompressedMemoryStream = new MemoryStream();

            decompressionStream.CopyTo(decompressedMemoryStream);
            return decompressedMemoryStream.ToArray();
        }
    }
}
