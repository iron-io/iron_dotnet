using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace IronSharp.Core
{
    public class KeystoneContainer: ITokenContainer
    {
        private String token;
        private KeystoneClientConfig config;
        private DateTime localExpiresAt;

        public KeystoneContainer(KeystoneClientConfig config)
        {
            this.config = config;
        }

        public String getToken()
        {
            DateTime today = System.DateTime.Now;
            if (this.token == null || this.localExpiresAt.CompareTo(today) < 0)
            {
                MqRestClient restClient = new MqRestClient(this);
                KeystoneResponse response = restClient.GetKeystone<KeystoneResponse>(this.config);
                DateTime issuedAt = DateTime.Parse(response.Access.Token.IssuedAt, null, DateTimeStyles.RoundtripKind);
                DateTime expires = DateTime.Parse(response.Access.Token.Expires, null, DateTimeStyles.RoundtripKind);
                TimeSpan timespan = expires.Subtract(issuedAt);
                this.localExpiresAt = today.Add(timespan);
                this.token = response.Access.Token.Id;
            }
            return this.token;
        }
    }
}
