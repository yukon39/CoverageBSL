﻿using System.Xml.Serialization;

namespace com.github.yukon39.DebugBSL.debugger.debugRTEFilter
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugRTEFilter")]
    public class RteFilterItem
    {
        [XmlElement(ElementName = "include")]
        public bool Include { get; set; }

        [XmlElement(ElementName = "str")]
        public string Str { get; set; }
    }
}
