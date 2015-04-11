using System;
using Newtonsoft.Json;

namespace IronSharp.Core
{
    public class Credentials
    {
        public Credentials(String username, String password)
        {
            Username = username;
            Password = password;
        }

        [JsonProperty("username")]
        public String Username { get; set; }

        [JsonProperty("password")]
        public String Password { get; set; }
    }
}