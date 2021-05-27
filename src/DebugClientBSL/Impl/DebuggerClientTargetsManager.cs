using com.github.yukon39.DebugBSL.Client.Data;
using com.github.yukon39.DebugBSL.Client.Internal;
using com.github.yukon39.DebugBSL.debugger.debugAutoAttach;
using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace com.github.yukon39.DebugBSL.Client.Impl
{
    public class DebuggerClientTargetsManager : DebuggerClientEntityManager, IDebuggerClientTargets
    {
        public DebuggerClientTargetsManager(DebuggerClientExecutor executor, SessionContext context)
            : base(executor, context) { }

        public override void SubscribeSessionEvents(IDebuggerClientSession session)
        {
            session.TargetStarted += TargetStartedHandler;
            session.TargetQuit += TargetQuitHandler;
        }

        public async Task TargetStartedHandler(IDebuggerClientSession sender, DebugTargetId targetID) =>
            await AttachDebugTargetAsync(targetID.TargetIdLight);

        public async Task TargetQuitHandler(IDebuggerClientSession sender, DebugTargetId targetID) =>
            await DetachDebugTargetAsync(targetID.TargetIdLight);

        public async Task AttachDebugTargetAsync(DebugTargetIdLight target) =>
           await AttachDetachDebugTargetsAsync(new List<DebugTargetIdLight>() { target }, true);

        public async Task DetachDebugTargetAsync(DebugTargetIdLight target) =>
            await AttachDetachDebugTargetsAsync(new List<DebugTargetIdLight>() { target }, false);

        private async Task AttachDetachDebugTargetsAsync(List<DebugTargetIdLight> targets, bool Attach)
        {
            var requestParameters = new RequestParameters("attachDetachDbgTargets");

            var request = Context.NewSessionRequest<RDBGAttachDetachDebugTargetsRequest>();
            request.Attach = Attach;
            request.ID.AddRange(targets);

            await Executor.ExecuteAsync<RDBGEmptyResponse>(request, requestParameters);
        }

        public async Task InitSettingsAsync(HTTPServerInitialDebugSettingsData data)
        {
            var requestParameters = new RequestParameters("initSettings");

            var request = Context.NewSessionRequest<RDBGSetInitialDebugSettingsRequest>();
            request.Data = data;

            await Executor.ExecuteAsync<RDBGEmptyResponse>(request, requestParameters);

            //Logger.LogDebug("InitSettings successful");
        }

        public async Task SetAutoAttachSettingsAsync(DebugAutoAttachSettings autoAttachSettings)
        {
            var requestParameters = new RequestParameters("setAutoAttachSettings");

            var request = Context.NewSessionRequest<RDBGSetAutoAttachSettingsRequest>();
            request.AutoAttachSettings = autoAttachSettings;

            await Executor.ExecuteAsync<RDBGEmptyResponse>(request, requestParameters);

            //Logger.LogDebug("SetAutoAttachSettings successful");
        }

        public async Task<List<DbgTargetStateInfo>> AttachedTargetsStatesAsync(string areaName)
        {
            var requestParameters = new RequestParameters("getDbgAllTargetStates");

            var request = Context.NewSessionRequest<RDBGGetDbgAllTargetStatesRequest>();

            if (!string.IsNullOrEmpty(areaName))
            {
                request.DebugAreaName = areaName;
            }

            var response = await Executor.ExecuteAsync<RDBGGetDbgAllTargetStatesResponse>(request, requestParameters);
            var items = response.Item;

            return items;
        }
    }
}
