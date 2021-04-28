using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.github.yukon39.DebugClientBSL
{
    class RequestParameters
    {
        private readonly Uri RootUrl;
        private readonly string Resource;
        private readonly Dictionary<string, string> Parameters = new Dictionary<string, string>();

        public RequestParameters(Uri rootUrl, string command) : this(rootUrl, command, "rdbg") { }

        public RequestParameters(Uri rootUrl, string command, string resource)
        {
            RootUrl = rootUrl;
            Resource = resource;
            Parameters.Add("cmd", command);
        }

        public Guid DebugID
        {
            set
            {
                Parameters.Add("dbgui", value.ToString());
            }
        }

        public Uri RequestUrl()
        {
            var query = Parameters.Select(x => string.Format("{0}={1}", x.Key, x.Value));

            var uriBuilder = new UriBuilder(RootUrl);
            uriBuilder.Path = string.Format("e1crdbg/{0}", Resource);
            uriBuilder.Query = string.Join("&", query);

            return uriBuilder.Uri;
        }
    }
}
