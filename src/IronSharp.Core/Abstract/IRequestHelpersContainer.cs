using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace IronSharp.Core.Abstract
{
    interface IRequestHelpersContainer
    {
        Uri BuildUri(IronClientConfig config, string path, NameValueCollection query);

        void SetOathQueryParameterIfRequired(IRestClientRequest request, string token);

        void SetOauthHeaderIfRequired(IronClientConfig config, IRestClientRequest request, HttpRequestHeaders headers);
    }
}
