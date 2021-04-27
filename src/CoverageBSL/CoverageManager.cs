using com.github.yukon39.CoverageBSL.Coverage;
using com.github.yukon39.DebugClientBSL;
using com.github.yukon39.DebugBSL;
using ScriptEngine.Machine.Contexts;
using System;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using ScriptEngine.Machine;

namespace com.github.yukon39.CoverageBSL
{
    [ContextClass(typeName: "МенеджерПокрытия", typeAlias: "CoverageManager")]
    public class CoverageManager : AutoContext<CoverageManager>
    {
        private static readonly ILog log = CreateLogger();

        private IDebuggerClient Client;

        [ScriptConstructor(Name = "По умолчанию")]
        public static CoverageManager Constructor() => 
            new CoverageManager();

        [ContextMethod("Настроить", "Configure")]
        public string Configure(string debuggerURI)
        {
            try
            {
                var DebuggetURI = new Uri(debuggerURI);
                Client = HTTPDebugClient.Build(DebuggetURI);
                return ApiVersionConfigureAwait().GetAwaiter().GetResult();
            } catch (Exception e)
            {
                log.Error("Error configuring debugger", e);
                throw new RuntimeException("Error configuring debugger", e);
            }
        }

        [ContextMethod("NewCoverageSession", "НоваяСессияОтладки")]
        public CoverageSession NewCoverageSession(string infobaseAlias) => 
            new CoverageSession(Client, infobaseAlias);

        private async Task<string> ApiVersionConfigureAwait() => 
            await Client.ApiVersionAsync().ConfigureAwait(false);

        private static ILog CreateLogger() {
            BasicConfigurator.Configure();
            return LogManager.GetLogger(typeof(CoverageManager));
        }
    }
}
