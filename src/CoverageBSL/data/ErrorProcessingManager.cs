using com.github.yukon39.CoverageBSL.data.core;

namespace com.github.yukon39.CoverageBSL.data
{
    public static class ErrorProcessingManager
    {
        public static string BriefErrorDescription(GenericException genericException) => genericException.Description;
    }
}
