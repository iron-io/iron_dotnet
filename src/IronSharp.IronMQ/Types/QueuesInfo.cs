using System.Collections.Generic;
using IronIO.Core;
using Newtonsoft.Json;

namespace IronIO.IronMQ
{
    public class QueuesInfo : IInspectable
    {
        [JsonProperty("queues", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<QueueInfo> Queues { get; set; }
    }
}