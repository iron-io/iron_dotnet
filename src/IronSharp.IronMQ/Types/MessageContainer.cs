using Newtonsoft.Json;

namespace IronSharp.IronMQ
{
    public class MessageContainer
    {
        [JsonProperty("message", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public QueueMessage Message { get; set; }
    }
}