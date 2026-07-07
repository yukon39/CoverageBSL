using com.github.yukon39.CoverageBSL.AddIn.Converters.ModulePathResolvers.Edt;
using com.github.yukon39.CoverageBSL.AddIn.Converters.ModulePathResolvers.Xml;
using com.github.yukon39.CoverageBSL.AddIn.Debugger;
using System;
using System.Collections.Generic;

namespace com.github.yukon39.CoverageBSL.AddIn.Converters.ModulePathResolvers
{
    internal static class ModulePathResolverRegistry
    {
        private static readonly IReadOnlyDictionary<SourceFilesFormatEnum, ICoverageModulePathResolver> Resolvers =
            new Dictionary<SourceFilesFormatEnum, ICoverageModulePathResolver>
            {
                [SourceFilesFormatEnum.XML] = new XmlCoverageModulePathResolver(),
                [SourceFilesFormatEnum.EDT] = new EdtCoverageModulePathResolver()
            };

        public static ICoverageModulePathResolver GetRequired(SourceFilesFormatEnum format)
        {
            if (!Resolvers.TryGetValue(format, out var resolver))
            {
                throw new NotSupportedException($"Unsupported source files format: {format}");
            }

            return resolver;
        }
    }
}
