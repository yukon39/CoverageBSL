using com.github.yukon39.CoverageBSL.debugger.debugRDBGRequestResponse;
using com.github.yukon39.CoverageBSL.httpDebug;
using NUnit.Framework;
using System;

namespace CoverageBSL.Tests.debugger.debugRDBGRequestResponse
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
            var xmlString = HTTPDebugSerializer.Serialize(response);
            Console.Write(xmlString);

            // Then
            var xmlResponse = HTTPDebugSerializer.Deserialize<MiscRDbgGetAPIVerResponse>(xmlString);
            Assert.AreEqual(response.Version, xmlResponse.Version);
        }

        [Test]
        public void TestResponseDeserialization()
        {
            // Given
            var xmlString = UtilsTest.XmlString("debugger", "debugRDBGRequestResponse", "MiscRDbgGetAPIVerResponseTest.xml");

            // When
            var request = HTTPDebugSerializer.Deserialize<MiscRDbgGetAPIVerResponse>(xmlString);

            // Then
            Assert.AreEqual(request.Version, "8.3.17");
        }
    }
}
