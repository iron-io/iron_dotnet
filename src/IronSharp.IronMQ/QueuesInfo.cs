using System;
using System.Collections.Generic;
using System.Threading;
using IronIO.Core;
using IronSharp.Core;
using Newtonsoft.Json;

namespace IronSharp.IronMQ
{
    public class QueuesInfo : IInspectable
    {
        [JsonProperty("queues", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private List<QueueInfo> _queues;

        public List<QueueInfo> Queues
        {
            get { return _queues; }
        }
    }
}
