using System;
using Newtonsoft.Json;

namespace IronIO.Core
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

        public virtual string CreateKey()
        {
            return string.Concat(Tenant, Server, Username);
        }

        public override string ToString()
        {
            return string.Format("Tenant: {0}, Server: {1}, Username: {2}, Password: {3}", Tenant, Server, Username, Password);
        }
    }
}
