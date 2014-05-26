using System;
using Newtonsoft.Json;

namespace IronSharp.IronMQ
{
    public class QueueMessage : MessageOptions
    {
        public QueueMessage(string body, MessageOptions options = null)
            : this()
        {
            Body = body;

            if (options == null) return;

            Delay = options.Delay;
            ExpiresIn = options.ExpiresIn;
            Timeout = options.Timeout;
        }

        protected QueueMessage()
        {
        }

        #region Properties

        /// <summary>
        /// The message data
        /// </summary>
        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("push_status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public PushStatus PushStatus { get; set; }

        [JsonProperty("reserved_count", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? ReservedCount { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// This call will delete the message. Be sure you call this after you’re done with a message or it will be placed back on the queue.
        /// </summary>
        public bool Delete()
        {
            return Client.DeleteMessage(Id, ReservationId);
        }

        /// <summary>
        /// Releases this message and puts it back on the queue as if the message had timed out.
        /// </summary>
        /// <param name="delay">The item will not be available on the queue until this many seconds have passed. Default is 0 seconds. Maximum is 604,800 seconds (7 days).</param>
        /// <returns> </returns>
        public bool Release(int? delay = null)
        {
            return Client.Release(Id, ReservationId, delay);
        }

        /// <summary>
        /// Extends this message's timeout by the duration specified when the message was created, which is 60 seconds by default.
        /// </summary>
        public bool Touch()
        {
            return Client.Touch(Id, ReservationId);
        }

        #endregion

        [JsonIgnore]
        internal QueueClient Client { get; set; }

        public static implicit operator QueueMessage(string message)
        {
            return new QueueMessage(message);
        }

        public object ReadValueAs(Type type)
        {
            return Client.ValueSerializer.Parse(Body, type);
        }

        public T ReadValueAs<T>()
        {
            return Client.ValueSerializer.Parse<T>(Body);
        }
    }
}