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
        public IRequestHelpersContainer RequestHelpersContainer { get; set; }

        public HttpRequestMessageBuilder(IronClientConfig config, IRequestHelpersContainer requestHelpersContainer, IRestClientRequest request)
        {
            Config = config;
            RequestHelpersContainer = requestHelpersContainer;
            Request = request;
        }

        public HttpRequestMessage Build()
        {
            RequestHelpersContainer.SetOathQueryParameterIfRequired(Request, Config.Token);
            var httpRequest = new HttpRequestMessage
            {
                Content = Payload != null ? new JsonContent(Payload) : Request.Content,
                RequestUri = RequestHelpersContainer.BuildUri(Config, Request.EndPoint, Request.Query),
                Method = Request.Method
            };

            HttpRequestHeaders headers = httpRequest.Headers;
            RequestHelpersContainer.SetOauthHeaderIfRequired(Config, Request, headers);

            headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Request.Accept));

            return httpRequest;
        }
    }
}
