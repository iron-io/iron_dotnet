using System.Collections.Specialized;
using System.Net.Http;

namespace IronIO.Core
{
    public interface IRestClientRequest
    {
        HttpContent Content { get; set; }

        string EndPoint { get; set; }

        NameValueCollection Query { get; set; }

        HttpMethod Method { get; set; }

        string Accept { get; set; }

        AuthTokenLocation AuthTokenLocation { get; set; }
    }
}