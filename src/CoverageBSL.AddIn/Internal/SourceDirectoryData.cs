namespace com.github.yukon39.CoverageBSL.AddIn.Internal
{
    internal sealed class SourceDirectoryData(string path, SourceFilesFormatEnum format)
    {
        public string Path { get; } = path;

        public SourceFilesFormatEnum Format { get; } = format;
    }
}
