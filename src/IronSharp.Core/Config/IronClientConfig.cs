using System;
using System.Threading;
using Newtonsoft.Json;

namespace IronSharp.Core
{
    public class IronClientConfig : IIronSharpConfig, IInspectable
    {
        private IronSharpConfig _sharpConfig;

        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("project_id")]
        public string ProjectId { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("api_version")]
        public int ApiVersion { get; set; }

        [JsonProperty("port")]
        public int? Port { get; set; }

        [JsonProperty("scheme")]
        public String Scheme { get; set; }

        [JsonProperty("keystone")]
        public KeystoneClientConfig Keystone { get; set; }

        [JsonProperty("sharp_config")]
        public IronSharpConfig SharpConfig
        {
            get { return LazyInitializer.EnsureInitialized(ref _sharpConfig, CreateDefaultIronSharpConfig); }
            set { _sharpConfig = value; }
        }

        public bool KeystoneKeysExist() 
        {
            return Keystone.Tenant != null && Keystone.Server != null && 
                   Keystone.Username != null && Keystone.Password != null;
        }

        private static IronSharpConfig CreateDefaultIronSharpConfig()
        {
            return new IronSharpConfig
            {
                BackoffFactor = 25
            };
        }
    }
}