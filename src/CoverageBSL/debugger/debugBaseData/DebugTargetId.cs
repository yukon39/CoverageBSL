﻿using System;
using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.debugger.debugBaseData
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugBaseData")]
    public class DebugTargetId : DebugTargetIdLight
    {
        [XmlElement(ElementName = "seanceId")]
        public Guid SeanceId;

        [XmlElement(ElementName = "seanceNo")]
        public int SeanceNo;

        [XmlElement(ElementName = "infoBaseInstanceID")]
        public Guid InfoBaseInstanceID;

        [XmlElement(ElementName = "infoBaseAlias")]
        public string InfoBaseAlias;

        [XmlElement(ElementName = "isServerInfoBase")]
        public IsServerInfoBase IsServerInfoBase;

        [XmlElement(ElementName = "userName")]
        public string UserName;

        [XmlElement(ElementName = "configVersion")]
        public string ConfigVersion;

        [XmlElement(ElementName = "targetType")]
        public DebugTargetType TargetType;
    }
}
