using multithreaded_gzip.Archiver;
using multithreaded_gzip.FileUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace measurments
{
    public static class CompressionStream
    {
        public static void MultipleThreadsCompressByParts(string nameOfFileToCompress, string nameOfCompressedFile,
            int partSize = 1024)
        {
            FileTools.ReadFileByParts(nameOfFileToCompress, partSize)
                .AsParallel()
                .AsOrdered()
                .Select(part => {
                    var compressed = part.CompressPart();
                    var lengthInBytes = BitConverter.GetBytes(compressed.Length);
                    return lengthInBytes.Concat(compressed).ToArray();
                })
                .WriteFileByParts(nameOfCompressedFile);
        }

        public static void CompressByParts(string nameOfFileToCompress, string nameOfCompressedFile)
        {
            FileTools.ReadFileByParts(nameOfFileToCompress)
                .Select(part => {
                    var compressed = part.CompressPart();
                    var lengthInBytes = BitConverter.GetBytes(compressed.Length);
                    return lengthInBytes.Concat(compressed).ToArray();
                })
                .WriteFileByParts(nameOfCompressedFile);
        }

        public static void DecompressByParts(string nameOfCompressedFile, string nameOfDecompressedFile)
        {
            DecompressedParts(nameOfCompressedFile)
                .WriteFileByParts(nameOfDecompressedFile);
        }

        public static void Compress(string nameOfFileToCompress, string nameOfCompressedFile, int partSize)
        {
            using var originalFileStream = File.OpenRead(nameOfFileToCompress);
            using var compressedFileStream = File.Create(nameOfCompressedFile);
            using var compressionStream = new GZipStream(compressedFileStream,
                CompressionMode.Compress);
            originalFileStream.CopyTo(compressionStream, partSize);
        }

        public static void Decompress(string nameOfCompressedFile, string nameOfDecompressedFile)
        {
            using var originalFileStream = File.OpenRead(nameOfCompressedFile);
            using var decompressedFileStream = File.Create(nameOfDecompressedFile);

            using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
            {
                decompressionStream.CopyTo(decompressedFileStream);
            }
        }

        private static IEnumerable<byte[]> DecompressedParts(string nameOfCompressedFile)
        {
            var dynamicPartConfiguration = new DynamicPartConfiguration
            {
                PartSize = BitConverter.GetBytes(default(int)).Length
            };

            var enumerable = FileTools.ReadFileByDynamicParts(nameOfCompressedFile, dynamicPartConfiguration);

            using var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var lengthBytes = enumerator.Current;
                var lengthOfPart = BitConverter.ToInt32(lengthBytes, 0);
                dynamicPartConfiguration.PartSize = lengthOfPart;

                enumerator.MoveNext();
                var compressed = enumerator.Current;
                var decompressed = compressed.DecompressPart();
                dynamicPartConfiguration.PartSize = lengthBytes.Length;

                yield return decompressed;
            }
        }
    }
}
