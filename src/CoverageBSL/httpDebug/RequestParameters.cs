using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.github.yukon39.CoverageBSL.httpDebug
{
    class RequestParameters
    {
        private readonly Dictionary<string, string> Parameters;

        public RequestParameters()
        {
            Parameters = new Dictionary<string, string>();
            Resource = "rdbg";
        }

        public string Command
        {
            set
            {
                Parameters.Add("cmd", value);
            }
        }

        public Guid DebugID
        {
            set
            {
                Parameters.Add("dbgui", value.ToString());
            }
        }

        public string Resource { get; set; }

        public override string ToString()
        {
            var URLParams = Parameters.Select(x => string.Format("{0}={1}", x.Key, x.Value));

            var Result = new StringBuilder();
            Result.Append(Resource);
            Result.Append('?');
            Result.Append(string.Join('&', URLParams));

            return Result.ToString();
        }
    }
}
