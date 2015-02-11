using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IronSharp.IronMQ
{
    public class MessageContainer
    {
        [JsonProperty("message", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public QueueMessage Message { get; set; }
    }
}
