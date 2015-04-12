using System;
using IronIO.Core;
using IronIO.Core.Extensions;
using Newtonsoft.Json;

namespace IronSharp.IronWorker
{
    public class TaskInfo : IMsg, IInspectable
    {
        [JsonProperty("code_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string CodeId { get; set; }

        [JsonProperty("code_name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string CodeName { get; set; }

        [JsonProperty("created_at", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("duration", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? Duration { get; set; }

        [JsonProperty("end_time", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime? EndTime { get; set; }

        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("schedule_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ScheduleId { get; set; }

        [JsonProperty("msg", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty("percent", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? Percent { get; set; }

        [JsonProperty("project_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ProjectId { get; set; }

        [JsonProperty("run_times", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? RunTimes { get; set; }

        [JsonProperty("start_time", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime? StartTime { get; set; }

        [JsonIgnore]
        public TaskStates Status {
            get { return StatusValue.As<TaskStates>(); }
            set { StatusValue = Convert.ToString(value).ToLower(); }
        }

        [JsonProperty("timeout", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? Timeout { get; set; }

        [JsonProperty("updated_at", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        protected string StatusValue { get; set; }
    }
}