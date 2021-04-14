using System.IO;
using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.httpDebug
{
    public static class HTTPDebugSerializer
    {
        public static string Serialize(object o)
        {
            var namespaces = Namespaces();

            var T = o.GetType();
            var Serializer = new XmlSerializer(T);

            var Writer = new StringWriter();
            Serializer.Serialize(Writer, o, namespaces);
            return Writer.ToString();
        }

          public static T Deserialize<T>(string xmlString)
        {
            var Serializer = new XmlSerializer(typeof(T));
            var Reader = new StringReader(xmlString);
            return (T)Serializer.Deserialize(Reader);
        }

        public static XmlSerializerNamespaces Namespaces()
        {
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            namespaces.Add("cfg", "http://v8.1c.ru/8.1/data/enterprise/current-config");
            namespaces.Add("v8", "http://v8.1c.ru/8.1/data/core");
            namespaces.Add("debugAutoAttach", "http://v8.1c.ru/8.3/debugger/debugAutoAttach");
            namespaces.Add("debugRDBGRequestResponse", "http://v8.1c.ru/8.3/debugger/debugRDBGRequestResponse");
            namespaces.Add("debugRTEFilter", "http://v8.1c.ru/8.3/debugger/debugRTEFilter");

            return namespaces;
        }
    }
}
