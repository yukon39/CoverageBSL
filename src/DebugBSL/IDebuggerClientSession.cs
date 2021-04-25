using com.github.yukon39.DebugBSL.debugger.debugAutoAttach;
using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using com.github.yukon39.DebugBSL.debugger.debugDBGUICommands;
using com.github.yukon39.DebugBSL.debugger.debugMeasure;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using System;
using System.Collections.Generic;

namespace com.github.yukon39.DebugBSL
{
    public interface IDebuggerClientSession
    {
        public delegate void TargetStartedHandler(DebugTargetId TargetID);
        public event TargetStartedHandler TargetStarted;

        public delegate void TargetQuitHandler(DebugTargetId TargetID);
        public event TargetQuitHandler TargetQuit;

        public delegate void MeasureProcessingHandler(PerformanceInfoMain TargetID);
        public event MeasureProcessingHandler MeasureProcessing;

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

        public void SetMeasureMode(Guid measureMode);
    }
}
