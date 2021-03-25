using multithreaded_gzip.ConcurrencyUtilities;
using System;

namespace multithreaded_gzip.Archiver
{
    public interface IParallelArchiver
    {
        void Process(Action onFinished, Cancellation cancellation = null);
    }
}
