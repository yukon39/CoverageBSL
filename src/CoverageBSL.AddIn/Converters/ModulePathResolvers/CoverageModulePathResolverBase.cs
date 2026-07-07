using com.github.yukon39.CoverageBSL.AddIn.Debugger;
using System;
using System.Collections.Generic;
using System.IO;

namespace com.github.yukon39.CoverageBSL.AddIn.Converters.ModulePathResolvers
{
    internal abstract class CoverageModulePathResolverBase : ICoverageModulePathResolver
    {
        public abstract SourceFilesFormatEnum Format { get; }

        protected abstract string MetadataFileExtension { get; }

        public abstract string GetSourceRoot(string projectPath);

        public string Resolve(CoverageModuleIdWrapper moduleId, string projectPath)
        {
            var sourcePath = GetSourceRoot(projectPath);

            if (string.Equals(moduleId.ModuleType, "ExtMDModule", StringComparison.OrdinalIgnoreCase))
            {
                return ResolveExternalModule(moduleId, sourcePath);
            }

            if (!IsConfigOrExtensionModule(moduleId.ModuleType))
            {
                throw new NotSupportedException($"Unsupported module type: {moduleId.ModuleType}");
            }

            return ResolveConfigModule(moduleId, sourcePath);
        }

        protected abstract string ResolveConfigModule(CoverageModuleIdWrapper moduleId, string sourcePath);

        protected abstract string ResolveExternalModule(CoverageModuleIdWrapper moduleId, string sourcePath);

        protected string FindNestedChildByUuid(
            string sourcePath,
            string objectId,
            IEnumerable<string> directories,
            string childDirectory)
        {
            foreach (var directory in directories)
            {
                var path = FindNestedChildByUuid(sourcePath, objectId, directory, childDirectory);
                if (path != null)
                {
                    return path;
                }
            }

            return null;
        }

        protected string FindNestedChildByUuid(
            string sourcePath,
            string objectId,
            string directory,
            string childDirectory)
        {
            if (!MetadataUuidIndex.TryGetFilePath(sourcePath, MetadataFileExtension, objectId, out var filePath))
            {
                return null;
            }

            if (!MetadataPathHelper.HasExtension(filePath, MetadataFileExtension))
            {
                return null;
            }

            var searchRoot = Path.Combine(sourcePath, directory);
            if (!MetadataPathHelper.IsUnderDirectory(filePath, searchRoot))
            {
                return null;
            }

            return MetadataPathHelper.ContainsDirectorySegment(filePath, childDirectory) ? filePath : null;
        }

        private static bool IsConfigOrExtensionModule(string moduleType)
            => string.Equals(moduleType, "ConfigModule", StringComparison.OrdinalIgnoreCase)
               || string.Equals(moduleType, "ExtensionModule", StringComparison.OrdinalIgnoreCase);
    }
}
