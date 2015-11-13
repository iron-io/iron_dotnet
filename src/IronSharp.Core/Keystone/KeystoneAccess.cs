using Newtonsoft.Json;

namespace IronIO.Core
{
    public class KeystoneAccess
    {
        [JsonProperty("token")]
        public KestoneToken Token { get; set; }
    }
}