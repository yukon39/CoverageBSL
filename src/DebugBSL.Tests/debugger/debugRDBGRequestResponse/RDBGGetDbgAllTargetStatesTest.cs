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
            Assert.AreEqual(2, request.Item.Count);

            var targetStateInfo = request.Item[0];
            Assert.AreEqual("DefAlias", targetStateInfo.TargetID.InfoBaseAlias);
            Assert.AreEqual(DbgTargetState.Worked, targetStateInfo.State);
        }
    }
}
