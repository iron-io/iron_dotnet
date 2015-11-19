using IronIO.Core;
using Newtonsoft.Json;

namespace IronIO.IronMQ
{
    public class MessageIdContainer : IInspectable
    {
        /// <summary>
        ///     Id of message.
        /// </summary>
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Id { get; set; }

        /// <summary>
        ///     When message is reserved this property stores actual reservation_id. This id can be used
        ///     for deleting the message, touching or releasing.
        /// </summary>
        [JsonProperty("reservation_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ReservationId { get; set; }

        [JsonProperty("subscriber_name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string SubscriberName { get; set; }
    }
}