using com.github.yukon39.CoverageBSL.AddIn.Utils;
using OneScript.StandardLibrary.Json;
using OneScript.Values;
using ScriptEngine.Machine;
using System.Linq;

namespace com.github.yukon39.CoverageBSL.AddIn.CoverageData.Serialization
{
    internal static class RawCoverageDataSetSerializer
    {
        private const string ModulesPropertyName = "Modules";
        private const string ModuleUrlPropertyName = "ModuleUrl";
        private const string LineInfoPropertyName = "LineInfo";
        private const string LineNoPropertyName = "LineNo";
        private const string HasCoveragePropertyName = "HasCoverage";

        public static void Write(CoverageDataSet dataSet, string filePath)
        {
            var writer = JSONWriter.Constructor();
            writer.OpenFile(filePath);

            try
            {
                writer.WriteStartObject();
                writer.WritePropertyName(ModulesPropertyName);
                writer.WriteStartArray();

                foreach (var module in dataSet.Modules.OrderBy(module => module.ModuleUrl))
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName(ModuleUrlPropertyName);
                    JSONUtils.WriteValue(writer, module.ModuleUrl);

                    writer.WritePropertyName(LineInfoPropertyName);
                    writer.WriteStartArray();

                    foreach (var line in module.LineInfo)
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName(LineNoPropertyName);
                        JSONUtils.WriteValue(writer, line.LineNo);
                        writer.WritePropertyName(HasCoveragePropertyName);
                        writer.WriteValue(BslBooleanValue.Create(line.HasCoverage));
                        writer.WriteEndObject();
                    }

                    writer.WriteEndArray();
                    writer.WriteEndObject();
                }

                writer.WriteEndArray();
                writer.WriteEndObject();
            }
            finally
            {
                writer.Close();
            }
        }

        public static CoverageDataSet Read(string filePath)
        {
            var reader = JSONReader.Constructor();
            reader.OpenFile(filePath);

            try
            {
                var dataSet = new CoverageDataSet();
                JsonReaderHelper.ReadExpected(reader, JSONValueTypeEnum.ObjectStart);

                while (reader.Read())
                {
                    if (reader.CurrentValueType == JSONValueTypeEnum.ObjectEnd)
                    {
                        return dataSet;
                    }

                    var propertyName = JsonReaderHelper.ReadPropertyName(reader);
                    if (propertyName == ModulesPropertyName)
                    {
                        ReadModules(reader, dataSet);
                    }
                    else
                    {
                        JsonReaderHelper.SkipValue(reader);
                    }
                }

                return dataSet;
            }
            finally
            {
                reader.Close();
            }
        }

        private static void ReadModules(JSONReader reader, CoverageDataSet dataSet)
        {
            JsonReaderHelper.ReadExpected(reader, JSONValueTypeEnum.ArrayStart);

            while (reader.Read())
            {
                if (reader.CurrentValueType == JSONValueTypeEnum.ArrayEnd)
                {
                    return;
                }

                JsonReaderHelper.EnsureValueType(reader, JSONValueTypeEnum.ObjectStart);
                ReadModule(reader, dataSet);
            }
        }

        private static void ReadModule(JSONReader reader, CoverageDataSet dataSet)
        {
            string moduleUrl = null;

            while (reader.Read())
            {
                if (reader.CurrentValueType == JSONValueTypeEnum.ObjectEnd)
                {
                    break;
                }

                var propertyName = JsonReaderHelper.ReadPropertyName(reader);
                switch (propertyName)
                {
                    case ModuleUrlPropertyName:
                        moduleUrl = JsonReaderHelper.ReadString(reader);
                        break;
                    case LineInfoPropertyName:
                        ReadLineInfo(reader, dataSet, moduleUrl);
                        break;
                    default:
                        JsonReaderHelper.SkipValue(reader);
                        break;
                }
            }
        }

        private static void ReadLineInfo(JSONReader reader, CoverageDataSet dataSet, string moduleUrl)
        {
            JsonReaderHelper.ReadExpected(reader, JSONValueTypeEnum.ArrayStart);

            if (string.IsNullOrEmpty(moduleUrl))
            {
                JsonReaderHelper.SkipValue(reader);
                return;
            }

            var module = dataSet.GetOrAdd(moduleUrl);

            while (reader.Read())
            {
                if (reader.CurrentValueType == JSONValueTypeEnum.ArrayEnd)
                {
                    return;
                }

                JsonReaderHelper.EnsureValueType(reader, JSONValueTypeEnum.ObjectStart);
                ReadLine(reader, module);
            }
        }

        private static void ReadLine(JSONReader reader, CoverageModuleData module)
        {
            int lineNo = 0;
            var hasCoverage = false;

            while (reader.Read())
            {
                if (reader.CurrentValueType == JSONValueTypeEnum.ObjectEnd)
                {
                    if (lineNo > 0)
                    {
                        if (hasCoverage)
                        {
                            module.MarkCovered(lineNo);
                        }
                        else
                        {
                            module.AddLine(lineNo);
                        }
                    }

                    return;
                }

                var propertyName = JsonReaderHelper.ReadPropertyName(reader);
                switch (propertyName)
                {
                    case LineNoPropertyName:
                        lineNo = JsonReaderHelper.ReadInt32(reader);
                        break;
                    case HasCoveragePropertyName:
                        hasCoverage = ReadBoolean(reader);
                        break;
                    default:
                        JsonReaderHelper.SkipValue(reader);
                        break;
                }
            }
        }

        private static bool ReadBoolean(JSONReader reader)
        {
            JsonReaderHelper.ReadExpected(
                reader,
                JSONValueTypeEnum.Boolean,
                JSONValueTypeEnum.String,
                JSONValueTypeEnum.Number);

            return reader.CurrentValueType switch
            {
                JSONValueTypeEnum.Boolean => reader.CurrentValue.AsBoolean(),
                JSONValueTypeEnum.String => bool.Parse(reader.CurrentValue.ToString()),
                JSONValueTypeEnum.Number => reader.CurrentValue.AsNumber() != 0,
                _ => false
            };
        }
    }
}
