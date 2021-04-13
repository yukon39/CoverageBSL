using com.github.yukon39.CoverageBSL.debugger;
using com.github.yukon39.CoverageBSL.debugger.debugRDBGRequestResponse;
using System;
using System.Net.Http;

namespace com.github.yukon39.CoverageBSL.httpDebug
{
    class HTTPDebugClient : IDebuggerClient
    {
        private static readonly HttpClient Client = new();

        private Uri DebugServerURL;

        private HTTPDebugClient(Uri debugServerURL)
        {
            DebugServerURL = debugServerURL;
        }

        public void Test()
        {
            var RequestParameters = new RequestParameters
            {
                Resource = "rdbgTest",
                Command = "test"
            };

            var Request = new RDBGTestRequest();

            Execute<RDBGTestResponse>(Request, RequestParameters);

            //Logger.LogDebug("Test successful");
        }

        public string ApiVersion()
        {
            var RequestParameters = new RequestParameters
            {
                Command = "getRDbgAPIVer"
            };

            var Request = new MiscRDbgGetAPIVerRequest();

            var Response = Execute<MiscRDbgGetAPIVerResponse>(Request, RequestParameters);
            var Version = Response.Version;

            return Version;
        }

        public IDebuggerSession CreateSession(string infobaseAlias)
        {
            return new HTTPDebugSession(this, infobaseAlias);
        }

        public T Execute<T>(IRDBGRequest request, RequestParameters requestParameters)
        {
            var CommandURL = string.Format("{0}/e1crdbg/{1}", DebugServerURL, requestParameters.ToString());

            var Content = HTTPDebugSerializer.Serialize(request);

            var HTTPContent = new StringContent(Content);
            HTTPContent.Headers.Add("Content-Type", "application/xml; charset=utf-8");
            HTTPContent.Headers.Add("Accept", "application/xml");
            HTTPContent.Headers.Add("Accept-Encoding", "gzip");

            Client.PostAsync(CommandURL, HTTPContent);

            return Activator.CreateInstance<T>();
        }

        public static IDebuggerClient Build(Uri debugServerURL) => new HTTPDebugClient(debugServerURL);
    }
}
