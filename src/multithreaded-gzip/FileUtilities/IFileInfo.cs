namespace multithreaded_gzip.FileUtilities
{
    public interface IFileInfo
    {
        string FullName { get; }
        long Length { get; }
    }
}
