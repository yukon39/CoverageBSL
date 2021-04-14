using com.github.yukon39.CoverageBSL.debugger.debugRDBGRequestResponse;
using com.github.yukon39.CoverageBSL.httpDebug;
using NUnit.Framework;
using System;
using System.IO;

namespace CoverageBSL.Tests.debugger.debugRDBGRequestResponse
{
    class MiscRDbgGetAPIVerTest
    {
        [Test]
        public void TestResponseSerialization()
        {
            // Given
            var Response = new MiscRDbgGetAPIVerResponse()
            {
                Version = "8.3.17"
            };

            // When
            var XmlString = HTTPDebugSerializer.Serialize(Response);
            Console.Write(XmlString);

            // Then
            var Object = HTTPDebugSerializer.Deserialize<MiscRDbgGetAPIVerResponse>(XmlString);
            Assert.AreEqual(Response.Version, Object.Version);
        }

        [Test]
        public void TestResponseDeserialization()
        {
            // Given
            var XmlFile = Path.Join(TestContext.CurrentContext.TestDirectory,
                    "debugger", "debugRDBGRequestResponse", "MiscRDbgGetAPIVerResponseTest.xml");
            var XmlString = File.ReadAllText(XmlFile);

            // When
            var Request = HTTPDebugSerializer.Deserialize<MiscRDbgGetAPIVerResponse>(XmlString);

            // Then
            Assert.AreEqual(Request.Version, "8.3.17");
        }
    }
}
