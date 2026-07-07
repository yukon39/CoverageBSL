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
            if (!string.Equals(moduleId.ModuleType, "ConfigModule", StringComparison.OrdinalIgnoreCase))
            {
                throw new NotSupportedException($"Unsupported module type: {moduleId.ModuleType}");
            }

            return ResolveConfigModule(moduleId, GetSourceRoot(projectPath));
        }

        protected abstract string ResolveConfigModule(CoverageModuleIdWrapper moduleId, string sourcePath);
    }
}