using System.Collections.Generic;
using System.Threading.Tasks;

namespace multithreaded_gzip.FileUtilities
{
    public interface IFileTarget : IFileInfo
    {
        void WriteByParts(IEnumerable<byte[]> parts);
        Task WriteByParts(IAsyncEnumerable<byte[]> Parts);
    }
}
