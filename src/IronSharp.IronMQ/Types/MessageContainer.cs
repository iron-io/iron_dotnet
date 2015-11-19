using IronIO.Core;
using Newtonsoft.Json;

namespace IronIO.IronMQ
{
    public class MessageContainer : IInspectable
    {
        [JsonProperty("message", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public QueueMessage Message { get; set; }
    }
}