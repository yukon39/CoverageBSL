using System.Collections.Generic;

namespace com.github.yukon39.CoverageBSL.AddIn.CoverageData
{
    internal sealed class CoverageModuleData
    {
        private readonly SortedDictionary<int, CoverageModuleLineData> lineInfo =
            new SortedDictionary<int, CoverageModuleLineData>();

        public CoverageModuleData(string moduleUrl)
            => ModuleUrl = moduleUrl;

        public string ModuleUrl { get; }

        public IEnumerable<CoverageModuleLineData> LineInfo => lineInfo.Values;

        public void AddLine(int lineNo)
        {
            if (!lineInfo.ContainsKey(lineNo))
            {
                lineInfo.Add(lineNo, new CoverageModuleLineData(lineNo, false));
            }
        }

        public void AddLines(IEnumerable<int> lineNumbers)
        {
            foreach (var lineNo in lineNumbers)
            {
                AddLine(lineNo);
            }
        }

        public void MarkCovered(int lineNo)
        {
            if (lineInfo.TryGetValue(lineNo, out var line))
            {
                line.MarkCovered();
                return;
            }

            lineInfo.Add(lineNo, new CoverageModuleLineData(lineNo, true));
        }

    }
}
