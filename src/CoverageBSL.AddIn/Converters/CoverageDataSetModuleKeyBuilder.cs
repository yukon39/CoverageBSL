using com.github.yukon39.CoverageBSL.AddIn.Debugger;
using com.github.yukon39.CoverageBSL.AddIn.Internal;
using System;
using System.Collections.Generic;
using System.IO;

namespace com.github.yukon39.CoverageBSL.AddIn.Converters
{
    internal static class CoverageDataSetModuleKeyBuilder
    {
        public static string Build(
            CoverageModuleIdWrapper moduleId,
            string workingArea,
            IEnumerable<SourceDirectoryData> sourceDirectories)
        {
            if (!string.IsNullOrWhiteSpace(moduleId.URL))
            {
                return moduleId.URL;
            }

            if (!IsResolvableModuleType(moduleId.ModuleType))
            {
                return null;
            }

            var modulePath = ResolveModulePath(moduleId, sourceDirectories);
            if (modulePath == null)
            {
                return null;
            }

            return ToModuleUrl(workingArea, modulePath);
        }

        private static bool IsResolvableModuleType(string moduleType)
            => string.Equals(moduleType, "ConfigModule", StringComparison.OrdinalIgnoreCase)
               || string.Equals(moduleType, "ExtensionModule", StringComparison.OrdinalIgnoreCase);

        private static string ResolveModulePath(
            CoverageModuleIdWrapper moduleId,
            IEnumerable<SourceDirectoryData> sourceDirectories)
        {
            foreach (var sourceDirectory in sourceDirectories)
            {
                if (!MatchesModuleSource(moduleId, sourceDirectory))
                {
                    continue;
                }

                var modulePath = CoverageModulePathResolver.Resolve(moduleId, sourceDirectory);
                if (modulePath != null)
                {
                    return modulePath;
                }
            }

            return null;
        }

        private static bool MatchesModuleSource(
            CoverageModuleIdWrapper moduleId,
            SourceDirectoryData sourceDirectory)
        {
            if (string.Equals(moduleId.ModuleType, "ConfigModule", StringComparison.OrdinalIgnoreCase))
            {
                return string.IsNullOrEmpty(sourceDirectory.ExtensionName);
            }

            if (string.Equals(moduleId.ModuleType, "ExtensionModule", StringComparison.OrdinalIgnoreCase))
            {
                return string.Equals(
                    sourceDirectory.ExtensionName,
                    moduleId.ExtensionName,
                    StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private static string ToModuleUrl(string workingArea, string filePath)
        {
            var basePath = string.IsNullOrWhiteSpace(workingArea)
                ? Directory.GetCurrentDirectory()
                : Path.GetFullPath(workingArea);

            return Path.GetRelativePath(basePath, Path.GetFullPath(filePath)).Replace('\\', '/');
        }
    }
}
