using IronIO.Core;
using Newtonsoft.Json;

namespace IronIO.IronMQ
{
    public class MessageOptions : IInspectable
    {
        /// <summary>
        ///     The item will not be available on the queue until this many seconds have passed.
        ///     Default is 0 seconds.
        ///     Maximum is 604,800 seconds (7 days).
        /// </summary>
        [JsonProperty("delay", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? Delay { get; set; }

        /// <summary>
        ///     How long in seconds to keep the item on the queue before it is deleted.
        ///     Default is 604,800 seconds (7 days).
        ///     Maximum is 2,592,000 seconds (30 days).
        /// </summary>
        [JsonProperty("expires_in", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? ExpiresIn { get; set; }

        /// <summary>
        ///     After timeout (in seconds), item will be placed back onto queue.
        ///     You must delete the message from the queue to ensure it does not go back onto the queue.
        ///     Default is 60 seconds.
        ///     Minimum is 30 seconds, and maximum is 86,400 seconds (24 hours).
        /// </summary>
        [JsonProperty("timeout", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? Timeout { get; set; }

        /// <summary>
        ///     When message is reserved this property stores actual reservation_id. This id can be used
        ///     for deleting the message, touching or releasing.
        /// </summary>
        [JsonProperty("reservation_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ReservationId { get; set; }

        [JsonProperty("msg", DefaultValueHandling = DefaultValueHandling.Ignore)]
        protected string Msg { get; set; }
    }
}