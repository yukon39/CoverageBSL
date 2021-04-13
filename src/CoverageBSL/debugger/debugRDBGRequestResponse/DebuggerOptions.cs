using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.debugger.debugRDBGRequestResponse
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugRDBGRequestResponse")]
    public class DebuggerOptions
    {
        [XmlElement(ElementName = "foregroundAbility")]
        public bool ForegroundAbility;
    }
}
