using Newtonsoft.Json;

namespace IronIO.IronWorker
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
    }
}