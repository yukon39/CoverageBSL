using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace com.github.yukon39.DebugBSL.debugger.debugArea
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugArea")]
    public class DebugAreaInfo
    {
        [XmlElement(ElementName = "name")]
        public string Name;

        [XmlElement(ElementName = "useMaskOfDSArea")]
        public bool UseMaskOfDSArea;

        [XmlElement(ElementName = "dsPairInfo")]
        public readonly List<DebugAreaDSPairInfo> DSPairInfo = new();

        [XmlElement(ElementName = "useMaskOfTargetTypes")]
        public bool UseMaskOfTargetTypes;

        [XmlElement(ElementName = "targetType")]
        public List<DebugTargetType> TargetType;

        [XmlElement(ElementName = "useMaskOfUsers")]
        public bool useMaskOfUsers;

        [XmlElement(ElementName = "userInfo")]
        public readonly List<DebugAreaUserInfo> UserInfo = new();
    }
}