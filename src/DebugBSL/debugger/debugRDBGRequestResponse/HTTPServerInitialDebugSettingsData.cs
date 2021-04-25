using com.github.yukon39.DebugBSL.debugger.debugArea;
using com.github.yukon39.DebugBSL.debugger.debugAutoAttach;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugRDBGRequestResponse")]
    public class HTTPServerInitialDebugSettingsData : HTTPInitialDebugSettingsData
    {
        [XmlElement(ElementName = "debugAreaInfo")]
        public readonly List<DebugAreaInfo> DebugAreaInfo = new();

        [XmlElement(ElementName = "autoAttachSettings")]
        public DebugAutoAttachSettings AutoAttachSettings;
    }
}
