using Newtonsoft.Json;

namespace IronSharp.Core
{
    public class KeystoneResponse
    {
        [JsonProperty("access")]
        public KeystoneAccess Access { get; set; }
    }
}