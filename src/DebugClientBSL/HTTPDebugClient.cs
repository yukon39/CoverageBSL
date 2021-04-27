using com.github.yukon39.DebugBSL;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using System;
using System.Threading.Tasks;

namespace com.github.yukon39.DebugClientBSL
{
    public class HTTPDebugClient : IDebuggerClient
    {
        private readonly Uri DebugServerURL;

        private HTTPDebugClient(Uri debugServerURL)
        {
            DebugServerURL = debugServerURL;
        }

        public async Task TestAsync()
        {
            var requestParameters = new RequestParameters(DebugServerURL, "test", "rdbgTest");
            var request = new RDBGTestRequest();

            await HttpClientExecutor.ExecuteAsync<RDBGEmptyResponse>(request, requestParameters);
        }

        public async Task<string> ApiVersionAsync()
        {
            var requestParameters = new RequestParameters(DebugServerURL, "getRDbgAPIVer");
            var request = new MiscRDbgGetAPIVerRequest();

            var response = await HttpClientExecutor.ExecuteAsync<MiscRDbgGetAPIVerResponse>(request, requestParameters);
            var version = response.Version;

            return version;
        }

        public IDebuggerClientSession CreateSession(string infobaseAlias) => new HTTPDebugSession(DebugServerURL, infobaseAlias);

        public static IDebuggerClient Build(Uri debugServerURL) => new HTTPDebugClient(debugServerURL);
    }
}
