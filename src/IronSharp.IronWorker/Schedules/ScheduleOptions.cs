using System;
using Newtonsoft.Json;

namespace IronSharp.IronWorker
{
    public class ScheduleOptions : PriorityOption
    {
        private int? _runEvery;

        /// <summary>
        /// The amount of time, in seconds, between runs. By default, the task will only run once. run_every will return a 400 error if it is set to less than 60.
        /// </summary>
        [JsonProperty("run_every", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? RunEvery
        {
            get { return _runEvery; }
            set
            {
                if (value.HasValue && value < 60)
                {
                    value = 60;
                }
                _runEvery = value;
            }
        }

        /// <summary>
        /// The number of times a task will run.
        /// </summary>
        [JsonProperty("run_times", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? RunTimes { get; set; }

        /// <summary>
        /// The time tasks will stop being queued. Should be a time or datetime.
        /// </summary>
        [JsonProperty("end_at", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime? EndAt { get; set; }

        /// <summary>
        /// The time the scheduled task should first be run.
        /// </summary>
        [JsonProperty("start_at", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime? StartAt { get; set; }

        /// <summary>
        /// Optional text label for the task. 
        /// </summary>
        [JsonProperty("label", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public String Label { get; set; }

        /// <summary>
        /// The cluster name ex. "high-mem" or "dedicated". 
        /// </summary>
        [JsonProperty("cluster", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public String Cluster { get; set; } 
    }
}