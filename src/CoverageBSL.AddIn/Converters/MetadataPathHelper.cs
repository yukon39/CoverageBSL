using System;
using System.IO;

namespace com.github.yukon39.CoverageBSL.AddIn.Converters
{
    internal static class MetadataPathHelper
    {
        public static bool IsDirectChild(string filePath, string directory)
        {
            var fileDirectory = Path.GetDirectoryName(filePath);
            return string.Equals(
                Path.GetFullPath(fileDirectory!),
                Path.GetFullPath(directory),
                StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsUnderDirectory(string filePath, string directory)
        {
            var fullFilePath = Path.GetFullPath(filePath);
            var fullDirectory = Path.GetFullPath(directory);
            if (!fullDirectory.EndsWith(Path.DirectorySeparatorChar))
            {
                fullDirectory += Path.DirectorySeparatorChar;
            }

            return fullFilePath.StartsWith(fullDirectory, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ContainsDirectorySegment(string filePath, string segment)
        {
            var parts = filePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            return Array.Exists(parts, part => string.Equals(part, segment, StringComparison.OrdinalIgnoreCase));
        }

        public static bool HasExtension(string filePath, string extension)
            => string.Equals(Path.GetExtension(filePath), extension, StringComparison.OrdinalIgnoreCase);
    }
}
