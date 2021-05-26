using com.github.yukon39.DebugBSL.debugger.debugAutoAttach;
using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using com.github.yukon39.DebugBSL.debugger.debugMeasure;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace com.github.yukon39.DebugBSL.Client
{

    public delegate Task TargetStartedHandler(DebugTargetId TargetID);
    public delegate Task TargetQuitHandler(DebugTargetId TargetID);
    public delegate Task MeasureProcessingHandler(PerformanceInfoMain TargetID);

    public interface IDebuggerClientSession
    {
        event TargetStartedHandler TargetStarted;
        event TargetQuitHandler TargetQuit;
        event MeasureProcessingHandler MeasureProcessing;

        bool IsAttached();

        Task<AttachDebugUIResult> AttachAsync(char[] Password, DebuggerOptions Options);

        Task<bool> DetachAsync();

        Task AttachDebugTargetAsync(DebugTargetIdLight target);

        Task DetachDebugTargetAsync(DebugTargetIdLight target);

        Task<List<DbgTargetStateInfo>> AttachedTargetsStatesAsync(string areaName);

        Task InitSettingsAsync(HTTPServerInitialDebugSettingsData Data);

        Task SetAutoAttachSettingsAsync(DebugAutoAttachSettings AutoAttachSettings);

        Task ClearBreakOnNextStatementAsync();

        Task PingAsync();

        Task SetMeasureModeAsync(Guid measureMode);
    }
}
