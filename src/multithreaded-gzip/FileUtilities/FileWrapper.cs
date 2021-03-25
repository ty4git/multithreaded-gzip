using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace multithreaded_gzip.FileUtilities
{
    public class FileWrapper : IFileSource, IFileTarget
    {
        private readonly FileInfo _fileInfo;

        public FileWrapper(string fileName)
            : this(new FileInfo(fileName))
        {
        }

        public FileWrapper(FileInfo fileInfo)
        {
            _fileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
        }

        public string FullName => _fileInfo.FullName;

        public long Length => _fileInfo.Length;

        public IEnumerable<byte[]> ReadByParts(int partSize = 1024)
        {
            return FileTools.ReadFileByParts(FullName, partSize);
        }

        public IAsyncEnumerable<byte[]> ReadByPartsAsync(int partSize = 1024)
        {
            return FileTools.ReadFileByPartsAsync(FullName, partSize);
        }

        public IEnumerable<byte[]> ReadFileByDynamicParts(DynamicPartConfiguration dynamicPart)
        {
            return FileTools.ReadFileByDynamicParts(FullName, dynamicPart);
        }

        public void WriteByParts(IEnumerable<byte[]> parts)
        {
            parts.WriteFileByParts(FullName);
        }

        public Task WriteByParts(IAsyncEnumerable<byte[]> parts)
        {
            return parts.WriteFileByPartsAsync(FullName);
        }
    }
}
