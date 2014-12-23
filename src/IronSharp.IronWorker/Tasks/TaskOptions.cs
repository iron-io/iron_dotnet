using System;
using Newtonsoft.Json;

namespace IronSharp.IronWorker
{
    public class TaskOptions : PriorityOption
    {
        /// <summary>
        /// The maximum runtime of your task in seconds. No task can exceed 3600 seconds (60 minutes). The default is 3600 but can be set to a shorter duration.
        /// </summary>
        [JsonProperty("timeout", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? Timeout { get; set; }

        /// <summary>
        /// The number of seconds to delay before actually queuing the task. Default is 0.
        /// </summary>
        [JsonProperty("delay", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? Delay { get; set; }

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