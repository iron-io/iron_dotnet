using System;
using Newtonsoft.Json;

namespace IronSharp.Core
{
    public class Auth
    {
        public Auth(String tenant, Credentials credentials)
        {
            Tenant = tenant;
            Credentials = credentials;
        }

        [JsonProperty("tenantName")]
        public String Tenant { get; set; }

        [JsonProperty("passwordCredentials")]
        public Credentials Credentials { get; set; }
    }
}