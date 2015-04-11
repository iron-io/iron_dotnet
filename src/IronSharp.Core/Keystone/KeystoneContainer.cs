using System;
using System.Threading;
using System.Threading.Tasks;

namespace IronSharp.Core
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
        public string Token { get; set; }

        public String GetToken()
        {
            if (KeystoneUtil.CurrentTokenIsInvalid(Token, LocalExpiresAt))
            {
                var response = new KeystoneClient().GetKeystone(_config);

                UpdateLocalValues(response);
            }

            return Token;
        }

        public async Task<string> GetTokenAsync()
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
            Token = keyStoneToken.Id;
        }
    }
}