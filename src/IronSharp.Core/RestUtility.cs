using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

namespace IronSharp.Core
{
    public static class RestUtility
    {
        public static HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler();
            
            var client =  HttpClientFactory.Create(handler);

            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                var headers = client.DefaultRequestHeaders;

                headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            }

            return client;
        }

        private static HttpClient _defaultInstance;

        public static HttpClient DefaultInstance
        {
            get { return LazyInitializer.EnsureInitialized(ref _defaultInstance, CreateHttpClient); }
        }
    }
}