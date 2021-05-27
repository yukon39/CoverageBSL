using ScriptEngine.Machine;
using System;

namespace com.github.yukon39.CoverageBSL.AddIn.Utils
{
    public class RuntimeExceptionFactory
    {
        public static RuntimeException NewException(string message, Exception e)
        {
            var exceptionMessage = string.Format("{0}: {1}", message, ExceptionMessage(e));
            return new RuntimeException(exceptionMessage, e);
        }

        private static string ExceptionMessage(Exception e)
        {
            if (e.InnerException is Exception inner)
            {
                return string.Format("{0} ---> {1}", e.Message, ExceptionMessage(inner));
            }
            else
            {
                return e.Message;
            }
        }

    }
}
