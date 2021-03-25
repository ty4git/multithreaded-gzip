#define UseExampleFile

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using multithreaded_gzip.Archiver;
using multithreaded_gzip.ConcurrencyUtilities;
using multithreaded_gzip.FileUtilities;

namespace measurments
{

    class Program
    {
        static void Main(string[] args)
        {
            //BenchmarkRunner.Run<Measuring>();

            //custom benchmark.
            //new Measuring().SingleThreadStreamCompress();
            //new Measuring().MultipleThreadsStreamCompressViaLinq();
            //new Measuring().TaskBasedParallelArchiver();
            //new Measuring().MultipleThreadsParallelArchiver();

            //console application test.
            new Measuring().MultithreadedGzipMain();
        }
    }

    [MemoryDiagnoser]
    public class Measuring
    {
        private readonly Configuration _configuration;

        public Measuring()
        {
            var minBufferSize = 1024 * 8;
            var maxBufferSize = 1024 * 2024;

#if UseExampleFile
            var nameOfFileToCompress = "example.txt";
            var fileToCompressInfo = new FileInfo(nameOfFileToCompress);
            var nameOfCompressedFile = Path.Combine(fileToCompressInfo.DirectoryName,
                $"{Path.GetFileNameWithoutExtension(fileToCompressInfo.FullName)}.gz");
            var nameOfDecompressedFile = Path.Combine(fileToCompressInfo.DirectoryName,
                $"{Path.GetFileNameWithoutExtension(fileToCompressInfo.Name)}-decompressed{fileToCompressInfo.Extension}");

            var fileToCompress = new FileWrapper(fileToCompressInfo);
            var compressedFile = new FileWrapper(nameOfCompressedFile);
            var decompressingFile = compressedFile;
            var decompressedFile = new FileWrapper(nameOfDecompressedFile);
#elif UseStub
            var fileToCompress = new StubFile();
            var compressedFile = new StubFile();
            var compressedFileTarget = compressedFile;
            var compressedFileSource = compressedFile;
            var decompressedFile = new StubFile();
            var decompressedFile = decompressedFile;
#endif

            _configuration = new Configuration(
                minBufferSize,
                maxBufferSize,
                fileToCompress,
                compressedFile,
                decompressingFile,
                decompressedFile);
        }

        public void MultithreadedGzipMain()
        {
            UseDefaultConfiguration(defaultConfiguration =>
            {
                multithreaded_gzip.Program.Main(new[] { "compress", defaultConfiguration.CompressingFile.FullName,
                    defaultConfiguration.CompressedFile.FullName });
                multithreaded_gzip.Program.Main(new[] { "decompress", defaultConfiguration.DecompressingFile.FullName,
                    defaultConfiguration.DecompressedFile.FullName });
            });
        }

        public void TaskBasedParallelArchiver()
        {
            var cancellation = new CancellationToken();

            UseDefaultConfiguration((defaultConfiguration) =>
            {
                UseDifferentBufferSizes(defaultConfiguration, (bufferSize) =>
                {
                    var parallelCompressor = new TaskBasedParallelCompressor(
                        defaultConfiguration.CompressingFile,
                        defaultConfiguration.CompressedFile,
                        bufferSize);

                    UseMeasure((stopMeasure) =>
                    {
                        parallelCompressor.Process(cancellation).Wait();
                        stopMeasure();
                    }, defaultConfiguration.CompressingFile);
                });

                CompressionStream.DecompressByParts(
                    defaultConfiguration.DecompressingFile.FullName,
                    defaultConfiguration.DecompressedFile.FullName);

                CompareMd5(defaultConfiguration.CompressingFile, defaultConfiguration.DecompressedFile);
            });
        }

        public void MultipleThreadsParallelArchiver()
        {
            var cancellation = new Cancellation();
            var finishedEvent = new AutoResetEvent(false);

            UseDefaultConfiguration(defaultConfiguration =>
            {
                UseDifferentBufferSizes(defaultConfiguration, bufferSize =>
                {
                    var parallelCompressor = new ParallelCompressor(
                        defaultConfiguration.CompressingFile,
                        defaultConfiguration.CompressedFile,
                        1 * 10,
                        bufferSize);
                    UseMeasure((stopMeasure) =>
                    {
                        parallelCompressor.Process(() => { stopMeasure(); finishedEvent.Set(); }, cancellation);
                    }, defaultConfiguration.CompressingFile);

                    finishedEvent.WaitOne();
                });

                var parallelDecompressor = new ParallelDecompressor(
                    defaultConfiguration.DecompressingFile,
                    defaultConfiguration.DecompressedFile);
                Console.WriteLine();
                UseMeasure((stopMeasure) =>
                {
                    parallelDecompressor.Process(() => { stopMeasure(); finishedEvent.Set(); }, cancellation);
                }, defaultConfiguration.DecompressingFile);
                finishedEvent.WaitOne();

                CompareMd5(defaultConfiguration.CompressingFile, defaultConfiguration.DecompressedFile);
            });
        }

        public void SingleThreadStreamCompress()
        {
            UseDefaultConfiguration(defaultConfiguration =>
            {
                UseDifferentBufferSizes(defaultConfiguration, bufferSize =>
                {
                    UseMeasure(stopMeasure =>
                    {
                        CompressionStream.Compress(
                            defaultConfiguration.CompressingFile.FullName,
                            defaultConfiguration.CompressedFile.FullName,
                            bufferSize);
                        stopMeasure();
                    }, defaultConfiguration.CompressingFile);
                });
            });
        }

        public void MultipleThreadsStreamCompressViaLinq()
        {
            UseDefaultConfiguration(defaultConfiguration =>
            {
                UseDifferentBufferSizes(defaultConfiguration, bufferSize =>
                {
                    UseMeasure(stopMeasure =>
                    {
                        CompressionStream.MultipleThreadsCompressByParts(
                            defaultConfiguration.CompressingFile.FullName,
                            defaultConfiguration.CompressedFile.FullName,
                            bufferSize);
                        stopMeasure();
                    }, defaultConfiguration.CompressingFile);
                });
            });
        }

        public void OnlyGZipStream()
        {
            UseDefaultConfiguration(defaultConfiguration =>
            {
                UseDifferentBufferSizes(defaultConfiguration, bufferSize =>
                {
                    UseMeasure(stopMeasure =>
                    {
                        var source = defaultConfiguration.CompressingFile;
                        var target = defaultConfiguration.CompressedFile;
                        target.WriteByParts(
                            source.ReadByParts(bufferSize)
                                .Select(part => CompressionTools.CompressPart(part)));
                        stopMeasure();
                    }, defaultConfiguration.CompressingFile);
                });
            });
        }

        public void MultipleThreadsBlockedParallelCompressor()
        {
            UseDefaultConfiguration(defaultConfiguration =>
            {
                UseDifferentBufferSizes(defaultConfiguration, bufferSize =>
                {
                    var pc = new ParallelCompressor(
                        defaultConfiguration.CompressingFile,
                        defaultConfiguration.CompressedFile,
                        1 * 10, bufferSize);

                    UseMeasure(stopMeasure =>
                    {
                        pc.Process(() => { stopMeasure(); },
                            new Cancellation());
                    }, defaultConfiguration.CompressingFile);
                });
            });
        }

        static void DifferentBufferSizes(Action<int> actionWithBufferSize,
            int minBufferSize = 1024 * 4,
            int maxBufferSize = 1024 * 1024)
        {
            var KB = 1024;
            var bufferSizes = Enumerable.Range(0, 20)
                .Select(i => KB * (int)Math.Pow(2, i))
                .Where(bufferSize => bufferSize >= minBufferSize && bufferSize <= maxBufferSize)
                .ToList();

            foreach (var bufferSize in bufferSizes)
            {
                Console.WriteLine();
                var bufferSizeKB = Math.Round((double)bufferSize / KB, 3);
                var bufferSizeMB = Math.Round(bufferSizeKB / KB, 3);
                Console.WriteLine($"buffer size: {bufferSize} B = {bufferSize / KB} KB = {bufferSizeMB} MB");

                actionWithBufferSize(bufferSize);
            }
        }

        private static void UseDifferentBufferSizes(Configuration configuration, Action<int> actionWithBufferSize)
        {
            DifferentBufferSizes(actionWithBufferSize, configuration.MinBufferSize, configuration.MaxBufferSize);
        }

        public static void UseMeasure(Action<Action> action, IFileInfo fileInfo)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            action(() =>
            {
                stopwatch.Stop();
                Console.WriteLine(Path.GetFileName(fileInfo.FullName));
                var totalms = stopwatch.Elapsed.TotalMilliseconds;
                var totals = stopwatch.Elapsed.TotalSeconds;
                Console.WriteLine($"total time: {totalms} ms = {totals} s");
                Console.WriteLine("total time: min:sec.ms: " + stopwatch.Elapsed.ToString(@"mm\:ss\.fff"));
                var fileSizeB = fileInfo.Length;
                var fileSizeKB = Math.Round((double)fileSizeB / 1024, 3);
                var fileSizeMB = Math.Round(fileSizeKB / 1024, 3);
                var fileSizeGB = Math.Round(fileSizeMB / 1024, 3);
                Console.WriteLine($"file size: {fileSizeB} B = {fileSizeKB} KB = {fileSizeMB} MB = {fileSizeGB} GB");
                var Bs = Math.Round(fileSizeB / totals, 3);
                var KBs = Math.Round(fileSizeKB / totals, 3);
                var MBs = Math.Round(fileSizeMB / totals, 3);
                Console.WriteLine($"speed: {Bs} B/s = {KBs} KB/s = {MBs} MB/s");
            });
            stopwatch.Start();
        }

        private void UseConfiguration(Configuration configuration, Action<Configuration> actionWithConfiguration)
        {
            actionWithConfiguration(configuration);
        }

        private void UseDefaultConfiguration(Action<Configuration> actionWithConfiguration)
        {
            UseConfiguration(_configuration, actionWithConfiguration);
        }

        private void CompareMd5(IFileInfo fileToCompress, IFileInfo decompressedFile)
        {
            var result = new IFileInfo[] { fileToCompress, decompressedFile }
                .Select(file => (file.FullName, md5: FileTools.CalculateMD5(file.FullName)))
                .ToList();

            result.ForEach(x => Console.WriteLine($"{x.FullName}: {x.md5}"));
            Console.WriteLine(result[0].md5 == result[1].md5);
        }
    }

    class Configuration
    {
        public Configuration(
            int minBufferSize,
            int maxBufferSize,
            IFileSource compressingFile,
            IFileTarget compressedFile,
            IFileSource decompressingFile,
            IFileTarget decompressedFile)
        {
            MinBufferSize = minBufferSize;
            MaxBufferSize = maxBufferSize;
            CompressingFile = compressingFile ?? throw new ArgumentNullException(nameof(compressingFile));
            CompressedFile = compressedFile ?? throw new ArgumentNullException(nameof(compressedFile));
            DecompressingFile = decompressingFile ?? throw new ArgumentNullException(nameof(decompressingFile));
            DecompressedFile = decompressedFile ?? throw new ArgumentNullException(nameof(decompressedFile));
        }

        public int MinBufferSize { get; }
        public int MaxBufferSize { get; }
        public IFileSource CompressingFile { get; }
        public IFileTarget CompressedFile { get; }
        public IFileSource DecompressingFile { get; }
        public IFileTarget DecompressedFile { get; }
        public int BufferSize { get; set; }
        public Action StopMeasure { get; set; }
    }
}
