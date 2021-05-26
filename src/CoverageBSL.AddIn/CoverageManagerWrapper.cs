using com.github.yukon39.CoverageBSL.AddIn.Utils;
using ScriptEngine;
using ScriptEngine.Machine.Contexts;
using System;

namespace com.github.yukon39.CoverageBSL.AddIn
{
    [ContextClass(typeName: "CoverageManager", typeAlias: "МенеджерПокрытия")]
    public class CoverageManagerWrapper : AutoContext<CoverageManagerWrapper>, IObjectWrapper
    {
        private readonly CoverageManager manager;

        [ScriptConstructor]
        public static CoverageManagerWrapper ScriptConstructor(string debuggerURI) =>
            new CoverageManagerWrapper(debuggerURI);

        private CoverageManagerWrapper(string debuggerURI) =>
            manager = new CoverageManager(debuggerURI);

        [ContextMethod("APIVersion", "ВерсияAPI")]
        public string APIVersion()
        {
            try
            {
                return manager.APIVersion();
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

        [ContextMethod("TestConnection", "ПроверитьСоединение")]
        public void TestConnection()
        {
            try
            {
                manager.TestConnection();
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

        [ContextMethod("NewCoverageSession", "НоваяСессияПокрытия")]
        public CoverageSessionWrapper NewCoverageSession(string infobaseAlias)
        {
            var session = manager.NewCoverageSession(infobaseAlias);
            return new CoverageSessionWrapper(session);
        }

        public object UnderlyingObject => manager;
    }
}
