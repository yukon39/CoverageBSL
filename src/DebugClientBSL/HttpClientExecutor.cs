using com.github.yukon39.DebugBSL;
using com.github.yukon39.DebugBSL.data;
using com.github.yukon39.DebugBSL.data.core;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace com.github.yukon39.DebugClientBSL
{
    static class HttpClientExecutor
    {
        private static readonly HttpClient Client = new HttpClient();

        public static async Task<T> ExecuteAsync<T>(IRDBGRequest request, RequestParameters parameters) where T : IRDBGResponse
        {
            var CommandURL = string.Format("{0}e1crdbg/{1}", parameters.RootUrl, parameters.ToString());

            var RequestContent = DebuggerXmlSerializer.Serialize(request);

            var ResponseContent = await HttpResponseContent(CommandURL, RequestContent);

            if (string.IsNullOrEmpty(ResponseContent))
            {
                return Activator.CreateInstance<T>();
            }
            else
            {
                return DebuggerXmlSerializer.Deserialize<T>(ResponseContent);
            }
        }

        private static async Task<string> HttpResponseContent(string url, string requestContent)
        {

            var Content = new StringContent(requestContent, Encoding.UTF8, "application/xml");
    
            var HttpRequest = new HttpRequestMessage(HttpMethod.Post, url);
            HttpRequest.Content = Content;
            HttpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            HttpRequest.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            var HttpResponse = await Client.SendAsync(HttpRequest);

            var ResponseContentString = await HttpResponse.Content.ReadAsStringAsync();
            if (!HttpResponse.IsSuccessStatusCode)
            {
                var exception = DebuggerXmlSerializer.Deserialize<VRSException>(ResponseContentString);
                var description = ErrorProcessingManager.BriefErrorDescription(exception);
                throw new Exception(description);
            }

            return ResponseContentString;
        }
    }
}
