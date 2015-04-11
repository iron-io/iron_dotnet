using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace IronSharp.Core
{
    public interface IRequestAuthBuilder
    {
        void SetOathQueryParameterIfRequired(IRestClientRequest request, string token);

        void SetOauthHeaderIfRequired(IronClientConfig config, IRestClientRequest request, HttpRequestHeaders headers);

        Task SetOauthHeaderIfRequiredAsync(IronClientConfig config, IRestClientRequest request, HttpRequestHeaders headers);
    }
}