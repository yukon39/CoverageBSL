using com.github.yukon39.CoverageBSL.Impl;

namespace com.github.yukon39.CoverageBSL
{
    public class CoverageFactory
    {
        public static ICoverageManager NewManager(string debuggerURI) =>
            new CoverageManager(debuggerURI);
    }
}
