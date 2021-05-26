using com.github.yukon39.CoverageBSL.Coverage;
using com.github.yukon39.DebugBSL.Client;
using System.Threading.Tasks;

namespace com.github.yukon39.CoverageBSL
{
    public class CoverageManager
    {
        private readonly IDebuggerClient Client;

        public CoverageManager(string debuggerURI) =>
            Client = DebuggerClientFactory.NewInstance(debuggerURI);

        public string APIVersion() =>
            ApiVersionConfigureAwait().GetAwaiter().GetResult();

        private async Task<string> ApiVersionConfigureAwait() =>
           await Client.ApiVersionAsync().ConfigureAwait(false);

        public void TestConnection() =>
            TestConfigureAwait().GetAwaiter().GetResult();

        private async Task TestConfigureAwait() =>
           await Client.TestAsync().ConfigureAwait(false);

        public CoverageSession NewCoverageSession(string infobaseAlias) =>
            new CoverageSession(Client, infobaseAlias);
    }
}
