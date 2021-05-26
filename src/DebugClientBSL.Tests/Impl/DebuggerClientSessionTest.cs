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
        public async Task TestAttachDebugTargetAsync()
        {
            // given
            var sessionId = Guid.NewGuid();
            var response = new RDBGEmptyResponse();

            var messageHandler = new MockHttpMessageHandler();
            messageHandler.Enqueue(HttpStatusCode.OK, response);

            var session = Create(messageHandler, sessionId);
            var debugTargetId = new DebugTargetIdLight()
            {
                ID = Guid.NewGuid()
            };

            // when
            await session.AttachDebugTargetAsync(debugTargetId);

            // then
            var request = messageHandler.Dequeue();
            Assert.AreEqual("http://localhost/e1crdbg/rdbg?cmd=attachDetachDbgTargets", request.RequestUri.ToString());
        }

        [Test]
        public async Task TestDetachDebugTargetAsync()
        {
            // given
            var sessionId = Guid.NewGuid();
            var response = new RDBGEmptyResponse();

            var messageHandler = new MockHttpMessageHandler();
            messageHandler.Enqueue(HttpStatusCode.OK, response);

            var session = Create(messageHandler, sessionId);
            var debugTargetId = new DebugTargetIdLight()
            {
                ID = Guid.NewGuid()
            };

            // when
            await session.DetachDebugTargetAsync(debugTargetId);

            // then
            var request = messageHandler.Dequeue();
            Assert.AreEqual("http://localhost/e1crdbg/rdbg?cmd=attachDetachDbgTargets", request.RequestUri.ToString());
        }

        [Test]
        public async Task TestAttachedTargetsStatesAsync()
        {
            // given
            var sessionId = Guid.NewGuid();
            var response = new RDBGGetDbgAllTargetStatesResponse();

            var messageHandler = new MockHttpMessageHandler();
            messageHandler.Enqueue(HttpStatusCode.OK, response);

            var session = Create(messageHandler, sessionId);
            var areaName = "";

            // when
            var result = await session.AttachedTargetsStatesAsync(areaName);

            // then
            Assert.IsInstanceOf<List<DbgTargetStateInfo>>(result);

            var request = messageHandler.Dequeue();
            Assert.AreEqual("http://localhost/e1crdbg/rdbg?cmd=getDbgAllTargetStates", request.RequestUri.ToString());
        }

        [Test]
        public async Task TestSetAutoAttachSettingsAsync()
        {
            // given
            var sessionId = Guid.NewGuid();
            var response = new RDBGEmptyResponse();

            var messageHandler = new MockHttpMessageHandler();
            messageHandler.Enqueue(HttpStatusCode.OK, response);

            var session = Create(messageHandler, sessionId);
            var autoAttachSettings = new DebugAutoAttachSettings();

            // when
            await session.SetAutoAttachSettingsAsync(autoAttachSettings);

            // then
            var request = messageHandler.Dequeue();
            Assert.AreEqual("http://localhost/e1crdbg/rdbg?cmd=setAutoAttachSettings", request.RequestUri.ToString());
        }

        [Test]
        public async Task TestClearBreakOnNextStatementAsync()
        {
            // given
            var sessionId = Guid.NewGuid();
            var response = new RDBGEmptyResponse();

            var messageHandler = new MockHttpMessageHandler();
            messageHandler.Enqueue(HttpStatusCode.OK, response);

            var session = Create(messageHandler, sessionId);

            // when
            await session.ClearBreakOnNextStatementAsync();

            // then
            var request = messageHandler.Dequeue();
            Assert.AreEqual("http://localhost/e1crdbg/rdbg?cmd=clearBreakOnNextStatement", request.RequestUri.ToString());
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
        public async Task TestSetMeasureModeAsync()
        {
            // given
            var sessionId = Guid.NewGuid();
            var response = new RDBGEmptyResponse();

            var messageHandler = new MockHttpMessageHandler();
            messageHandler.Enqueue(HttpStatusCode.OK, response);

            var session = Create(messageHandler, sessionId);
            var measureMode = Guid.NewGuid();

            // when
            await session.SetMeasureModeAsync(measureMode);

            // then
            var request = messageHandler.Dequeue();
            Assert.AreEqual("http://localhost/e1crdbg/rdbg?cmd=setMeasureMode", request.RequestUri.ToString());
        }

        private static DebuggerClientSession Create(MockHttpMessageHandler messageHandler, Guid sessionId)
        {
            var executor = messageHandler.CreateExecutor();
            return DebuggerClientSession.NewInstance(executor, "testAlias", sessionId);
        }
    }
}
