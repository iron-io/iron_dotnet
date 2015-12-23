using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

namespace IronSharp.IronMQ
{
    public class Subscriber
    {
        public Subscriber() : this(null,null,null)
        {
        }

        public Subscriber(string name, string url)
        {
            Name = name;
            Url = url;
        }

        public Subscriber(string name, string url, Dictionary<string,string> headers )
        {
            Name = name;
            Url = url;
            Headers = headers;
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

        [JsonProperty("headers", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, string> Headers;
            
        [JsonIgnore]
        public HttpStatusCode StatusCode
        {
            get { return (HttpStatusCode) Code.GetValueOrDefault(200); }
        }
    }
}