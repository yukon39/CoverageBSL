using System.Threading.Tasks;
using com.github.yukon39.DebugBSL.Client.Data;
using com.github.yukon39.DebugBSL.Client.Internal;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;

namespace com.github.yukon39.DebugBSL.Client.Impl
{
    public class DebuggerClient : IDebuggerClient
    {
        private readonly DebuggerClientExecutor Executor;

        private DebuggerClient(DebuggerClientExecutor executor) => Executor = executor;

        public static DebuggerClient NewInstance(DebuggerClientExecutor executor) =>
            new DebuggerClient(executor);

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
            DebuggerClientSession.NewInstance(Executor, infobaseAlias);
    }
}
