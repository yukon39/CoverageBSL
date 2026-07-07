namespace com.github.yukon39.CoverageBSL.AddIn.CoverageData
{
    internal sealed class CoverageModuleLineData(int lineNo, bool hasCoverage = false)
    {
        public int LineNo { get; } = lineNo;

        public bool HasCoverage { get; private set; } = hasCoverage;

        public void MarkCovered()
            => HasCoverage = true;
    }
}
