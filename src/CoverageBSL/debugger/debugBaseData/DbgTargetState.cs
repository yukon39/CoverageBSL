using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.debugger.debugBaseData
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugBaseData")]
    public enum DbgTargetState
    {

        [XmlEnum(Name = "Undefined")]
        Undefined,

        [XmlEnum(Name = "NotRegistered")]
        NotRegistered,

        [XmlEnum(Name = "Registered")]
        Registered,

        [XmlEnum(Name = "WaitDebugger")]
        WaitDebugger,

        [XmlEnum(Name = "Worked")]
        Worked = 16,

        [XmlEnum(Name = "StopOnNextLine")]
        StopOnNextLine,

        [XmlEnum(Name = "Evaluating")]
        Evaluating,

        [XmlEnum(Name = "Terminating")]
        Terminating,

        [XmlEnum(Name = "Last")]
        Last
    }
}
