using System.Net;
using Newtonsoft.Json;

namespace IronSharp.IronMQ
{
    public class Subscriber
    {
        public Subscriber() : this(null, null)
        {
        }

        public Subscriber(string name, string url)
        {
            Name = name;
            Url = url;
        }

        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("retries_delay", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? RetriesDelay { get; set; }

        [JsonProperty("retries_remaining", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? RetriesRemaining { get; set; }

        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Status { get; set; }

        [JsonProperty("status_code", DefaultValueHandling = DefaultValueHandling.Ignore)]
        protected int? Code { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("error_queue", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ErrorQueue { get; set; }

        [JsonIgnore]
        public HttpStatusCode StatusCode => (HttpStatusCode) Code.GetValueOrDefault(200);
    }
}