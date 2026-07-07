using com.github.yukon39.CoverageBSL.AddIn.Debugger;
using System;

namespace com.github.yukon39.CoverageBSL.AddIn.Converters.ModulePathResolvers
{
    internal abstract class CoverageModulePathResolverBase
    {
        public abstract SourceFilesFormatEnum Format { get; }

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

        private static bool IsConfigOrExtensionModule(string moduleType)
            => string.Equals(moduleType, "ConfigModule", StringComparison.OrdinalIgnoreCase)
               || string.Equals(moduleType, "ExtensionModule", StringComparison.OrdinalIgnoreCase);
    }
}
