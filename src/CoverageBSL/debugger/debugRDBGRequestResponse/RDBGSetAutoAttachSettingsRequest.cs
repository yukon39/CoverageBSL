using com.github.yukon39.CoverageBSL.debugger.debugAutoAttach;

namespace com.github.yukon39.CoverageBSL.debugger.debugRDBGRequestResponse
{
    class RDBGSetAutoAttachSettingsRequest : RDbgBaseRequest, IRDBGRequest
    {
        public DebugAutoAttachSettings autoAttachSettings { get; set; }
    }
}
