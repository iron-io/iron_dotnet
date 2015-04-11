using System;
using Newtonsoft.Json;

namespace IronSharp.Core
{
    public class KestoneToken
    {
        [JsonProperty("issued_at")]
        public String IssuedAt { get; set; }

        [JsonProperty("expires")]
        public String Expires { get; set; }

        [JsonProperty("id")]
        public String Id { get; set; }
    }
}