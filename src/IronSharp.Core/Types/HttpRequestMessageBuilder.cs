using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace IronSharp.Core.Types
{
    internal class HttpRequestMessageBuilder
    {
        public IronClientConfig Config { get; set; }
        
        public IRestClientRequest Request { get; set; }
        
        public Object Payload { get; set; }

        public ITokenManager TokenManager { get; set; }

        public HttpRequestMessageBuilder(IronClientConfig config, ITokenManager tokenManager, IRestClientRequest request)
        {
            Config = config;
            TokenManager = tokenManager;
            Request = request;
        }

        public HttpRequestMessage Build()
        {
            TokenManager.SetAuthQueryParameterIfRequired(Request, Config.Token);

            var httpRequest = new HttpRequestMessage
            {
                Content = Payload != null ? new JsonContent(Payload) : Request.Content,
                RequestUri = TokenManager.BuildUri(Config, Request.EndPoint, Request.Query),
                Method = Request.Method
            };

            HttpRequestHeaders headers = httpRequest.Headers;

            TokenManager.SetAuthHeaderIfRequired(Config, Request, headers);

            headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Request.Accept));

            return httpRequest;
        }
    }
}
