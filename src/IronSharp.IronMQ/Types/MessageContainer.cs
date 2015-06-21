using Newtonsoft.Json;

namespace IronIO.IronMQ
{
    public class MessageContainer
    {
        [JsonProperty("message", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public QueueMessage Message { get; set; }
    }
}