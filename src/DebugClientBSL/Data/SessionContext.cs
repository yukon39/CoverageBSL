using System;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;

namespace com.github.yukon39.DebugBSL.Client.Data
{
    public class SessionContext
    {
        private readonly string InfobaseAlias;
        public readonly Guid DebugSession;

        private SessionContext(string infobaseAlias, Guid debugSession)
        {
            InfobaseAlias = infobaseAlias;
            DebugSession = debugSession;
        }

        public static SessionContext NewInstance(string infobaseAlias, Guid debugSession) =>
            new SessionContext(infobaseAlias, debugSession);

        public static SessionContext NewInstance(string infobaseAlias) =>
            new SessionContext(infobaseAlias, Guid.NewGuid());

        public T NewSessionRequest<T>() where T : RDbgBaseRequest
        {
            var request = Activator.CreateInstance<T>();
            request.IdOfDebuggerUI = DebugSession;
            request.InfoBaseAlias = InfobaseAlias;

            return request;
        }
    }
}
