using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace multithreaded_gzip.ConcurrencyUtilities
{
    class StaticThreadPool
    {
        private readonly BlockingQueue<Action> _innerJobsQueue;
        private readonly List<Worker> _workers;
        private readonly object _locker = new object();

        public StaticThreadPool()
        {
            _innerJobsQueue = new BlockingQueue<Action>(1000);
            _workers = Enumerable.Range(0, 1)
                .Select(_ => new Worker(_innerJobsQueue))
                .ToList();
        }

        public int MaxSize { get; } = Environment.ProcessorCount;

        public void QueueJob(Action job)
        {
            lock (_locker)
            {
                if (!_innerJobsQueue.TryEnqueue(job))
                {
                    throw new Exception();
                }

                if (_workers.Count < MaxSize)
                {
                    var additionalWorker = new Worker(_innerJobsQueue);
                    _workers.Add(additionalWorker);
                }
            }
        }

        class Worker
        {
            private readonly Thread _thread;

            public Worker(BlockingQueue<Action> jobsQueueOfthreadPool)
            {
                _thread = new Thread(() =>
                {
                    while (jobsQueueOfthreadPool.Dequeue(out var action))
                    {
                        action();
                    }
                })
                {
                    IsBackground = true
                };
                _thread.Start();
            }
        }
    }
}
