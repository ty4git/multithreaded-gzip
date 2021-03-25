using System;
using System.Threading;

namespace multithreaded_gzip.ConcurrencyUtilities
{
    class BlockingWorkItem<T>
    {
        private T _item;
        private volatile bool _hasItem = false;
        private readonly object _locker = new object();

        public T GetResult()
        {
            if (_hasItem)
            {
                return _item;
            }

            lock (_locker)
            {
                if (!_hasItem)
                {
                    Monitor.Wait(_locker);
                }

                return _item;
            }
        }

        public void SetResult(T item)
        {
            lock (_locker)
            {
                if (_hasItem)
                {
                    throw new Exception("Item is set already.");
                }

                _item = item;
                _hasItem = true;
                Monitor.PulseAll(_locker);
            }
        }
    }
}
