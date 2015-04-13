using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace IronIO.Core
{
    public class KeystoneContainer : ITokenContainer
    {
        private static readonly ConcurrentDictionary<string, ITokenContainer> KeystoneContainers =
            new ConcurrentDictionary<string, ITokenContainer>();

        private readonly KeystoneClientConfig _config;

        private KeystoneContainer(KeystoneClientConfig config)
        {
            _config = config;
        }

        public DateTime LocalExpiresAt { get; set; }
        public AuthToken Token { get; set; }

        public AuthToken GetWebHookToken(string tokenId)
        {
            return new AuthToken
            {
                Location = AuthTokenLocation.Header,
                Scheme = "oauth",
                Token = tokenId
            };
        }

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
            var key = config.CreateKey();

            return KeystoneContainers.GetOrAdd(key, k => Create(config));
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