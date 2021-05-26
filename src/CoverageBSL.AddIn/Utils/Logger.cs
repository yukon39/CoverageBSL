using ScriptEngine;
using ScriptEngine.Machine;
using System;

namespace com.github.yukon39.CoverageBSL.AddIn.Utils
{
    public class Logger
    {
        public static void Error(string message)
        {
            SystemLogger.Write(string.Format("[ERROR] {0}", message));
        }

        public static void Error(string message, Exception e)
        {
            string exceptionMessage;
            if (e is RuntimeException re)
            {
                exceptionMessage = re.Message;
            } 
            else
            {
                exceptionMessage = e.ToString();
            }

            SystemLogger.Write(string.Format("[ERROR] {0}: {1}", message, exceptionMessage));
        }
    }
}
