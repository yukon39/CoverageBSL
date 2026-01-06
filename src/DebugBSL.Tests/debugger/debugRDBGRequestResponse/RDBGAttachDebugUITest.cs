using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using NUnit.Framework;
using System;

namespace com.github.yukon39.DebugBSL.Tests.debugger.debugRDBGRequestResponse
{
    class RDBGAttachDebugUITest
    {

        [Test]
        public void TestRequestSerialization()
        {
            // Given
            var request = new RDBGAttachDebugUIRequest
            {
                InfoBaseAlias = "DefAlias",
                IdOfDebuggerUI = Guid.Parse("dbe7b1e9-9786-4a25-8da8-304684fa2ce3"),
                Options = new DebuggerOptions
                {
                    ForegroundAbility = true
                }
            };

            // When
            var xmlString = DebuggerXmlSerializer.Serialize(request);
            Console.Write(xmlString);

            // Then
            var xmlRequest = DebuggerXmlSerializer.Deserialize<RDBGAttachDebugUIRequest>(xmlString);
            Assert.That(xmlRequest.InfoBaseAlias, Is.EqualTo(request.InfoBaseAlias));
            Assert.That(xmlRequest.IdOfDebuggerUI, Is.EqualTo(request.IdOfDebuggerUI));
            Assert.That(xmlRequest.Options.ForegroundAbility, Is.EqualTo(request.Options.ForegroundAbility));
        }

        [Test]
        public void TestRequestDeserialization()
        {
            // Given
            var xmlString = UtilsTest.XmlString("debugger", "debugRDBGRequestResponse", "RDBGAttachDebugUIRequestTest.xml");

            // When
            var request = DebuggerXmlSerializer.Deserialize<RDBGAttachDebugUIRequest>(xmlString);

            // Then
            Assert.That(request.InfoBaseAlias, Is.EqualTo("DefAlias"));
            Assert.That(request.IdOfDebuggerUI, Is.EqualTo(Guid.Parse("dbe7b1e9-9786-4a25-8da8-304684fa2ce3")));
            Assert.That(request.Options.ForegroundAbility, Is.True);
        }

        [Test]
        public void TestResponseSerialization()
        {
            // Given
            var response = new RDBGAttachDebugUIResponse()
            {
                Result = AttachDebugUIResult.Registered
            };

            // When
            var xmlString = DebuggerXmlSerializer.Serialize(response);
            Console.Write(xmlString);

            // Then
            var xmlResponse = DebuggerXmlSerializer.Deserialize<RDBGAttachDebugUIResponse>(xmlString);
            Assert.That(xmlResponse.Result, Is.EqualTo(response.Result));
        }

        [Test]
        public void TestResponseDeserialization()
        {
            // Given
            var xmlString = UtilsTest.XmlString("debugger", "debugRDBGRequestResponse",
                    "RDBGAttachDebugUIResponseTest.xml");

            // When
            var request = DebuggerXmlSerializer.Deserialize<RDBGAttachDebugUIResponse>(xmlString);

            // Then
            Assert.That(request.Result, Is.EqualTo(AttachDebugUIResult.Registered));
        }
    }
}
