using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;

namespace IronSharp.IronMQ
{
    public class PushInfo
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

        [JsonIgnore]
        public List<Subscriber> Subscribers
        {
            get { return LazyInitializer.EnsureInitialized(ref _subscribers); }
            set { _subscribers = value; }
        }
    }
}