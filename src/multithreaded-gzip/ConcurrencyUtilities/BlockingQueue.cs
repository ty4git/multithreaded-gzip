using System;
using System.Collections.Generic;
using System.Threading;

namespace multithreaded_gzip.ConcurrencyUtilities
{
    //Warning: there should not be contention of threads in Add or TryAdd methods.
    //  "lock" doesn't guarantee "FIFO" of locked threads;
    class BlockingQueue<T>
    {
        private readonly object _locker = new object();
        private Queue<T> _queue = new Queue<T>();
        private bool _isCompleted = false;

        public BlockingQueue(int size = 10)
        {
            MaxSize = size;
        }

        public int MaxSize { get; }

        public void Enqueue(T item)
        {
            lock (_locker)
            {
                if (_isCompleted)
                {
                    throw new Exception("Queue is completed.");
                }

                while (IsFull())
                {
                    Monitor.Wait(_locker);
                }

                NotThreadSafeInnerAdd(item);
            }
        }

        public bool TryEnqueue(T item)
        {
            lock (_locker)
            {
                if (_isCompleted)
                {
                    throw new Exception("Queue is completed.");
                }

                if (IsFull())
                {
                    return false;
                }

                NotThreadSafeInnerAdd(item);
                return true;
            }
        }

        public bool Dequeue(out T item)
        {
            return InnerDequeue(out item, true);
        }

        public bool Peek(out T item)
        {
            return InnerDequeue(out item, false);
        }

        public void Complete()
        {
            lock (_locker)
            {
                _isCompleted = true;
                Monitor.PulseAll(_locker);
            }
        }

        private bool IsFull()
        {
            return _queue.Count >= MaxSize;
        }

        private void NotThreadSafeInnerAdd(T item)
        {
            _queue.Enqueue(item);

            if (_queue.Count == 1)
            {
                Monitor.PulseAll(_locker);
            }
        }

        private bool InnerDequeue(out T item, bool extract)
        {
            lock (_locker)
            {
                while (_queue.Count == 0)
                {
                    if (_isCompleted)
                    {
                        item = default;
                        return false;
                    }

                    Monitor.Wait(_locker);
                }

                item = extract ? _queue.Dequeue() : _queue.Peek();

                if (extract && (_queue.Count == MaxSize - 1))
                {
                    Monitor.PulseAll(_locker);
                }

                return true;
            }
        }
    }

}
