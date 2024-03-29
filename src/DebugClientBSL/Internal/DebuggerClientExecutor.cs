﻿using com.github.yukon39.DebugBSL.Client.Data;
using com.github.yukon39.DebugBSL.data;
using com.github.yukon39.DebugBSL.data.core;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace com.github.yukon39.DebugBSL.Client.Internal
{
    public class DebuggerClientExecutor
    {
        private readonly HttpClient Client;
        private readonly Uri RootUrl;

        private DebuggerClientExecutor(Uri rootUrl, HttpClient client)
        {
            Client = client;
            RootUrl = rootUrl;
        }

        public static DebuggerClientExecutor Create(Uri rootUrl)
        {
            var httpClient = new HttpClient();
            ConfigureHttpClient(httpClient);
            return new DebuggerClientExecutor(rootUrl, httpClient);
        }

        public static DebuggerClientExecutor Create(Uri rootUrl, HttpClient httpClient) =>
            new DebuggerClientExecutor(rootUrl, httpClient);

        public static void ConfigureHttpClient(HttpClient client)
        {
            var requestHeaders = client.DefaultRequestHeaders;
            requestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            requestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            client.Timeout = TimeSpan.FromSeconds(15);
        }

        public async Task<T> ExecuteAsync<T>(IRDBGRequest request, RequestParameters parameters) where T : IRDBGResponse
        {
            var requesrUrl = parameters.RequestUrl(RootUrl);

            var requestContent = DebuggerXmlSerializer.Serialize(request);

            var responseContent = await HttpResponseContent(requesrUrl, requestContent);

            if (string.IsNullOrEmpty(responseContent))
            {
                return Activator.CreateInstance<T>();
            }
            else
            {
                return DebuggerXmlSerializer.Deserialize<T>(responseContent);
            }
        }

        private async Task<string> HttpResponseContent(Uri requestUrl, string requestContent)
        {

            var content = new StringContent(requestContent, Encoding.UTF8, "application/xml");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            httpRequest.Content = content;
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            httpRequest.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            var httpResponse = await Client.SendAsync(httpRequest);

            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            if (httpResponse.IsSuccessStatusCode)
            {
                return responseContent;
            }
            else
            {
                GenericException exception;
                string description;

                exception = DebuggerXmlSerializer.TryDeserialize<VRSException>(responseContent);
                if (exception is null)
                {
                    exception = DebuggerXmlSerializer.TryDeserialize<GenericException>(responseContent);
                }

                if (exception is null)
                {
                    description = responseContent;
                }
                else
                {
                    description = ErrorProcessingManager.BriefErrorDescription(exception);
                }

                throw new Exception(description);
            }
        }
    }
}
