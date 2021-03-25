using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace multithreaded_gzip.FileUtilities
{
    public static class FileTools
    {
        public static IEnumerable<byte[]> ReadFileByParts(string fileName, int partSize = 1024)
        {
            using var sourceFileStream = File.OpenRead(fileName);
            var buffer = new byte[partSize];
            var readCount = 0;
            while ((readCount = sourceFileStream.Read(buffer)) > 0)
            {
                if (readCount < buffer.Length)
                {
                    buffer = buffer[0..readCount];
                }

                yield return buffer.ToArray();
            }
        }

        public async static IAsyncEnumerable<byte[]> ReadFileByPartsAsync(string fileName, int partSize = 1024)
        {
            using var sourceFileStream = File.OpenRead(fileName);
            var buffer = new byte[partSize];
            var readCount = 0;
            while ((readCount = await sourceFileStream.ReadAsync(buffer)) > 0)
            {
                if (readCount < buffer.Length)
                {
                    buffer = buffer[0..readCount];
                }

                yield return buffer.ToArray();
            }
        }

        public static IEnumerable<byte[]> ReadFileByDynamicParts(string fileName, DynamicPartConfiguration dynamicPart)
        {
            using var compressedFileStream = File.OpenRead(fileName);
            var part = new byte[dynamicPart.PartSize];
            var readCount = 0;
            while ((readCount = compressedFileStream.Read(part)) > 0)
            {
                if (readCount != part.Length)
                {
                    throw new InvalidDataException();
                }

                yield return part;

                part = new byte[dynamicPart.PartSize];
            }
        }

        public static void WriteFileByParts(this IEnumerable<byte[]> parts, string outputFileName)
        {
            using var compressedFileStream = File.Create(outputFileName);
            foreach (var part in parts)
            {
                compressedFileStream.Write(part);
            }
        }

        public async static Task WriteFileByPartsAsync(this IAsyncEnumerable<byte[]> parts, string outputFileName)
        {
            using var compressedFileStream = File.Create(outputFileName);
            await foreach (var part in parts)
            {
                await compressedFileStream.WriteAsync(part);
            }
        }

        public static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
