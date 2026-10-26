using OneScript.StandardLibrary.Xml;
using ScriptEngine.Machine;
using System.Linq;

namespace com.github.yukon39.CoverageBSL.AddIn.CoverageData.Serialization
{
    internal static class GenericCoverageDataSetSerializer
    {
        public static void Write(CoverageDataSet dataSet, string filePath)
        {
            var writer = new XmlWriterImpl();
            writer.OpenFile(filePath);

            try
            {
                writer.WriteXMLDeclaration();
                writer.WriteStartElement("coverage");
                writer.WriteAttribute("version", "1");

                foreach (var module in dataSet.Modules.OrderBy(module => module.ModuleUrl))
                {
                    writer.WriteStartElement("file");
                    writer.WriteAttribute("path", module.ModuleUrl);

                    foreach (var line in module.LineInfo)
                    {
                        writer.WriteStartElement("lineToCover");
                        writer.WriteAttribute("lineNumber", line.LineNo.ToString());
                        writer.WriteAttribute("covered", ToXmlBoolean(line.HasCoverage));
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
            finally
            {
                writer.Close();
            }
        }

        public static CoverageDataSet Read(string filePath)
        {
            var reader = new XmlReaderImpl();
            reader.OpenFile(filePath);

            try
            {
                var dataSet = new CoverageDataSet();
                reader.MoveToContent();

                while (reader.Read() && reader.LocalName == "file")
                {
                    var moduleUrl = reader.GetAttribute(ValueFactory.Create("path")).ToString();
                    var module = dataSet.GetOrAdd(moduleUrl);

                    while (reader.Read() && reader.LocalName == "lineToCover")
                    {
                        var lineNumber = int.Parse(
                            reader.GetAttribute(ValueFactory.Create("lineNumber")).ToString());
                        var covered = ParseXmlBoolean(
                            reader.GetAttribute(ValueFactory.Create("covered")).ToString());

                        if (covered)
                        {
                            module.MarkCovered(lineNumber);
                        }
                        else
                        {
                            module.AddLine(lineNumber);
                        }
                    }
                }

                return dataSet;
            }
            finally
            {
                reader.Dispose();
            }
        }

        private static string ToXmlBoolean(bool value)
            => value ? "true" : "false";

        private static bool ParseXmlBoolean(string value)
            => string.Equals(value, "true", System.StringComparison.OrdinalIgnoreCase)
               || value == "1";
    }
}
