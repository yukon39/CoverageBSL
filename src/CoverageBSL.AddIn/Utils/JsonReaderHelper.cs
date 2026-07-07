using OneScript.StandardLibrary.Json;
using ScriptEngine.Machine;

namespace com.github.yukon39.CoverageBSL.AddIn.Utils
{
    internal static class JsonReaderHelper
    {
        public static string ReadPropertyName(JSONReader reader)
        {
            EnsureValueType(reader, JSONValueTypeEnum.PropertyName);
            return reader.CurrentValue.ToString();
        }

        public static string ReadString(JSONReader reader)
        {
            ReadExpected(reader, JSONValueTypeEnum.String, JSONValueTypeEnum.Null);
            return reader.CurrentValueType == JSONValueTypeEnum.Null
                ? string.Empty
                : reader.CurrentValue.ToString();
        }

        public static int ReadInt32(JSONReader reader)
            => decimal.ToInt32(ReadNumber(reader));

        public static long ReadInt64(JSONReader reader)
            => decimal.ToInt64(ReadNumber(reader));

        public static decimal ReadNumber(JSONReader reader)
        {
            ReadExpected(reader, JSONValueTypeEnum.Number);
            return reader.CurrentValue.AsNumber();
        }

        public static void SkipValue(JSONReader reader)
        {
            ReadExpected(
                reader,
                JSONValueTypeEnum.ObjectStart,
                JSONValueTypeEnum.ArrayStart,
                JSONValueTypeEnum.String,
                JSONValueTypeEnum.Number,
                JSONValueTypeEnum.Boolean,
                JSONValueTypeEnum.Null);

            if (reader.CurrentValueType == JSONValueTypeEnum.ObjectStart ||
                reader.CurrentValueType == JSONValueTypeEnum.ArrayStart)
            {
                reader.Skip();
            }
        }

        public static void ReadExpected(JSONReader reader, params JSONValueTypeEnum[] expectedValueTypes)
        {
            if (!reader.Read())
            {
                throw new JSONReaderException("Unexpected end of JSON");
            }

            EnsureValueType(reader, expectedValueTypes);
        }

        public static void EnsureValueType(JSONReader reader, params JSONValueTypeEnum[] expectedValueTypes)
        {
            foreach (var valueType in expectedValueTypes)
            {
                if (reader.CurrentValueType == valueType)
                {
                    return;
                }
            }

            throw new JSONReaderException(
                $"Unexpected value type {reader.CurrentValueType}. Expected: {string.Join(", ", expectedValueTypes)}");
        }
    }
}
