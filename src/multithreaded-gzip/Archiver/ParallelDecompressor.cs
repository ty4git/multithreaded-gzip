using multithreaded_gzip.FileUtilities;
using System;
using System.Collections.Generic;

namespace multithreaded_gzip.Archiver
{
    public class ParallelDecompressor : ParallelArchiver
    {
        public ParallelDecompressor(IFileSource source,
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
            var dynamicPartConfiguration = new DynamicPartConfiguration
            {
                PartSize = BitConverter.GetBytes(default(int)).Length
            };

            var enumerable = _source.ReadFileByDynamicParts(dynamicPartConfiguration);

            using var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var lengthBytes = enumerator.Current;
                var lengthOfPart = BitConverter.ToInt32(lengthBytes);
                dynamicPartConfiguration.PartSize = lengthOfPart;

                enumerator.MoveNext();
                var compressed = enumerator.Current;
                dynamicPartConfiguration.PartSize = lengthBytes.Length;

                yield return compressed;
            }
        }

        protected override byte[] ProcessPart(byte[] part)
        {
            return part.DecompressPart();
        }
    }
}
