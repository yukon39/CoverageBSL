using com.github.yukon39.CoverageBSL.AddIn.Utils;
using com.github.yukon39.CoverageBSL.Coverage;
using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using ScriptEngine;
using ScriptEngine.HostedScript.Library;
using ScriptEngine.Machine;
using ScriptEngine.Machine.Contexts;
using System;

namespace com.github.yukon39.CoverageBSL.AddIn
{
    [ContextClass(typeName: "CoverageSession", typeAlias: "СессияПокрытия")]
    public class CoverageSessionWrapper : AutoContext<CoverageSessionWrapper>, IObjectWrapper
    {
        private readonly CoverageSession session;

        public CoverageSessionWrapper(CoverageSession session) =>
            this.session = session;

        [ContextMethod("Attach", "Подключить")]
        public void Attach(string password)
        {
            AttachDebugUIResult result;
            try
            {
                result = session.Attach(password);
            }
            catch (Exception ex)
            {
                var message = Locale.NStr(
                    "en = 'Attach error';" +
                    "ru = 'Ошибка подключения'");
                Logger.Error(message, ex);
                throw RuntimeExceptionFactory.NewException(message, ex);
            }

            switch (result)
            {
                case AttachDebugUIResult.Registered:
                    return;

                case AttachDebugUIResult.CredentialsRequired:

                    throw new RuntimeException(
                        Locale.NStr("en = 'Credentials required';ru = 'Требуется указание пароля'"));

                case AttachDebugUIResult.NotRegistered:

                    throw new RuntimeException(
                        Locale.NStr("en = 'Not registered';ru = 'Не зарегистрирован'"));

                case AttachDebugUIResult.IBInDebug:

                    throw new RuntimeException(
                        Locale.NStr("en = 'IB already in debug mode';ru = 'База уже в режиме отладки'"));

                default:
                    throw new RuntimeException(
                        Locale.NStr("en = 'Unknown error';ru = 'Неизвестная ошибка'"));
            }
        }

        [ContextMethod("Detach", "Отключить")]
        public void Detach()
        {
            try
            {
                session.Detach();
            }
            catch (Exception ex)
            {
                var message = Locale.NStr(
                    "en = 'Detach error';" +
                    "ru = 'Ошибка отключения'");
                Logger.Error(message, ex);
                throw RuntimeExceptionFactory.NewException(message, ex);
            }
        }

        [ContextMethod("StartCoverageCapture", "НачатьСборПокрытия")]
        public GuidWrapper StartCoverageCapture()
        {
            try
            {
                var measureId = session.StartCoverageCapture();
                return new GuidWrapper(measureId.ToString());
            }
            catch (Exception ex)
            {
                var message = Locale.NStr(
                    "en = 'StartCoverageCapture error';" +
                    "ru = 'Ошибка начала сбора покрытия'");
                Logger.Error(message, ex);
                throw RuntimeExceptionFactory.NewException(message, ex);
            }
        }

        [ContextMethod("StopCoverageCapture", "ЗавершитьСборПокрытия")]
        public CoverageDataWrapper StopCoverageCapture()
        {
            try
            {
                var coverageData = session.StopCoverageCapture();
                return new CoverageDataWrapper(coverageData);
            }
            catch (Exception ex)
            {
                var message = Locale.NStr(
                    "en = 'StopCoverageCapture error';" +
                    "ru = 'Ошибка завершения сбора покрытия'");
                Logger.Error(message, ex);
                throw RuntimeExceptionFactory.NewException(message, ex);
            }
        }

        public object UnderlyingObject => session;
    }
}
