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
            if (!IsResolvableModuleType(moduleId.ModuleType))
            {
                return null;
            }

            var modulePath = ResolveModulePath(moduleId, workingArea, sourceDirectories);
            if (modulePath == null)
            {
                return null;
            }

            return ToModuleUrl(workingArea, modulePath);
        }

        private static bool IsResolvableModuleType(string moduleType)
            => string.Equals(moduleType, "ConfigModule", StringComparison.OrdinalIgnoreCase)
               || string.Equals(moduleType, "ExtensionModule", StringComparison.OrdinalIgnoreCase)
               || string.Equals(moduleType, "ExtMDModule", StringComparison.OrdinalIgnoreCase);

        private static string ResolveModulePath(
            CoverageModuleIdWrapper moduleId,
            string workingArea,
            IEnumerable<SourceDirectoryData> sourceDirectories)
        {
            foreach (var sourceDirectory in sourceDirectories)
            {
                if (!MatchesModuleSource(moduleId, sourceDirectory))
                {
                    continue;
                }

                var resolvedSourceDirectory = new SourceDirectoryData(
                    ResolveSourceDirectory(workingArea, sourceDirectory.Path),
                    sourceDirectory.Format,
                    sourceDirectory.ExtensionName,
                    sourceDirectory.ModuleUrl);
                var modulePath = CoverageModulePathResolver.Resolve(moduleId, resolvedSourceDirectory);
                if (modulePath != null)
                {
                    return modulePath;
                }
            }

            return null;
        }

        private static string ResolveSourceDirectory(string workingArea, string sourceDirectory)
        {
            if (Path.IsPathRooted(sourceDirectory))
            {
                return Path.GetFullPath(sourceDirectory);
            }

            var basePath = string.IsNullOrWhiteSpace(workingArea)
                ? Directory.GetCurrentDirectory()
                : Path.GetFullPath(workingArea);

            return Path.GetFullPath(Path.Combine(basePath, sourceDirectory));
        }

        private static bool MatchesModuleSource(
            CoverageModuleIdWrapper moduleId,
            SourceDirectoryData sourceDirectory)
        {
            if (string.Equals(moduleId.ModuleType, "ConfigModule", StringComparison.OrdinalIgnoreCase))
            {
                return string.IsNullOrEmpty(sourceDirectory.ExtensionName)
                    && string.IsNullOrEmpty(sourceDirectory.ModuleUrl);
            }

            if (string.Equals(moduleId.ModuleType, "ExtensionModule", StringComparison.OrdinalIgnoreCase))
            {
                return string.IsNullOrEmpty(sourceDirectory.ModuleUrl)
                    && string.Equals(
                        sourceDirectory.ExtensionName,
                        moduleId.ExtensionName,
                        StringComparison.OrdinalIgnoreCase);
            }

            if (string.Equals(moduleId.ModuleType, "ExtMDModule", StringComparison.OrdinalIgnoreCase))
            {
                return string.IsNullOrEmpty(sourceDirectory.ExtensionName)
                    && CoverageModuleUrlHelper.Matches(moduleId.URL, sourceDirectory.ModuleUrl);
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
