using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using NUnit.Framework;
using System;

namespace com.github.yukon39.DebugBSL.Tests.debugger.debugRDBGRequestResponse
{
    class MiscRDbgGetAPIVerTest
    {
        [Test]
        public void TestResponseSerialization()
        {
            // Given
            var response = new MiscRDbgGetAPIVerResponse()
            {
                Version = "8.3.17"
            };

            // When
            var xmlString = DebuggerXmlSerializer.Serialize(response);
            Console.Write(xmlString);

            // Then
            var xmlResponse = DebuggerXmlSerializer.Deserialize<MiscRDbgGetAPIVerResponse>(xmlString);
            Assert.AreEqual(response.Version, xmlResponse.Version);
        }

        [Test]
        public void TestResponseDeserialization()
        {
            // Given
            var xmlString = UtilsTest.XmlString("debugger", "debugRDBGRequestResponse", "MiscRDbgGetAPIVerResponseTest.xml");

            // When
            var request = DebuggerXmlSerializer.Deserialize<MiscRDbgGetAPIVerResponse>(xmlString);

            // Then
            Assert.AreEqual(request.Version, "8.3.17");
        }
    }
}
