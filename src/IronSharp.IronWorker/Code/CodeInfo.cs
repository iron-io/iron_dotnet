using IronIO.Core;
using Newtonsoft.Json;

namespace IronSharp.IronWorker
{
    public class CodeInfo : IInspectable
    {
        [JsonProperty("file_name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string FileName { get; set; }

        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("latest_change", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string LatestChange { get; set; }

        [JsonProperty("latest_checksum", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string LatestChecksum { get; set; }

        [JsonProperty("latest_history_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string LatestHistoryId { get; set; }

        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("project_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ProjectId { get; set; }

        [JsonProperty("rev", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? Revision { get; set; }

        [JsonProperty("runtime", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Runtime { get; set; }
    }
}