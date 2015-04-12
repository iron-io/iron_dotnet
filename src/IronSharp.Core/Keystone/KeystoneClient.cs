using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IronIO.Core
{
    public class KeystoneClient
    {
        public KeystoneResponse GetKeystone(KeystoneClientConfig config)
        {
            using (var client = RestUtility.CreateHttpClient())
            {
                var response = PostAsync(config, client).Result;

                return response.Content.ReadAsAsync<KeystoneResponse>().Result;
            }
        }

        public async Task<KeystoneResponse> GetKeystoneAsync(KeystoneClientConfig config)
        {
            using (var client = RestUtility.CreateHttpClient())
            {
                var response = await PostAsync(config, client);

                return await response.Content.ReadAsAsync<KeystoneResponse>();
            }
        }

        protected virtual string BuildTokensRequestUrl(KeystoneClientConfig config)
        {
            return string.Format("{0}/tokens", VirtualPathUtility.RemoveTrailingSlash(config.Server));
        }

        protected virtual HttpContent CreatePostContent(KeystoneClientConfig config)
        {
            var keystone = new Keystone(config.Tenant, config.Username, config.Password);

            return new StringContent(JSON.Generate(keystone), Encoding.UTF8, "application/json");
        }

        protected virtual HttpRequestMessage CreateRequestMessage(KeystoneClientConfig config)
        {
            return new HttpRequestMessage(HttpMethod.Post, BuildTokensRequestUrl(config))
            {
                Content = CreatePostContent(config)
            };
        }

        protected virtual Task<HttpResponseMessage> PostAsync(KeystoneClientConfig config, HttpClient client)
        {
            var request = CreateRequestMessage(config);

            return client.SendAsync(request);
        }
    }
}