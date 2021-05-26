using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using com.github.yukon39.DebugBSL.debugger.debugMeasure;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using System;
using System.Threading.Tasks;

namespace com.github.yukon39.DebugBSL.Client
{

    public delegate Task TargetStartedHandler(IDebuggerClientSession sender, DebugTargetId targetId);
    public delegate Task TargetQuitHandler(IDebuggerClientSession sender, DebugTargetId targetId);
    public delegate Task MeasureProcessingHandler(IDebuggerClientSession sender, PerformanceInfoMain performanceInfo);

    public interface IDebuggerClientSession
    {
        event TargetStartedHandler TargetStarted;
        event TargetQuitHandler TargetQuit;
        event MeasureProcessingHandler MeasureProcessing;

        bool IsAttached();

        IDebuggerClientTargets GetTargetsManager();

        IDebuggerClientMeasure GetMeasureManager();

        Task<AttachDebugUIResult> AttachAsync(char[] Password, DebuggerOptions Options);

        Task<bool> DetachAsync();

        Task PingAsync();

        Task<bool> PingAsync(TimeSpan timeSpan);
    }
}
