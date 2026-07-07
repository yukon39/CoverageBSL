using com.github.yukon39.CoverageBSL.AddIn.CoverageData;
using System;
using System.Collections.Generic;
using System.IO;

namespace com.github.yukon39.CoverageBSL.AddIn.Internal
{
    internal static class CoverableLinesBuilder
    {
        public static CoverageDataSet Build(string workingArea, IEnumerable<SourceDirectoryData> sourceDirectories)
        {
            var basePath = ResolveWorkingArea(workingArea);
            var coverageData = new CoverageDataSet();
            var processedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var sourceDirectory in sourceDirectories)
            {
                var directoryPath = ResolveSourceDirectory(basePath, sourceDirectory.Path);
                if (!Directory.Exists(directoryPath))
                {
                    throw new DirectoryNotFoundException(directoryPath);
                }

                foreach (var filePath in Directory.EnumerateFiles(directoryPath, "*.bsl", SearchOption.AllDirectories))
                {
                    var fullPath = Path.GetFullPath(filePath);
                    if (!processedFiles.Add(fullPath))
                    {
                        continue;
                    }

                    var lineNumbers = CoverableLinesProvider.ExtractCoverableLinesFromFile(fullPath);
                    var module = coverageData.GetOrAdd(ToModuleUrl(basePath, fullPath));
                    module.AddLines(lineNumbers);
                }
            }

            return coverageData;
        }

        private static string ResolveWorkingArea(string workingArea)
        {
            if (string.IsNullOrWhiteSpace(workingArea))
            {
                return Directory.GetCurrentDirectory();
            }

            return Path.GetFullPath(workingArea);
        }

        private static string ResolveSourceDirectory(string basePath, string sourceDirectory)
        {
            if (Path.IsPathRooted(sourceDirectory))
            {
                return Path.GetFullPath(sourceDirectory);
            }

            return Path.GetFullPath(Path.Combine(basePath, sourceDirectory));
        }

        private static string ToModuleUrl(string basePath, string filePath)
            => Path.GetRelativePath(basePath, filePath).Replace('\\', '/');
    }
}
