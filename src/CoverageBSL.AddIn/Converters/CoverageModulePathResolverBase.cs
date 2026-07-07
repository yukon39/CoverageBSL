using com.github.yukon39.CoverageBSL.AddIn.Debugger;
using System;

namespace com.github.yukon39.CoverageBSL.AddIn.Converters
{
    internal abstract class CoverageModulePathResolverBase
    {
        public abstract SourceFilesFormatEnum Format { get; }

        public abstract string GetSourceRoot(string projectPath);

        public string Resolve(CoverageModuleIdWrapper moduleId, string projectPath)
        {
            if (!IsSupportedModuleType(moduleId.ModuleType))
            {
                throw new NotSupportedException($"Unsupported module type: {moduleId.ModuleType}");
            }

            return ResolveConfigModule(moduleId, GetSourceRoot(projectPath));
        }

        private static bool IsSupportedModuleType(string moduleType)
            => string.Equals(moduleType, "ConfigModule", StringComparison.OrdinalIgnoreCase)
               || string.Equals(moduleType, "ExtensionModule", StringComparison.OrdinalIgnoreCase);

        protected abstract string ResolveConfigModule(CoverageModuleIdWrapper moduleId, string sourcePath);
    }
}