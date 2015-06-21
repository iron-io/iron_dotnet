using System.Collections.Generic;
using System.Threading;
using IronIO.Core;
using Newtonsoft.Json;

namespace IronIO.IronMQ
{
    public class PushInfo : IInspectable
    {
        [JsonProperty("subscribers", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private List<Subscriber> _subscribers;

        /// <summary>
        ///     How many times to retry on failure.
        ///     Default is 3.
        ///     Maximum is 100.
        /// </summary>
        [JsonProperty("retries", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? Retries { get; set; }

        /// <summary>
        ///     Delay between each retry in seconds.
        ///     Default is 60.
        /// </summary>
        [JsonProperty("retries_delay", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? RetriesDelay { get; set; }

        [JsonProperty("error_queue", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ErrorQueueName { get; set; }

        [JsonProperty("rate_limit", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? RateLimit { get; set; }

        [JsonIgnore]
        public List<Subscriber> Subscribers
        {
            get { return LazyInitializer.EnsureInitialized(ref _subscribers); }
            set { _subscribers = value; }
        }
    }
}