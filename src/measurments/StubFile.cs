using multithreaded_gzip;
using multithreaded_gzip.FileUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace measurments
{
    public class StubFile : IFileSource, IFileTarget, IFileInfo
    {
        public long Length { get; } = 1024 * 1024 * 1024;
        public string FullName { get; } = nameof(StubFile);
        public byte[] Template { get; }

        public StubFile()
        {
            Template = new byte[1024 * 1024 * 1024];
            new Random().NextBytes(Template);
        }

        public IEnumerable<byte[]> ReadByParts(int partSize)
        {
            var template = Template.Take(partSize).ToArray();

            for (var i = 0; i < Length / partSize; i++)
            {
                yield return template.ToArray();
            }
        }

        public async IAsyncEnumerable<byte[]> ReadByPartsAsync(int partSize)
        {
            await Task.Yield();
            var template = Template.Take(partSize).ToArray();

            for (var i = 0; i < Length / partSize; i++)
            {
                yield return template.ToArray();
            }
        }

        public IEnumerable<byte[]> ReadFileByDynamicParts(DynamicPartConfiguration dynamicPart)
        {
            throw new System.NotImplementedException();
        }

        public void WriteByParts(IEnumerable<byte[]> parts)
        {
            foreach (var _ in parts)
            {
            }
        }

        public async Task WriteByParts(IAsyncEnumerable<byte[]> parts)
        {
            await foreach (var _ in parts)
            {
            }
        }
    }

}
