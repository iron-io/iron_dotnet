using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using static System.String;

namespace IronIO.Core
{
    public class IronTaskRequestBuilder
    {
        private readonly IIronTaskEndpointConfig _endpointConfig;
        private NameValueCollection _query;

        protected IronTaskRequestBuilder()
        {
            Accept = "application/json";

        }

        public IronTaskRequestBuilder(IIronTaskEndpointConfig endpointConfig) : this()
        {
            _endpointConfig = endpointConfig;
        }

        public NameValueCollection Query
        {
            get { return LazyInitializer.EnsureInitialized(ref _query, () => HttpUtility.ParseQueryString("")); }
        }

        public HttpContent HttpContent { get; set; }

        public HttpMethod HttpMethod { get; set; }

        public string Accept { get; set; }

        public AuthToken AuthToken { get; set; }

        public string Path { get; set; }

        public void SetJsonContent(object value)
        {
            HttpContent = new JsonContent(value);
        }

        public virtual Uri BuildUri(IronClientConfig config, string path, NameValueCollection query)
        {
            var queryString = "";

            if (query != null && query.Count > 0)
            {
                var httpValueCollection = HttpUtility.ParseQueryString("");

                httpValueCollection.Add(query);

                queryString = httpValueCollection.ToString();
            }

            var scheme = IsNullOrEmpty(config.Scheme) ? Uri.UriSchemeHttps : config.Scheme;


            var uriBuilder = new UriBuilder(scheme, config.Host)
            {
                Path = BuildPath(config, path),
                Query = queryString
            };

            if (config.Port.HasValue)
            {
                uriBuilder.Port = config.Port.Value;
            }

            return uriBuilder.Uri;
        }

        public HttpRequestMessage Build()
        {
            var authToken = _endpointConfig.TokenContainer.GetToken();
            return Build(authToken);
        }

        public async Task<HttpRequestMessage> BuildAsync()
        {
            var authToken = await _endpointConfig.TokenContainer.GetTokenAsync();
            return Build(authToken);
        }

        private HttpRequestMessage Build(AuthToken authToken)
        {
            var config = _endpointConfig.Config;

            if (authToken.Location == AuthTokenLocation.Querystring)
            {
                Query[authToken.Scheme.ToLower()] = authToken.Token;
            }

            var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod,
                Content = HttpContent,
                RequestUri = BuildUri(config, Path, Query)
            };

            var headers = httpRequest.Headers;

            headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Accept));

            if (authToken.Location == AuthTokenLocation.Header)
            {
                headers.Authorization = new AuthenticationHeaderValue(authToken.Scheme, authToken.Token);
            }

            return httpRequest;
        }

        public virtual string BuildPath(IronClientConfig config, string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }

            return $"{config.ApiVersion}/{path.Replace("{Project ID}", config.ProjectId)}";
        }
    }
}