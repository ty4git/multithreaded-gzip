using multithreaded_gzip.FileUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace multithreaded_gzip.Archiver
{
    public abstract class TaskBasedParallelArchiver
    {
        private readonly IFileSource _fileSource;
        private readonly IFileTarget _fileTarget;
        private readonly int _partSize;

        public TaskBasedParallelArchiver(IFileSource fileSource, IFileTarget fileTarget, int partSize = 1024)
        {
            _fileSource = fileSource ?? throw new ArgumentNullException(nameof(fileSource));
            _fileTarget = fileTarget ?? throw new ArgumentNullException(nameof(fileTarget));
            _partSize = partSize;
        }

        public async Task Process(CancellationToken cancellationToken)
        {
            var processorsCount = Environment.ProcessorCount;
            var channel = Channel.CreateBounded<Task<byte[]>>(processorsCount);
            var fileParts = _fileSource.ReadByPartsAsync(_partSize);

            var processing = Task.Run(async () =>
            {
                var channelWriter = channel.Writer;
                await foreach (var part in fileParts)
                {
                    var compressedPartTask = Task.Run(() =>
                    {
                        var compressedPart = ProcessPart(part);
                        return compressedPart;
                    }, cancellationToken);
                    await channelWriter.WriteAsync(compressedPartTask, cancellationToken);
                }
                channelWriter.Complete();
            }, cancellationToken);

            await WriteByParts(channel.Reader, cancellationToken);
        }

        private async Task WriteByParts(ChannelReader<Task<byte[]>> channelReader, CancellationToken cancellationToken)
        {
            await Task.Run(async () =>
            {
                await _fileTarget.WriteByParts(Parts());
            }, cancellationToken);

            async IAsyncEnumerable<byte[]> Parts()
            {
                while (await channelReader.WaitToReadAsync(cancellationToken))
                {
                    var processedPartTask = await channelReader.ReadAsync(cancellationToken);
                    var processedPart = await processedPartTask;
                    yield return processedPart;
                }
            }
        }

        protected abstract byte[] ProcessPart(byte[] part);
    }

    public class TaskBasedParallelCompressor : TaskBasedParallelArchiver
    {
        public TaskBasedParallelCompressor(IFileSource fileSource, IFileTarget fileTarget, int partSize = 1024)
            : base(fileSource, fileTarget, partSize)
        {
        }

        protected override byte[] ProcessPart(byte[] part)
        {
            var compressed = part.CompressPart();
            var lengthInBytes = BitConverter.GetBytes(compressed.Length);
            return lengthInBytes.Concat(compressed).ToArray();
        }
    }

    public class TaskBasedParallelDecompressor : TaskBasedParallelArchiver
    {
        public TaskBasedParallelDecompressor(IFileSource fileSource, IFileTarget fileTarget, int partSize = 1024)
            : base(fileSource, fileTarget, partSize)
        {
        }

        protected override byte[] ProcessPart(byte[] part)
        {
            return part.DecompressPart();
        }
    }
}
