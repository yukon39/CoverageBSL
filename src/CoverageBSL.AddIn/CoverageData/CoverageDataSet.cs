using System;
using System.Collections.Generic;

namespace com.github.yukon39.CoverageBSL.AddIn.CoverageData
{
    internal sealed class CoverageDataSet
    {
        private readonly Dictionary<string, CoverageModuleData> modules =
            new Dictionary<string, CoverageModuleData>(StringComparer.OrdinalIgnoreCase);

        public IEnumerable<CoverageModuleData> Modules => modules.Values;

        public CoverageModuleData GetOrAdd(string moduleUrl)
        {
            if (!modules.TryGetValue(moduleUrl, out var module))
            {
                module = new CoverageModuleData(moduleUrl);
                modules.Add(moduleUrl, module);
            }

            return module;
        }

        public CoverageModuleData Get(string moduleUrl)
            => modules.TryGetValue(moduleUrl, out var module) ? module : null;

    }
}
