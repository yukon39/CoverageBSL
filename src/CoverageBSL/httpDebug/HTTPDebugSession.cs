﻿using com.github.yukon39.CoverageBSL.debugger;
using com.github.yukon39.CoverageBSL.debugger.debugAutoAttach;
using com.github.yukon39.CoverageBSL.debugger.debugBaseData;
using com.github.yukon39.CoverageBSL.debugger.debugDBGUICommands;
using com.github.yukon39.CoverageBSL.debugger.debugRDBGRequestResponse;
using System;
using System.Collections.Generic;

namespace com.github.yukon39.CoverageBSL.httpDebug
{
    class HTTPDebugSession : IDebuggerSession
    {
        public readonly string InfobaseAlias;
        public readonly Guid DebugSession;
        private readonly HTTPDebugClient Client;
        private bool Attached = false;

        public HTTPDebugSession(HTTPDebugClient client, string infobaseAlias)
        {
            Client = client;
            InfobaseAlias = infobaseAlias;
            DebugSession = Guid.NewGuid();
        }

        public bool IsAttached() => Attached;

        public AttachDebugUIResult Attach(char[] Password, DebuggerOptions Options)
        {
            var RequestParameters = new RequestParameters
            {
                Command = "attachDebugUI"
            };

            var Request = new RDBGAttachDebugUIRequest
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias
            };

            //if (password.length > 0)
            //{
            //    var credentials = StringUtils.toByteArray(password);
            //    request.setCredentials(credentials);
            //}
            Request.Options = Options;

            var Response = Client.Execute<RDBGAttachDebugUIResponse>(Request, RequestParameters);
            var Result = Response.Result;

            //Logger.LogDebug("Debug attach result is {result}", Result);

            Attached = Result.Equals(AttachDebugUIResult.Registered);

            return Result;
        }

        public bool Detach()
        {
            var RequestParameters = new RequestParameters
            {
                Command = "detachDebugUI"
            };


            var Request = new RDBGDetachDebugUIRequest
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias
            };

            var Response = Client.Execute<RDBGDetachDebugUIResponse>(Request, RequestParameters);
            var Result = Response.Result;

            //Logger.LogDebug("Debug detach result is {result}", Result);

            return Result;
        }

        public void AttachDebugTarget(DebugTargetIdLight target) => AttachDetachDebugTargets(new() { target }, true);

        public void DetachDebugTarget(DebugTargetIdLight target) => AttachDetachDebugTargets(new() { target }, false);

        private void AttachDetachDebugTargets(List<DebugTargetIdLight> targets, bool Attach)
        {
            var RequestParameters = new RequestParameters
            {
                Command = "attachDetachDbgTargets"
            };

            var Request = new RDBGAttachDetachDebugTargetsRequest
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias,
                Attach = Attach,
            };
            Request.ID.AddRange(targets);

            Client.Execute<RDBGAttachDetachDebugTargetsResponse>(Request, RequestParameters);
        }

        public void InitSettings(HTTPServerInitialDebugSettingsData Data)
        {
            var RequestParameters = new RequestParameters
            {
                Command = "initSettings"
            };


            var Request = new RDBGSetInitialDebugSettingsRequest
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias,
                data = Data
            };

            Client.Execute<RDBGSetInitialDebugSettingsResponse>(Request, RequestParameters);

            //Logger.LogDebug("InitSettings successful");
        }

        public void SetAutoAttachSettings(DebugAutoAttachSettings AutoAttachSettings)
        {
            var RequestParameters = new RequestParameters
            {
                Command = "setAutoAttachSettings"
            };

            var Request = new RDBGSetAutoAttachSettingsRequest
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias,

                autoAttachSettings = AutoAttachSettings
            };

            Client.Execute<RDBGSetAutoAttachSettingsResponse>(Request, RequestParameters);

            //Logger.LogDebug("SetAutoAttachSettings successful");
        }

        public void ClearBreakOnNextStatement()
        {
            var RequestParameters = new RequestParameters
            {
                Command = "clearBreakOnNextStatement"
            };

            var Request = new RDBGClearBreakOnNextStatementRequest
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias
            };

            Client.Execute<RDBGClearBreakOnNextStatementResponse>(Request, RequestParameters);

            //Logger.LogDebug("ClearBreakOnNextStatement successful");
        }

        public List<DBGUIExtCmdInfoBase> Ping()
        {
            var RequestParameters = new RequestParameters
            {
                Command = "pingDebugUIParams",
                DebugID = DebugSession
            };

            var Request = new RDBGPingDebugUIRequest();

            var Response = Client.Execute<RDBGPingDebugUIResponse>(Request, RequestParameters);
            var Result = Response.Result;

            //Logger.LogDebug("Ping result size is {size}", Result.Count);

            return Result;
        }
    }
}
