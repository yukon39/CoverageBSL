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

            using var session = Create(messageHandler, sessionId);
            var password = "test";

            // when
            var result = await session.AttachAsync(password.ToCharArray(), new DebuggerOptions());

            // then
            Assert.That(result, Is.EqualTo(AttachDebugUIResult.Registered));

            var request = messageHandler.Dequeue();
            Assert.That(request.RequestUri.ToString(), Is.EqualTo("http://localhost/e1crdbg/rdbg?cmd=attachDebugUI"));
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

            using var session = Create(messageHandler, sessionId);

            // when
            var result = await session.DetachAsync();

            // then
            Assert.That(result, Is.True);

            var request = messageHandler.Dequeue();
            Assert.That(request.RequestUri.ToString(), Is.EqualTo("http://localhost/e1crdbg/rdbg?cmd=detachDebugUI"));
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

            using var session = Create(messageHandler, sessionId);

            // when
            await session.PingAsync();

            // then
            var request = messageHandler.Dequeue();
            Assert.That(request.RequestUri.ToString(), Is.EqualTo(requestUri));
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

            using var session = Create(messageHandler, sessionId);
            var timeSpan = TimeSpan.FromTicks(1);

            // when
            var result = await session.PingAsync(timeSpan);

            // then
            Assert.That(result, Is.True);

            var request = messageHandler.Dequeue();
            Assert.That(request.RequestUri.ToString(), Is.EqualTo(requestUri));
        }

        [Test]
        public async Task TestPingAsyncReleasesSemaphoreWhenRequestIsCanceled()
        {
            // given
            var sessionId = Guid.NewGuid();
            var response = new RDBGPingDebugUIResponse();

            var messageHandler = new MockHttpMessageHandler();
            messageHandler.EnqueueCanceled();
            messageHandler.Enqueue(HttpStatusCode.OK, response);

            using var session = Create(messageHandler, sessionId);

            // when
            Assert.ThrowsAsync<TaskCanceledException>(async () => await session.PingAsync());
            var result = await session.PingAsync(TimeSpan.FromSeconds(1));

            // then
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task TestTimerIgnoresCanceledPing()
        {
            // given
            var sessionId = Guid.NewGuid();
            var attachResponse = new RDBGAttachDebugUIResponse()
            {
                Result = AttachDebugUIResult.Registered
            };
            var detachResponse = new RDBGDetachDebugUIResponse()
            {
                Result = true
            };

            var messageHandler = new MockHttpMessageHandler();
            messageHandler.Enqueue(HttpStatusCode.OK, attachResponse);
            messageHandler.EnqueueCanceled();
            messageHandler.Enqueue(HttpStatusCode.OK, detachResponse);

            using var session = Create(messageHandler, sessionId);
            await session.AttachAsync("test".ToCharArray(), new DebuggerOptions());

            // when
            var waitUntil = DateTime.UtcNow.AddSeconds(5);
            while (messageHandler.RequestCount < 2 && DateTime.UtcNow < waitUntil)
            {
                await Task.Delay(25);
            }

            var result = await session.DetachAsync();

            // then
            Assert.That(messageHandler.RequestCount, Is.EqualTo(3));
            Assert.That(result, Is.True);
        }

        private static DebuggerClientSession Create(MockHttpMessageHandler messageHandler, Guid sessionId)
        {
            var executor = messageHandler.CreateExecutor();
            return DebuggerClientSession.NewInstance(executor, "testAlias", sessionId);
        }
    }
}
