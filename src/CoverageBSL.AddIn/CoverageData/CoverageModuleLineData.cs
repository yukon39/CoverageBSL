namespace com.github.yukon39.CoverageBSL.AddIn.CoverageData
{
    internal sealed class CoverageModuleLineData
    {
        public CoverageModuleLineData(int lineNo, bool hasCoverage = false)
        {
            LineNo = lineNo;
            HasCoverage = hasCoverage;
        }

        public int LineNo { get; }

        public bool HasCoverage { get; private set; }

        public void MarkCovered()
            => HasCoverage = true;
    }
}
