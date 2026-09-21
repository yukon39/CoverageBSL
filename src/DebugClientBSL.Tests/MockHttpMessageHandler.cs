using com.github.yukon39.DebugBSL.Client.Internal;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace com.github.yukon39.DebugBSL.Client.Tests
{
    class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Queue<Task<HttpResponseMessage>> responses = new Queue<Task<HttpResponseMessage>>();

        private readonly Queue<HttpRequestMessage> requests = new Queue<HttpRequestMessage>();

        private readonly SemaphoreSlim requestAdded = new SemaphoreSlim(0);

        public void Enqueue(HttpResponseMessage responseMessage)
            => Enqueue(Task.FromResult(responseMessage));

        public void EnqueueCanceled()
            => Enqueue(Task.FromCanceled<HttpResponseMessage>(new CancellationToken(true)));

        public void Enqueue(HttpStatusCode statusCode, IRDBGResponse response)
        {
            var content = DebuggerXmlSerializer.Serialize(response);
            var responseMessage = new HttpResponseMessage(statusCode);
            responseMessage.Content = new StringContent(content);
            Enqueue(responseMessage);
        }

        public int RequestCount
        {
            get
            {
                lock (requests)
                {
                    return requests.Count;
                }
            }
        }

        public HttpRequestMessage Dequeue()
        {
            lock (requests)
            {
                return requests.Dequeue();
            }
        }

        public async Task<bool> WaitForRequestCountAsync(int count, TimeSpan timeout)
        {
            while (RequestCount < count)
            {
                if (!await requestAdded.WaitAsync(timeout))
                {
                    return false;
                }
            }

            return true;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            lock (requests)
            {
                requests.Enqueue(request);
            }
            requestAdded.Release();

            lock (responses)
            {
                return responses.Dequeue();
            }
        }

        private void Enqueue(Task<HttpResponseMessage> response)
        {
            lock (responses)
            {
                responses.Enqueue(response);
            }
        }

        public DebuggerClientExecutor CreateExecutor()
        {
            var httpClient = new HttpClient(this);
            var rootUrl = new Uri("http://localhost");
            var executor = DebuggerClientExecutor.Create(rootUrl, httpClient);

            return executor;
        }
    }
}
