using System;
using Newtonsoft.Json;

namespace IronIO.Core
{
    public class Keystone
    {
        public Keystone(String tenant, String username, String password)
        {
            var credentials = new Credentials(username, password);
            Auth = new Auth(tenant, credentials);
        }

        [JsonProperty("auth")]
        public Auth Auth { get; set; }
    }
}