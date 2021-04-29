using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.yukon39.DebugClientBSL.Tests
{
    class RequestParametersTests
    {
        [Test]
        public void TestURLWithCommand()
        {
            // Given
            var rootUrl = new Uri("http://localhost:1550");
            var parameters = new RequestParameters(rootUrl, "testCommand");

            // When
            var requestUrl = parameters.RequestUrl();

            // Then
            Assert.AreEqual("http://localhost:1550/e1crdbg/rdbg?cmd=testCommand", requestUrl.ToString());
        }

        [Test]
        public void TestURLWithResource()
        {
            // Given
            var rootUrl = new Uri("http://localhost:1550");
            var parameters = new RequestParameters(rootUrl, "testCommand", "testRsc");

            // When
            var requestUrl = parameters.RequestUrl();

            // Then
            Assert.AreEqual("http://localhost:1550/e1crdbg/testRsc?cmd=testCommand", requestUrl.ToString());
        }

        [Test]
        public void TestURLWithDebugID()
        {
            // Given
            var rootUrl = new Uri("http://localhost:1550");
            var debugId = Guid.NewGuid();
            var expectedUrl = string.Format("http://localhost:1550/e1crdbg/rdbg?cmd=testCommand&dbgui={0}", debugId);
            var parameters = new RequestParameters(rootUrl, "testCommand");
            parameters.DebugID = debugId;

            // When
            var requestUrl = parameters.RequestUrl();

            // Then
            Assert.AreEqual(expectedUrl, requestUrl.ToString());
        }
    }
}
