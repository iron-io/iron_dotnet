using System;
using IronIO.Core;
using Newtonsoft.Json;

namespace IronIO.IronWorker
{
    public class ScheduleInfo : IMsg, IInspectable
    {
        [JsonProperty("code_name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string CodeName { get; set; }

        [JsonProperty("created_at", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("run_times", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? RunTimes { get; set; }

        [JsonProperty("end_at", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime? EndAt { get; set; }

        [JsonProperty("start_at", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime? StartAt { get; set; }

        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("last_run_time", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime? LastRunTime { get; set; }

        [JsonProperty("next_start", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime? NextStart { get; set; }

        [JsonProperty("project_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ProjectId { get; set; }

        [JsonProperty("run_count", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? RunCount { get; set; }

        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Status { get; set; }

        [JsonProperty("updated_at", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty("msg", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Message { get; set; }
    }
}