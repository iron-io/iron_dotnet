using System.Threading.Tasks;

namespace IronSharp.Core
{
    public class IronConfigTokenContainer : ITokenContainer
    {
        private readonly IronClientConfig _config;

        public IronConfigTokenContainer(IronClientConfig config)
        {
            _config = config;
        }

        public AuthToken GetToken()
        {
            return new AuthToken
            {
                Token = _config.Token,
                Location = AuthTokenLocation.Header,
                Scheme = "OAuth"
            };
        }

        public Task<AuthToken> GetTokenAsync()
        {
            return Task.FromResult(GetToken());
        }
    }
}