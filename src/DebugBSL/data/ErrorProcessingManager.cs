using com.github.yukon39.DebugBSL.data.core;

namespace com.github.yukon39.DebugBSL.data
{
    public static class ErrorProcessingManager
    {
        public static string BriefErrorDescription(GenericException genericException) => genericException.Description;
    }
}
