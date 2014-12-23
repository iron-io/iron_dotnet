using Newtonsoft.Json;

namespace IronSharp.IronWorker
{
    public class SchedulePayload : ScheduleOptions
    {
        public SchedulePayload()
        {
        }

        public SchedulePayload(string codeName, string payload, ScheduleOptions options = null)
        {
            CodeName = codeName;
            Payload = payload;

            if (options == null) return;

            EndAt = options.EndAt;
            Priority = options.Priority;
            RunEvery = options.RunEvery;
            StartAt = options.StartAt;
            Cluster = options.Cluster;
            Label = options.Label;
        }

        /// <summary>
        /// The name of the code package to execute.
        /// </summary>
        [JsonProperty("code_name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string CodeName { get; set; }

        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// A string of data to pass to the code package on execution.
        /// </summary>
        [JsonProperty("payload", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Payload { get; set; }
    }
}