using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

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
    }
}