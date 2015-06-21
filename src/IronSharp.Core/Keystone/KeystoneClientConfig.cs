using Newtonsoft.Json;

namespace IronIO.Core
{
    public class KeystoneClientConfig
    {
        [JsonProperty("tenant")]
        public string Tenant { get; set; }

        [JsonProperty("server")]
        public string Server { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        public virtual string CreateKey()
        {
            return string.Concat(Tenant, Server, Username);
        }

        public override string ToString()
        {
            return $"Tenant: {Tenant}, Server: {Server}, Username: {Username}, Password: {Password}";
        }
    }
}