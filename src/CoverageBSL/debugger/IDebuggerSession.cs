using com.github.yukon39.CoverageBSL.debugger.debugAutoAttach;
using com.github.yukon39.CoverageBSL.debugger.debugBaseData;
using com.github.yukon39.CoverageBSL.debugger.debugDBGUICommands;
using com.github.yukon39.CoverageBSL.debugger.debugRDBGRequestResponse;
using System.Collections.Generic;

namespace com.github.yukon39.CoverageBSL.debugger
{
    public interface IDebuggerSession
    {
        public bool IsAttached();

        public AttachDebugUIResult Attach(char[] Password, DebuggerOptions Options);

        public bool Detach();

        public void AttachDebugTarget(DebugTargetIdLight target);

        public void DetachDebugTarget(DebugTargetIdLight target);

        public List<DbgTargetStateInfo> AttachedTargetsStates(string areaName);
       
        public void InitSettings(HTTPServerInitialDebugSettingsData Data);

        public void SetAutoAttachSettings(DebugAutoAttachSettings AutoAttachSettings);

        public void ClearBreakOnNextStatement();

        public List<DBGUIExtCmdInfoBase> Ping();
    }
}
