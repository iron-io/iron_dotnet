using Newtonsoft.Json;

namespace IronSharp.IronCache
{
    public class CacheInfo
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("project_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ProjectId { get; set; }

        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("size", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? Size { get; set; }

        [JsonProperty("data_size", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? DataSize { get; set; }
    }
}