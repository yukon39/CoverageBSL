using System;
using System.Xml.Serialization;

namespace com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugRDBGRequestResponse")]
    public abstract class RDbgBaseRequest
    {
        [XmlElement(ElementName = "infoBaseAlias")]
        public string InfoBaseAlias;

        [XmlElement(ElementName = "idOfDebuggerUI")]
        public Guid IdOfDebuggerUI;
    }
}
