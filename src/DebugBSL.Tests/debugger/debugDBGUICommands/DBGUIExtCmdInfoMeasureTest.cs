using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using com.github.yukon39.DebugBSL.debugger.debugDBGUICommands;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using NUnit.Framework;

namespace com.github.yukon39.DebugBSL.Tests.debugger.debugDBGUICommands
{
    class DBGUIExtCmdInfoMeasureTest
    {
        [Test]
        public void TestDeserialization()
        {
            // Given
            var xmlString = UtilsTest.XmlString("debugger", "debugDBGUICommands", "DBGUIExtCmdInfoMeasureTest.xml");

            // When
            var response = DebuggerXmlSerializer.Deserialize<RDBGPingDebugUIResponse>(xmlString);

            // Then
            Assert.That(response.Result.Count, Is.EqualTo(1));
            Assert.That(response.Result[0], Is.InstanceOf<DBGUIExtCmdInfoMeasure>());
            var infoMeasure = response.Result[0] as DBGUIExtCmdInfoMeasure;

            Assert.That(infoMeasure.TargetID, Is.InstanceOf<DebugTargetId>());
            Assert.That(infoMeasure.TargetID.InfoBaseAlias, Is.EqualTo("DefAlias"));
            Assert.That(infoMeasure.Measure.TargetID.InfoBaseAlias, Is.EqualTo("DefAlias"));
            Assert.That(infoMeasure.Measure.TotalDurability, Is.GreaterThan(0)); 
            Assert.That(infoMeasure.Measure.ModuleData.Count, Is.GreaterThan(0));

            var moduleData = infoMeasure.Measure.ModuleData[0];
            Assert.That(moduleData.ModuleID, Is.InstanceOf<BSLModuleIdInternal>());
            Assert.That(moduleData.LineInfo.Count, Is.GreaterThan(0));

            var lineInfo = moduleData.LineInfo[0];
            Assert.That(lineInfo.LineNo, Is.GreaterThan(0));
        }
    }
}
