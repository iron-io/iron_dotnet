using IronSharp.Core;
using Newtonsoft.Json;

namespace IronSharp.IronMQ
{
    /// <summary>
    /// Info about dead letter queue, as documented on http://dev.iron.io/mq/3/reference/dlq/
    /// </summary>
    public class QueueDeadletterInfo : IInspectable
    {
        /// <summary>
        /// Name of the dead-letter queue, if one is setup
        /// </summary>
        [JsonProperty("queue_name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// Max reservations before messages are moved to this dead letter queue
        /// </summary>
        [JsonProperty("max_reservations", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int MaxReservations { get; set; }
    }
}