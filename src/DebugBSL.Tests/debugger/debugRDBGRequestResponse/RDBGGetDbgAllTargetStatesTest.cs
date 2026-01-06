using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using NUnit.Framework;

namespace com.github.yukon39.DebugBSL.Tests.debugger.debugRDBGRequestResponse
{
    class RDBGGetDbgAllTargetStatesTest
    {
        [Test]
        public void TestResponseDeserialization()
        {
            // Given
            var xmlString = UtilsTest.XmlString("debugger", "debugRDBGRequestResponse",
                "RDBGGetDbgAllTargetStatesResponseTest.xml");

            // When
            var request = DebuggerXmlSerializer.Deserialize<RDBGGetDbgAllTargetStatesResponse>(xmlString);

            // Then
            Assert.That(request.Item.Count, Is.EqualTo(2));

            var targetStateInfo = request.Item[0];
            Assert.That(targetStateInfo.TargetID.InfoBaseAlias, Is.EqualTo("DefAlias"));
            Assert.That(targetStateInfo.State, Is.EqualTo(DbgTargetState.Worked));
        }
    }
}
