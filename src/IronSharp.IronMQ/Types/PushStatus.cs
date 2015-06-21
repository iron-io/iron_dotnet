using IronIO.Core;
using Newtonsoft.Json;

namespace IronIO.IronMQ
{
    public class PushStatus : IInspectable
    {
        [JsonProperty("retries_remaining", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? RetriesRemaining { get; set; }
    }
}