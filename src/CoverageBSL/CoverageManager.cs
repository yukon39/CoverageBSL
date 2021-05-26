using com.github.yukon39.CoverageBSL.Coverage;
using com.github.yukon39.DebugBSL.Client;
using log4net;
using log4net.Config;
using ScriptEngine;
using ScriptEngine.Machine;
using ScriptEngine.Machine.Contexts;
using System;
using System.Threading.Tasks;

namespace com.github.yukon39.CoverageBSL
{
    [ContextClass(typeName: "CoverageManager", typeAlias: "МенеджерПокрытия")]
    public class CoverageManager : AutoContext<CoverageManager>
    {
        private static readonly ILog log = CreateLogger();

        private readonly IDebuggerClient Client;

        [ScriptConstructor]
        public static CoverageManager Constructor(string debuggerURI) =>
            new CoverageManager(debuggerURI);

        private CoverageManager(string debuggerURI) =>
            Client = DebuggerClientFactory.NewInstance(debuggerURI);

        [ContextMethod("APIVersion", "ВерсияAPI")]
        public string APIVersion()
        {
            try
            {
                return ApiVersionConfigureAwait().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                var message = Locale.NStr(
                    "en = 'Get API version error';" +
                    "ru = 'Ошибка получения версии API'");
                log.Error(message, e);
                throw new RuntimeException("Error configuring debugger", e);
            }
        }

        [ContextMethod("NewCoverageSession", "НоваяСессияОтладки")]
        public CoverageSession NewCoverageSession(string infobaseAlias) =>
            new CoverageSession(Client, infobaseAlias);

        private async Task<string> ApiVersionConfigureAwait() =>
            await Client.ApiVersionAsync().ConfigureAwait(false);

        private static ILog CreateLogger()
        {
            BasicConfigurator.Configure();
            return LogManager.GetLogger(typeof(CoverageManager));
        }
    }
}
