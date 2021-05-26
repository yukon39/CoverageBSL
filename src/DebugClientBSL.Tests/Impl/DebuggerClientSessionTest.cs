using com.github.yukon39.DebugBSL.Client.Impl;
using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;

namespace com.github.yukon39.DebugBSL.Client.Tests
{
    class DebuggerClientSessionTest
    {

        [Test]
        public async Task TestAttachAsync()
        {
            // given
            var sessionId = Guid.NewGuid();
            var response = new RDBGAttachDebugUIResponse()
            {
                Result = AttachDebugUIResult.Registered
            };

            var messageHandler = new MockHttpMessageHandler();
            messageHandler.Enqueue(HttpStatusCode.OK, response);

            var session = Create(messageHandler, sessionId);
            var password = "test";

            // when
            var result = await session.AttachAsync(password.ToCharArray(), new DebuggerOptions());

            // then
            Assert.AreEqual(AttachDebugUIResult.Registered, result);

            var request = messageHandler.Dequeue();
            Assert.AreEqual("http://localhost/e1crdbg/rdbg?cmd=attachDebugUI", request.RequestUri.ToString());
        }

        [Test]
        public async Task TestDetachAsync()
        {
            // given
            var sessionId = Guid.NewGuid();
            var response = new RDBGDetachDebugUIResponse()
            {
                Result = true
            };

            var messageHandler = new MockHttpMessageHandler();
            messageHandler.Enqueue(HttpStatusCode.OK, response);

            var session = Create(messageHandler, sessionId);

            // when
            var result = await session.DetachAsync();

            // then
            Assert.IsTrue(result);

            var request = messageHandler.Dequeue();
            Assert.AreEqual("http://localhost/e1crdbg/rdbg?cmd=detachDebugUI", request.RequestUri.ToString());
        }


        [Test]
        public async Task TestPingAsync()
        {
            // given
            var sessionId = Guid.NewGuid();
            var requestUri = string.Format("http://localhost/e1crdbg/rdbg?cmd=pingDebugUIParams&dbgui={0}", sessionId);
            var response = new RDBGPingDebugUIResponse();

            var messageHandler = new MockHttpMessageHandler();
            messageHandler.Enqueue(HttpStatusCode.OK, response);

            var session = Create(messageHandler, sessionId);

            // when
            await session.PingAsync();

            // then
            var request = messageHandler.Dequeue();
            Assert.AreEqual(requestUri, request.RequestUri.ToString());
        }

        [Test]
        public async Task TestPingAsyncWithTimeSpan()
        {
            // given
            var sessionId = Guid.NewGuid();
            var requestUri = string.Format("http://localhost/e1crdbg/rdbg?cmd=pingDebugUIParams&dbgui={0}", sessionId);
            var response = new RDBGPingDebugUIResponse();

            var messageHandler = new MockHttpMessageHandler();
            messageHandler.Enqueue(HttpStatusCode.OK, response);

            var session = Create(messageHandler, sessionId);
            var timeSpan = TimeSpan.FromTicks(1);

            // when
            var result = await session.PingAsync(timeSpan);

            // then
            Assert.IsTrue(result);

            var request = messageHandler.Dequeue();
            Assert.AreEqual(requestUri, request.RequestUri.ToString());
        }

        private static IDebuggerClientSession Create(MockHttpMessageHandler messageHandler, Guid sessionId)
        {
            var executor = messageHandler.CreateExecutor();
            return DebuggerClientSession.NewInstance(executor, "testAlias", sessionId);
        }
    }
}
