
#if NET48
using ScriptEngine.Machine.Values;
using ScriptEngine.HostedScript.Library.Json;
#else
using OneScript.Values;
using OneScript.StandardLibrary.Json;
#endif

namespace com.github.yukon39.CoverageBSL.AddIn.Utils
{
    public static class JSONUtils
    {
        public static void WriteValue(JSONWriter writer, string value)
#if NET48
            => writer.WriteValue(StringValue.Create(value)); 
#else
            => writer.WriteValue(BslStringValue.Create(value));
#endif
        
        public static void WriteValue(JSONWriter writer, decimal value)
#if NET48
            => writer.WriteValue(NumberValue.Create(value));
#else
            => writer.WriteValue(BslNumericValue.Create(value));
#endif
    }
}
