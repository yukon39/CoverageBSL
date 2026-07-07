using com.github.yukon39.CoverageBSL.AddIn.Debugger;
using com.github.yukon39.CoverageBSL.AddIn.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace com.github.yukon39.CoverageBSL.AddIn.Converters
{
    internal static class CoverageModulePathResolver
    {
        private static readonly IReadOnlyDictionary<SourceFilesFormatEnum, ICoverageModulePathResolver> Resolvers =
            new Dictionary<SourceFilesFormatEnum, ICoverageModulePathResolver>
            {
                [SourceFilesFormatEnum.XML] = new XmlCoverageModulePathResolver(),
                [SourceFilesFormatEnum.EDT] = new EdtCoverageModulePathResolver()
            };

        public static string Resolve(CoverageModuleIdWrapper moduleId, IEnumerable<SourceDirectoryData> projects)
        {
            var project = projects.FirstOrDefault()
                ?? throw new InvalidOperationException("Source projects list is empty.");

            return Resolve(moduleId, project);
        }

        public static string Resolve(CoverageModuleIdWrapper moduleId, SourceDirectoryData project)
        {
            if (!Resolvers.TryGetValue(project.Format, out var resolver))
            {
                throw new NotSupportedException($"Unsupported source files format: {project.Format}");
            }

            return resolver.Resolve(moduleId, project.Path);
        }
    }
}
