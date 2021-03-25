using System.Collections.Generic;

namespace multithreaded_gzip.FileUtilities
{
    public interface IFileSource : IFileInfo
    {
        IEnumerable<byte[]> ReadByParts(int partSize);
        IAsyncEnumerable<byte[]> ReadByPartsAsync(int partSize);
        IEnumerable<byte[]> ReadFileByDynamicParts(DynamicPartConfiguration dynamicPart);
    }
}
