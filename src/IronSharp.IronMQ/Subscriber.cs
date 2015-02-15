using System;
using System.Net;
using Newtonsoft.Json;

namespace IronSharp.IronMQ
{
    public class Subscriber
    {
        public Subscriber() : this(null)
        {
        }

        public Subscriber(string url)
        {
            Url = url;
        }

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
        public HttpStatusCode StatusCode
        {
            get { return (HttpStatusCode) Code.GetValueOrDefault(200); }
        }

        public static implicit operator Subscriber(string url)
        {
            return new Subscriber(url);
        }

        public static implicit operator Subscriber(Uri uri)
        {
            return new Subscriber(uri.ToString());
        }
    }
}