using Newtonsoft.Json;

namespace IronIO.Core
{
    public class KestoneToken
    {
        [JsonProperty("issued_at")]
        public string IssuedAt { get; set; }

        [JsonProperty("expires")]
        public string Expires { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}