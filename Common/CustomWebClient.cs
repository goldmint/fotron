using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Fotron.Common
{
    public class CustomWebClient : System.Net.WebClient
    {
        public int Timeout { get; set; }

        public CustomWebClient() : base()
        {
            Timeout = 60 * 1000;
        }


        protected override System.Net.WebRequest GetWebRequest(Uri uri)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var webRequest = base.GetWebRequest(uri);
            webRequest.Timeout = Timeout;
            webRequest.Headers[HttpRequestHeader.UserAgent] = "Chrome";
            ((HttpWebRequest)webRequest).ReadWriteTimeout = Timeout;
            return webRequest;
        }
    }
}
