
#if NET5_0_OR_GREATER
using OneScript.Values;
using OneScript.StandardLibrary.Json;
#else
using ScriptEngine.Machine.Values;
using ScriptEngine.HostedScript.Library.Json;
#endif

namespace com.github.yukon39.CoverageBSL.AddIn.Utils
{
    public static class JSONUtils
    {
        public static void WriteValue(JSONWriter writer, string value)
#if NET5_0_OR_GREATER
            => writer.WriteValue(BslStringValue.Create(value));
#else
            => writer.WriteValue(StringValue.Create(value));
#endif
        
        public static void WriteValue(JSONWriter writer, decimal value)
#if NET5_0_OR_GREATER
            => writer.WriteValue(BslNumericValue.Create(value));
#else
            => writer.WriteValue(NumberValue.Create(value));
#endif
    }
}
