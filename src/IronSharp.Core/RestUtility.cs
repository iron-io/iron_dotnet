using System.Net;
using System.Net.Http;

namespace IronSharp.Core
{
    public static class RestUtility
    {
        public static HttpClient CreateHttpClient()
        {
            return HttpClientFactory.Create(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            });
        }
    }
}