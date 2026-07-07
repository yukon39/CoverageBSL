using System;
using System.Collections.Generic;
using System.Linq;

namespace com.github.yukon39.CoverageBSL.AddIn.CoverageData
{
    internal sealed class CoverageDataSet
    {
        private readonly Dictionary<string, CoverageModuleData> modules =
            new(StringComparer.OrdinalIgnoreCase);

        public IEnumerable<CoverageModuleData> Modules => modules.Values;

        public int TotalLineCount => modules.Values.Sum(module => module.LineInfo.Count());

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
