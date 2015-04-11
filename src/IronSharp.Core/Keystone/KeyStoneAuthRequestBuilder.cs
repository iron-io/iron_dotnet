using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace IronSharp.Core
{
    public class KeystoneAuthRequestBuilder : IronRequestBuilder
    {
        private readonly ITokenContainer _tokenContainer;

        public KeystoneAuthRequestBuilder(ITokenContainer tokenContainer)
        {
            _tokenContainer = tokenContainer;
        }

        public override void SetOauthHeaderIfRequired(IronClientConfig config, IRestClientRequest request, HttpRequestHeaders headers)
        {
            if (request.AuthTokenLocation == AuthTokenLocation.Header)
            {
                SetAuthHeader(headers, _tokenContainer.GetToken());
            }
        }

        private static void SetAuthHeader(HttpRequestHeaders headers, string token)
        {
            headers.Authorization = new AuthenticationHeaderValue("OAuth", token);
        }

        public override async Task SetOauthHeaderIfRequiredAsync(IronClientConfig config, IRestClientRequest request, HttpRequestHeaders headers)
        {
            if (request.AuthTokenLocation == AuthTokenLocation.Header)
            {
                SetAuthHeader(headers, await _tokenContainer.GetTokenAsync());
            }
        }
    }
}