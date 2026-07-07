using com.github.yukon39.CoverageBSL.AddIn.Converters.ModulePathResolvers;
using com.github.yukon39.CoverageBSL.AddIn.Debugger;
using com.github.yukon39.CoverageBSL.AddIn.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace com.github.yukon39.CoverageBSL.AddIn.Converters
{
    internal static class CoverageModulePathResolver
    {
        public static string Resolve(CoverageModuleIdWrapper moduleId, IEnumerable<SourceDirectoryData> projects)
        {
            var project = projects.FirstOrDefault()
                ?? throw new InvalidOperationException("Source projects list is empty.");

            return Resolve(moduleId, project);
        }

        public static string Resolve(CoverageModuleIdWrapper moduleId, SourceDirectoryData project)
            => ModulePathResolverRegistry.GetRequired(project.Format).Resolve(moduleId, project.Path);
    }
}
