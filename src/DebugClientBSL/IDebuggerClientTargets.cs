using System.Collections.Generic;
using System.Threading.Tasks;
using com.github.yukon39.DebugBSL.debugger.debugAutoAttach;
using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;

namespace com.github.yukon39.DebugBSL.Client
{
    public interface IDebuggerClientTargets
    {
        Task AttachDebugTargetAsync(DebugTargetIdLight target);

        Task DetachDebugTargetAsync(DebugTargetIdLight target);

        Task InitSettingsAsync(HTTPServerInitialDebugSettingsData Data);

        Task SetAutoAttachSettingsAsync(DebugAutoAttachSettings AutoAttachSettings);

        Task<List<DbgTargetStateInfo>> AttachedTargetsStatesAsync(string areaName);
    }
}
