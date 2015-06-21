using Newtonsoft.Json;

namespace IronIO.IronMQ
{
    public class PushStatus
    {
        [JsonProperty("retries_remaining", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? RetriesRemaining { get; set; }
    }
}