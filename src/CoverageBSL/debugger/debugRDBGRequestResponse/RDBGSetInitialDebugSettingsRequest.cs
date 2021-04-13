namespace com.github.yukon39.CoverageBSL.debugger.debugRDBGRequestResponse
{
    class RDBGSetInitialDebugSettingsRequest : RDbgBaseRequest, IRDBGRequest
    {
        public HTTPServerInitialDebugSettingsData data { get; set; }
    }
}
