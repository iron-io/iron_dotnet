using System;
using System.Net.Http;
using System.Net.Http.Headers;
using IronSharp.Core.Abstract;

namespace IronSharp.Core.Types
{
    internal class HttpRequestMessageBuilder
    {
        public IronClientConfig Config { get; set; }
        
        public IRestClientRequest Request { get; set; }
        
        public Object Payload { get; set; }

        public IRequestBuilder RequestBuilder { get; set; }

        public HttpRequestMessageBuilder(IronClientConfig config, IRequestBuilder requestBuilder, IRestClientRequest request)
        {
            Config = config;
            RequestBuilder = requestBuilder;
            Request = request;
        }

        public HttpRequestMessage Build()
        {
            RequestBuilder.SetOathQueryParameterIfRequired(Request, Config.Token);

            var httpRequest = new HttpRequestMessage
            {
                Content = Payload != null ? new JsonContent(Payload) : Request.Content,
                RequestUri = RequestBuilder.BuildUri(Config, Request.EndPoint, Request.Query),
                Method = Request.Method
            };

            HttpRequestHeaders headers = httpRequest.Headers;

            RequestBuilder.SetOauthHeaderIfRequired(Config, Request, headers);

            headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Request.Accept));

            return httpRequest;
        }
    }
}
