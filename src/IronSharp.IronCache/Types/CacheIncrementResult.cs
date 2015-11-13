using IronIO.Core;
using IronIO.Core.Extensions;
using Newtonsoft.Json;

namespace IronIO.IronCache
{
    public class CacheIncrementResult : IMsg, IInspectable
    {
        public bool Success
        {
            get { return this.HasExpectedMessage("Added"); }
        }

        [JsonProperty("value", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? Value { get; set; }

        [JsonProperty("msg")]
        public string Message { get; set; }

        public static implicit operator bool(CacheIncrementResult value)
        {
            return value != null && value.Success;
        }
    }
}