using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IronSharp.Core
{
    public class Keystone
    {
        [JsonProperty("auth")]
        public Auth Auth { get; set; }

        public Keystone(String tenant, String username, String password)
        {
            Credentials credentials = new Credentials(username, password);
            Auth = new Auth(tenant, credentials);
        }
    }

    public class Auth
    {
        [JsonProperty("tenantName")]
        public String Tenant { get; set; }
        [JsonProperty("passwordCredentials")]
        public Credentials Credentials { get; set; }

        public Auth(String tenant, Credentials credentials)
        {
            Tenant = tenant;
            Credentials = credentials;
        }
    }

    public class Credentials
    {
        [JsonProperty("username")]
        public String Username { get; set; }
        [JsonProperty("password")]
        public String Password { get; set; }

        public Credentials(String username, String password)
        {
            Username = username;
            Password = password;
        }
    }

    public class KeystoneResponse
    {
        [JsonProperty("access")]
        public KeystoneAccess Access { get; set; } 
    }

    public class KeystoneAccess
    {
        [JsonProperty("token")]
        public KestoneToken Token { get; set; }
    }
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
