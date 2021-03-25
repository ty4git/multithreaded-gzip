namespace multithreaded_gzip.ConcurrencyUtilities
{
    public class Cancellation
    {
        private volatile bool _isCancelled;

        public bool IsCancelled()
        {
            return _isCancelled;
        }

        public void Cancel()
        {
            _isCancelled = true;
        }
    }
}
