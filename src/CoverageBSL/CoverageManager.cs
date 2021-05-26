using com.github.yukon39.CoverageBSL.Coverage;
using com.github.yukon39.CoverageBSL.Utils;
using com.github.yukon39.DebugBSL.Client;
using ScriptEngine;
using ScriptEngine.Machine.Contexts;
using System;
using System.Threading.Tasks;

namespace com.github.yukon39.CoverageBSL
{
    [ContextClass(typeName: "CoverageManager", typeAlias: "МенеджерПокрытия")]
    public class CoverageManager : AutoContext<CoverageManager>
    {
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
                Logger.Error(message, e);
                throw RuntimeExceptionFactory.NewException(message, e);
            }
        }

        private async Task<string> ApiVersionConfigureAwait() =>
           await Client.ApiVersionAsync().ConfigureAwait(false);

        [ContextMethod("TestConnection", "ПроверитьСоединение")]
        public void TestConnection()
        {
            try
            {
                TestConfigureAwait().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                var message = Locale.NStr(
                    "en = 'Test connection error';" +
                    "ru = 'Ошибка проверки соединения'");
                Logger.Error(message, e);
                throw RuntimeExceptionFactory.NewException(message, e);
            }
        }

        private async Task TestConfigureAwait() =>
           await Client.TestAsync().ConfigureAwait(false);

        [ContextMethod("NewCoverageSession", "НоваяСессияПокрытия")]
        public CoverageSession NewCoverageSession(string infobaseAlias) =>
            new CoverageSession(Client, infobaseAlias);
    }
}
