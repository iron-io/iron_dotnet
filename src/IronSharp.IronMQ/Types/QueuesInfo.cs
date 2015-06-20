using System.Collections.Generic;
using IronIO.Core;
using Newtonsoft.Json;

namespace IronSharp.IronMQ
{
    public class QueuesInfo : IInspectable
    {
        [JsonProperty("queues", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<QueueInfo> Queues { get; set; }
    }
}