using System;
using Newtonsoft.Json;

namespace IronSharp.IronWorker
{
    /// <summary>
    /// http://dev.iron.io/worker/reference/api/#schedule_a_task
    /// </summary>
    public class TaskPayload : TaskOptions
    {
        public TaskPayload()
        {
        }

        public TaskPayload(string codeName, string payload, TaskOptions options = null)
        {
            CodeName = codeName;
            Payload = payload;

            if (options == null) return;

            Timeout = options.Timeout;
            Delay = options.Delay;
            Priority = options.Priority;
            Label = options.Label;
            Cluster = options.Cluster;
        }

        /// <summary>
        /// The name of the code package to execute.
        /// </summary>
        [JsonProperty("code_name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string CodeName { get; set; }

        /// <summary>
        ///  The time tasks will stop being queued. Should be a time or datetime.
        /// </summary>
        [JsonProperty("end_at", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime? EndAt { get; set; }

        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// A string of data to pass to the code package on execution.
        /// </summary>
        [JsonProperty("payload", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Payload { get; set; }

        /// <summary>
        /// The amount of time, in seconds, between runs.
        /// By default, the task will only run once.
        /// run_every will return a 400 error if it is set to less than 60.
        /// </summary>
        [JsonProperty("run_every", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? RunEvery { get; set; }

        /// <summary>
        /// The number of times a task will run.
        /// </summary>
        [JsonProperty("run_times", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? RunTimes { get; set; }

        /// <summary>
        /// The time the scheduled task should first be run.
        /// </summary>
        [JsonProperty("start_at", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime? StartAt { get; set; }
    }
}