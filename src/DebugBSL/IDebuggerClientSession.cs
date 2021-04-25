using com.github.yukon39.DebugBSL.debugger.debugAutoAttach;
using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using com.github.yukon39.DebugBSL.debugger.debugDBGUICommands;
using com.github.yukon39.DebugBSL.debugger.debugMeasure;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using System;
using System.Collections.Generic;

namespace com.github.yukon39.DebugBSL
{

    public delegate void TargetStartedHandler(DebugTargetId TargetID);
    public delegate void TargetQuitHandler(DebugTargetId TargetID);
    public delegate void MeasureProcessingHandler(PerformanceInfoMain TargetID);

    public interface IDebuggerClientSession
    {
        event TargetStartedHandler TargetStarted;
        event TargetQuitHandler TargetQuit;
        event MeasureProcessingHandler MeasureProcessing;

        bool IsAttached();

        AttachDebugUIResult Attach(char[] Password, DebuggerOptions Options);

        bool Detach();

        void AttachDebugTarget(DebugTargetIdLight target);

        void DetachDebugTarget(DebugTargetIdLight target);

        List<DbgTargetStateInfo> AttachedTargetsStates(string areaName);

        void InitSettings(HTTPServerInitialDebugSettingsData Data);

        void SetAutoAttachSettings(DebugAutoAttachSettings AutoAttachSettings);

        void ClearBreakOnNextStatement();

        List<DBGUIExtCmdInfoBase> Ping();

        void SetMeasureMode(Guid measureMode);
    }
}
