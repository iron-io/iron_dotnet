using Newtonsoft.Json;

namespace IronSharp.Core
{
    public class KeystoneAccess
    {
        [JsonProperty("token")]
        public KestoneToken Token { get; set; }
    }
}