using System.Threading.Tasks;

namespace IronIO.Core
{
    public class IronConfigTokenContainer : ITokenContainer
    {
        private readonly IronClientConfig _config;

        public IronConfigTokenContainer(IronClientConfig config)
        {
            _config = config;
        }

        public AuthToken GetWebHookToken(string tokenId)
        {
            string tokenValue = _config.Token;

            if (!string.IsNullOrEmpty(tokenId))
            {
                tokenValue = tokenId;
            }

            return new AuthToken
            {
                Token = tokenValue,
                Location = AuthTokenLocation.Querystring,
                Scheme = "oauth"
            };
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