using com.github.yukon39.CoverageBSL.Coverage;
using com.github.yukon39.DebugClientBSL;
using com.github.yukon39.DebugBSL;
using ScriptEngine.Machine.Contexts;
using System;
using System.Threading.Tasks;

namespace com.github.yukon39.CoverageBSL
{
    [ContextClass(typeName: "МенеджерПокрытия", typeAlias: "CoverageManager")]
    public class CoverageManager : AutoContext<CoverageManager>
    {
        private IDebuggerClient Client;

        [ScriptConstructor(Name = "По умолчанию")]
        public static CoverageManager Constructor() => new CoverageManager();

        [ContextMethod("Настроить", "Configure")]
        public string Configure(string debuggerURI)
        {
            var DebuggetURI = new Uri(debuggerURI);
            Client = HTTPDebugClient.Build(DebuggetURI);
            return ApiVersionAsync().GetAwaiter().GetResult();
        }

        [ContextMethod("NewCoverageSession", "НоваяСессияОтладки")]
        public CoverageSession NewCoverageSession(string infobaseAlias) => new CoverageSession(Client, infobaseAlias);

        private async Task<string> ApiVersionAsync() => await Client.ApiVersionAsync().ConfigureAwait(false);
    }
}
