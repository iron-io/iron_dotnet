using IronIO.Core;
using Newtonsoft.Json;

namespace IronIO.IronMQ
{
    public class QueueContainer : IInspectable
    {
        public QueueContainer(QueueInfo queue)
        {
            Queue = queue;
        }

        [JsonProperty("queue", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public QueueInfo Queue { get; set; }
    }
}