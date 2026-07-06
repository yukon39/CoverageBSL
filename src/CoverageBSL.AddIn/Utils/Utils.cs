using OneScript.Values;
using OneScript.StandardLibrary.Json;

namespace com.github.yukon39.CoverageBSL.AddIn.Utils
{
    public static class JSONUtils
    {
        public static void WriteValue(JSONWriter writer, string value)
            => writer.WriteValue(BslStringValue.Create(value));
        
        public static void WriteValue(JSONWriter writer, decimal value)
            => writer.WriteValue(BslNumericValue.Create(value));
    }
}
