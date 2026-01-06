using com.github.yukon39.DebugBSL.Client.Tests;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;

namespace com.github.yukon39.DebugBSL.Client.Impl.Tests
{
    class DebuggerClientTest
    {
        [Test]
        public void TestTestAsync()
        {
            // given
            var response = new RDBGEmptyResponse();

            var messageHandler = new MockHttpMessageHandler();
            messageHandler.Enqueue(HttpStatusCode.OK, response);

            var client = Create(messageHandler);

            // when
            Assert.DoesNotThrowAsync(async () => await client.TestAsync());

            // then
            var request = messageHandler.Dequeue();
            Assert.That(request.RequestUri.ToString(), Is.EqualTo("http://localhost/e1crdbg/rdbgTest?cmd=test"));
        }

        [Test]
        public async Task TestApiVersionAsync()
        {
            // given
            var response = new MiscRDbgGetAPIVerResponse()
            {
                Version = "1.2.3.4"
            };

            var messageHandler = new MockHttpMessageHandler();
            messageHandler.Enqueue(HttpStatusCode.OK, response);

            var client = Create(messageHandler);

            // when
            var version = await client.ApiVersionAsync();

            // then
            Assert.That(version, Is.EqualTo("1.2.3.4"));

            var request = messageHandler.Dequeue();
            Assert.That(request.RequestUri.ToString(), Is.EqualTo("http://localhost/e1crdbg/rdbg?cmd=getRDbgAPIVer"));
        }

        [Test]
        public void TestCreateSession()
        {
            // given
            var client = Create(new MockHttpMessageHandler());

            // when
            var session = client.CreateSession("testAlias");

            // than
            Assert.That(session, Is.InstanceOf<IDebuggerClientSession>());
        }

        private static IDebuggerClient Create(MockHttpMessageHandler messageHandler)
        {
            var executor = messageHandler.CreateExecutor();
            return DebuggerClient.NewInstance(executor);
        }
    }
}
