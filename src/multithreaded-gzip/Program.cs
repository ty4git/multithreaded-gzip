using multithreaded_gzip.Archiver;
using multithreaded_gzip.ConcurrencyUtilities;
using multithreaded_gzip.FileUtilities;
using System;
using System.Linq;
using System.Threading;

namespace multithreaded_gzip
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Use pattern: (compress | decompress) <source file name> <output file name>");

            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += (sender, eventArgs) =>
            {
                Console.WriteLine((Exception)eventArgs.ExceptionObject);
                Environment.Exit(1);
            };

            ValidateArgs(args);

            var cancellation = new Cancellation();
            Console.WriteLine("Press Ctrl-C to cancel.");
            var cancelledEvent = new AutoResetEvent(false);
            Console.CancelKeyPress += (sender, args) =>
            {
                cancellation.Cancel();
                args.Cancel = true;
                cancelledEvent.Set();
            };

            var isCompression = args[0] == "compress";
            var sourceFileName = args[1];
            var targetFileName = args[2];

            var finishedEvent = new AutoResetEvent(false);
            var sourceFile = new FileWrapper(sourceFileName);
            var targetFile = new FileWrapper(targetFileName);
            var blockingQueueSize = Environment.ProcessorCount * 2;
            var partSize = 1024 * 16;

            var parallelArchiver = isCompression
                ? (IParallelArchiver)new ParallelCompressor(sourceFile, targetFile, blockingQueueSize, partSize)
                : new ParallelDecompressor(sourceFile, targetFile, blockingQueueSize, partSize);

            parallelArchiver.Process(() => { finishedEvent.Set(); }, cancellation);

            var eventWaitHandles = new[] { finishedEvent, cancelledEvent };
            var eventIndex = WaitHandle.WaitAny(eventWaitHandles);

            if (eventWaitHandles[eventIndex] == finishedEvent)
            {
                Console.WriteLine("Application has finished successfully.");
            }
            else
            {
                Console.WriteLine("Application has been cancelled.");
            }

            Environment.ExitCode = 0;
        }

        static void ValidateArgs(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("Arguments should exist.");
            }

            if (args.Length != 3)
            {
                throw new Exception("Count of args is invalid.");
            }

            var modes = new[] { "compress", "decompress" };
            if (!modes.Contains(args[0]))
            {
                throw new Exception(@"Compression mode argument is invalid. Should be ""compress"" or ""decompress"".");
            }
        }
    }
}
