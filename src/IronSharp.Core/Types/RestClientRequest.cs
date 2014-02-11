using System.Collections.Specialized;
using System.Net.Http;

namespace IronSharp.Core
{
    public class RestClientRequest : IRestClientRequest
    {
        public RestClientRequest()
        {
            Accept = "appliction/json";
            Method = HttpMethod.Get;
            AuthTokenLocation = AuthTokenLocation.Header;
        }

        public string Accept { get; set; }

        public AuthTokenLocation AuthTokenLocation { get; set; }

        public HttpContent Content { get; set; }

        public string EndPoint { get; set; }

        public HttpMethod Method { get; set; }

        public NameValueCollection Query { get; set; }
    }
}