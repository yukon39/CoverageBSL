using com.github.yukon39.DebugBSL.Client;
using System.Threading.Tasks;

namespace com.github.yukon39.CoverageBSL.Impl
{
    public class CoverageManager : ICoverageManager
    {
        private readonly IDebuggerClient Client;

        public CoverageManager(string debuggerURI) =>
            Client = DebuggerClientFactory.NewInstance(debuggerURI);

        public string APIVersion() =>
            ApiVersionConfigureAwait().GetAwaiter().GetResult();

        private async Task<string> ApiVersionConfigureAwait() =>
           await ApiVersionAsync().ConfigureAwait(false);

        public Task<string> ApiVersionAsync() =>
            Client.ApiVersionAsync();

        public void TestConnection() =>
            TestConfigureAwait().GetAwaiter().GetResult();

        private async Task TestConfigureAwait() =>
           await TestConnectionAsync().ConfigureAwait(false);

        public Task TestConnectionAsync() =>
            Client.TestAsync();

        public ICoverageSession NewCoverageSession(string infobaseAlias) =>
            new CoverageSession(Client, infobaseAlias);
    }
}
