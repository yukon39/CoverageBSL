using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using com.github.yukon39.DebugBSL.debugger.debugDBGUICommands;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using NUnit.Framework;

namespace com.github.yukon39.DebugBSL.Tests.debugger.debugDBGUICommands
{
    class DBGUIExtCmdInfoStartedTest
    {
        [Test]
        public void TestDeserialization()
        {
            // Given
            var xmlString = UtilsTest.XmlString("debugger", "debugDBGUICommands", "DBGUIExtCmdInfoStartedTest.xml");

            // When
            var response = DebuggerXmlSerializer.Deserialize<RDBGPingDebugUIResponse>(xmlString);

            // Then
            Assert.That(response.Result.Count, Is.EqualTo(1));
            Assert.That(response.Result[0], Is.InstanceOf<DBGUIExtCmdInfoStarted>());
            var infoStarted = (DBGUIExtCmdInfoStarted)response.Result[0];

            Assert.That(infoStarted.TargetID, Is.InstanceOf<DebugTargetId>());
            Assert.That(infoStarted.TargetID.InfoBaseAlias, Is.EqualTo("DefAlias"));
        }
    }
}
