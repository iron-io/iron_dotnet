using System;
using System.Threading;
using Newtonsoft.Json;

namespace IronSharp.Core
{
    public class KeystoneClientConfig
    {
        [JsonProperty("tenant")]
        public String Tenant { get; set; }

        [JsonProperty("server")]
        public String Server { get; set; }

        [JsonProperty("username")]
        public String Username { get; set; }

        [JsonProperty("password")]
        public String Password { get; set; }
    }
}
