using com.github.yukon39.CoverageBSL.AddIn.Converters;
using com.github.yukon39.CoverageBSL.AddIn.CoverageData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace com.github.yukon39.CoverageBSL.AddIn.Internal
{
    internal static class CoverableLinesBuilder
    {
        private static readonly IReadOnlyDictionary<SourceFilesFormatEnum, ICoverageModulePathResolver> Resolvers =
            new Dictionary<SourceFilesFormatEnum, ICoverageModulePathResolver>
            {
                [SourceFilesFormatEnum.XML] = new XmlCoverageModulePathResolver(),
                [SourceFilesFormatEnum.EDT] = new EdtCoverageModulePathResolver()
            };

        public static CoverageDataSet Build(string workingArea, IEnumerable<SourceDirectoryData> sourceDirectories)
        {
            var basePath = ResolveWorkingArea(workingArea);
            var coverageData = new CoverageDataSet();
            var processedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var sourceDirectory in sourceDirectories)
            {
                var projectPath = ResolveSourceDirectory(basePath, sourceDirectory.Path);
                if (!Directory.Exists(projectPath))
                {
                    throw new DirectoryNotFoundException(projectPath);
                }

                if (!Resolvers.TryGetValue(sourceDirectory.Format, out var resolver))
                {
                    throw new NotSupportedException($"Unsupported source files format: {sourceDirectory.Format}");
                }

                var scanRoot = resolver.GetSourceRoot(projectPath);
                if (!Directory.Exists(scanRoot))
                {
                    throw new DirectoryNotFoundException(scanRoot);
                }

                foreach (var filePath in Directory.EnumerateFiles(scanRoot, "*.bsl", SearchOption.AllDirectories))
                {
                    var fullPath = Path.GetFullPath(filePath);
                    if (!processedFiles.Add(fullPath))
                    {
                        continue;
                    }

                    var lineNumbers = CoverableLinesProvider.ExtractCoverableLinesFromFile(fullPath);
                    if (!lineNumbers.Any())
                    {
                        continue;
                    }

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
