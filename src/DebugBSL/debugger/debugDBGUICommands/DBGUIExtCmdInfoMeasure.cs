using com.github.yukon39.DebugBSL.debugger.debugMeasure;
using System.Xml.Serialization;

namespace com.github.yukon39.DebugBSL.debugger.debugDBGUICommands
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugDBGUICommands")]
    public class DBGUIExtCmdInfoMeasure : DBGUIExtCmdInfoBase
    {

        [XmlElement(ElementName = "measureStr")]
        public char[] MeasureStr;

        [XmlElement(ElementName = "measure")]
        public PerformanceInfoMain Measure;

        public DBGUIExtCmdInfoMeasure() : base(DBGUIExtCmds.MeasureResultProcessing)
        {
        }
    }
}
