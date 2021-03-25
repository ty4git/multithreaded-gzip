using multithreaded_gzip.FileUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace multithreaded_gzip.Archiver
{
    public class ParallelCompressor : ParallelArchiver
    {
        public ParallelCompressor(IFileSource source,
            IFileTarget target,
            int blockingQueueSize = 1000,
            int partSize = 1024)
            : base(source, target,
                  blockingQueueSize,
                  partSize)
        {
        }

        protected override IEnumerable<byte[]> PartsStream()
        {
            return _source.ReadByParts(_partSize);
        }

        protected override byte[] ProcessPart(byte[] part)
        {
            var compressed = part.CompressPart();
            var lengthInBytes = BitConverter.GetBytes(compressed.Length);
            return lengthInBytes.Concat(compressed).ToArray();
        }
    }
}
