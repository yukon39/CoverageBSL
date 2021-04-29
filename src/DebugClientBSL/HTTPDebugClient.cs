using com.github.yukon39.DebugBSL;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using System;
using System.Threading.Tasks;

namespace com.github.yukon39.DebugClientBSL
{
    public class HTTPDebugClient : IDebuggerClient
    {
        private readonly HttpClientExecutor Executor;

        private HTTPDebugClient(HttpClientExecutor executor)
        {
            Executor = executor;
        }

        public async Task TestAsync()
        {
            var requestParameters = new RequestParameters("test", "rdbgTest");
            var request = new RDBGTestRequest();

            await Executor.ExecuteAsync<RDBGEmptyResponse>(request, requestParameters);
        }

        public async Task<string> ApiVersionAsync()
        {
            var requestParameters = new RequestParameters("getRDbgAPIVer");
            var request = new MiscRDbgGetAPIVerRequest();

            var response = await Executor.ExecuteAsync<MiscRDbgGetAPIVerResponse>(request, requestParameters);
            var version = response.Version;

            return version;
        }

        public IDebuggerClientSession CreateSession(string infobaseAlias) =>
            new HTTPDebugSession(Executor, infobaseAlias);

        public static IDebuggerClient Create(HttpClientExecutor executor) => new HTTPDebugClient(executor);
    }
}
