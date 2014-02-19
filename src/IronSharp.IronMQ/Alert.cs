using System;
using System.Net;
using Newtonsoft.Json;

namespace IronSharp.IronMQ
{
    public class Alert
    {
        public Alert() : this(null, null, null, null)
        {
        }

        public Alert(string type, string direction, int? trigger, string queue)
        {
            Type = type;
            Direction = direction;
            Trigger = trigger;
            Queue = queue;
        }

        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("snooze", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? Snooze { get; set; }

        [JsonProperty("queue", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Queue { get; set; }

        [JsonProperty("trigger", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? Trigger { get; set; }

        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("direction", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Direction { get; set; }

        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Status { get; set; }

        [JsonProperty("status_code", DefaultValueHandling = DefaultValueHandling.Ignore)]
        protected int? Code { get; set; }

        [JsonIgnore]
        public HttpStatusCode StatusCode
        {
            get { return (HttpStatusCode) Code.GetValueOrDefault(200); }
        }
    }
}