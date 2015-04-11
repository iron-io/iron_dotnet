using System;
using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace IronSharp.Core
{
    public class IronRequestBuilder : IRequestBuilder
    {
        public virtual Uri BuildUri(IronClientConfig config, string path, NameValueCollection query)
        {
            var queryString = "";

            if (query != null && query.Count > 0)
            {
                var httpValueCollection = HttpUtility.ParseQueryString("");

                httpValueCollection.Add(query);

                queryString = httpValueCollection.ToString();
            }

            var scheme = String.IsNullOrEmpty(config.Scheme) ? Uri.UriSchemeHttps : config.Scheme;


            var uriBuilder = new UriBuilder(scheme, config.Host)
            {
                Path =  BuildPath(config, path),
                Query = queryString
            };

            if (config.Port.HasValue)
            {
                uriBuilder.Port = config.Port.Value;
            }

            return uriBuilder.Uri;
        }

        public virtual string BuildPath(IronClientConfig config, string path)
        {
            if (path == null) throw new ArgumentNullException("path");

            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }

            return string.Format("{0}/{1}", config.ApiVersion, path.Replace("{Project ID}", config.ProjectId));
        }

        public virtual void SetOathQueryParameterIfRequired(IRestClientRequest request, string token)
        {
            if (request.AuthTokenLocation != AuthTokenLocation.Querystring) return;

            request.Query = request.Query ?? new NameValueCollection();
            request.Query["oauth"] = token;
        }

        public virtual void SetOauthHeaderIfRequired(IronClientConfig config, IRestClientRequest request, HttpRequestHeaders headers)
        {
            if (request.AuthTokenLocation == AuthTokenLocation.Header)
            {
                headers.Authorization = new AuthenticationHeaderValue("OAuth", config.Token);
            }
        }

        public virtual Task SetOauthHeaderIfRequiredAsync(IronClientConfig config, IRestClientRequest request, HttpRequestHeaders headers)
        {
            SetOauthHeaderIfRequired(config, request, headers);
            return Task.FromResult(1);
        }
    }
}