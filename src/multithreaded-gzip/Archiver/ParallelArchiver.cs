using multithreaded_gzip.ConcurrencyUtilities;
using multithreaded_gzip.FileUtilities;
using System;
using System.Collections.Generic;
using System.Threading;

namespace multithreaded_gzip.Archiver
{
    public abstract class ParallelArchiver : IParallelArchiver
    {
        private readonly IFileTarget _target;
        private readonly int _blockingQueueSize;

        protected readonly int _partSize;
        protected readonly IFileSource _source;

        public ParallelArchiver(IFileSource source,
            IFileTarget target,
            int blockingQueueSize = 1000,
            int partSize = 1024)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _target = target ?? throw new ArgumentNullException(nameof(target));
            _blockingQueueSize = blockingQueueSize;
            _partSize = partSize;
        }

        public void Process(Action onFinished, Cancellation cancellation = null)
        {
            var sourceParts = new BlockingQueue<byte[]>(_blockingQueueSize);
            ReadByParts(sourceParts, cancellation);

            var processedParts = new BlockingQueue<BlockingWorkItem<byte[]>>(_blockingQueueSize);
            ProcessParts(sourceParts, processedParts, cancellation);

            WriteByParts(processedParts, onFinished, cancellation);
        }

        protected abstract IEnumerable<byte[]> PartsStream();
        protected abstract byte[] ProcessPart(byte[] part);

        private void ReadByParts(BlockingQueue<byte[]> outputBlockingQueue,
            Cancellation cancellation = null)
        {
            new Thread(() =>
            {
                var parts = PartsStream();
                foreach (var part in parts)
                {
                    if (cancellation?.IsCancelled() ?? false)
                    {
                        break;
                    }

                    outputBlockingQueue.Enqueue(part);
                }
                outputBlockingQueue.Complete();
            }).Start();
        }

        private void ProcessParts(BlockingQueue<byte[]> inputBlockingQueue,
            BlockingQueue<BlockingWorkItem<byte[]>> outputBlockingQueue,
            Cancellation cancellation = null)
        {
            var threadPool = new StaticThreadPool();
            new Thread(() =>
            {
                while (inputBlockingQueue.Dequeue(out var item))
                {
                    if (cancellation?.IsCancelled() ?? false)
                    {
                        break;
                    }

                    var blockingWorkItem = new BlockingWorkItem<byte[]>();
                    outputBlockingQueue.Enqueue(blockingWorkItem);

                    var part = item;
                    threadPool.QueueJob(() =>
                    {
                        var processed = ProcessPart(part);
                        blockingWorkItem.SetResult(processed);
                    });
                }
                outputBlockingQueue.Complete();
            }).Start();
        }

        private void WriteByParts(BlockingQueue<BlockingWorkItem<byte[]>> inputBlockingQueue,
            Action onFinished,
            Cancellation cancellation = null)
        {
            new Thread(() =>
            {
                _target.WriteByParts(CompressedParts());

                if (!cancellation.IsCancelled())
                {
                    onFinished();
                }

                IEnumerable<byte[]> CompressedParts()
                {
                    while (inputBlockingQueue.Dequeue(out var part))
                    {
                        if (cancellation?.IsCancelled() ?? false)
                        {
                            yield break;
                        }

                        yield return part.GetResult();
                    }
                }
            }).Start();
        }
    }
}
