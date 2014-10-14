using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Common.Logging;
using System.Collections.Generic;
using System.Text;

namespace IronSharp.Core
{
    public class MqRestClient: RestClient
    {
        private ITokenContainer tokenContainer;

        public MqRestClient(ITokenContainer tokenContainer)
        {
            this.tokenContainer = tokenContainer;
        }

        public RestResponse<T> GetKeystone<T>(KeystoneClientConfig config) where T : class
        {
            Keystone keystone = new Keystone(config.Tenant, config.Username, config.Password);
            HttpClient client = new HttpClient();
            HttpContent contentPost = new StringContent(JSON.Generate(keystone), Encoding.UTF8, "application/json");
            String uri = config.Server.TrimEnd('/') + "/tokens";
            HttpResponseMessage response = client.PostAsync(uri, contentPost).Result;
            return new RestResponse<T>(response);
        }

        protected override void SetOauthHeaderIfRequired(IronClientConfig config, IRestClientRequest request, HttpRequestHeaders headers)
        {
            if (request.AuthTokenLocation == AuthTokenLocation.Header)
            {
                String token = tokenContainer.getToken();
                headers.Authorization = new AuthenticationHeaderValue("OAuth", token);
            }
        }
    }
}
