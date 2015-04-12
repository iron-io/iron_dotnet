using System.Net.Http;
using System.Threading;

namespace IronSharp.Core
{
    public class IronTaskClient
    {
        private static HttpClient _httpClient;
        private IAuthTokenManager _authTokenManager;

        public static HttpClient Client
        {
            get { return LazyInitializer.EnsureInitialized(ref _httpClient, RestUtility.CreateHttpClient); }
        }

        public IAuthTokenManager AuthTokenManager
        {
            get { return LazyInitializer.EnsureInitialized(ref _authTokenManager, () => new IronTokenManager()); }
        }
    }
}