using Newtonsoft.Json;

namespace IronIO.Core
{
    public class KeystoneResponse
    {
        [JsonProperty("access")]
        public KeystoneAccess Access { get; set; }
    }
}