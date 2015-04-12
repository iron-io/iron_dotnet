using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace IronIO.Core
{
    public class KeystoneContainer : ITokenContainer
    {
        private static ITokenContainer _instance;
        private readonly KeystoneClientConfig _config;

        private KeystoneContainer(KeystoneClientConfig config)
        {
            _config = config;
        }

        public DateTime LocalExpiresAt { get; set; }
        public AuthToken Token { get; set; }

        [SuppressMessage("ReSharper", "InvertIf")]
        public AuthToken GetToken()
        {
            if (KeystoneUtil.CurrentTokenIsInvalid(Token, LocalExpiresAt))
            {
                var response = new KeystoneClient().GetKeystone(_config);

                UpdateLocalValues(response);
            }

            return Token;
        }

        [SuppressMessage("ReSharper", "InvertIf")]
        public async Task<AuthToken> GetTokenAsync()
        {
            if (KeystoneUtil.CurrentTokenIsInvalid(Token, LocalExpiresAt))
            {
                var response = await new KeystoneClient().GetKeystoneAsync(_config);

                UpdateLocalValues(response);
            }

            return Token;
        }

        public static ITokenContainer Create(KeystoneClientConfig config)
        {
            return new KeystoneContainer(config);
        }

        public static ITokenContainer GetOrCreateInstance(KeystoneClientConfig config)
        {
            return LazyInitializer.EnsureInitialized(ref _instance, () => Create(config));
        }

        private void UpdateLocalValues(KeystoneResponse response)
        {
            var keyStoneToken = response.Access.Token;

            LocalExpiresAt = KeystoneUtil.ComputeNewExpirationDate(keyStoneToken);

            Token = new AuthToken
            {
                Location = AuthTokenLocation.Header,
                Scheme = "OAuth",
                Token = keyStoneToken.Id
            };
        }
    }
}