using IronSharp.Core;
using Newtonsoft.Json;

namespace IronSharp.IronWorker
{
    public class TaskProgress : IMsg, IInspectable
    {
        /// <summary>
        /// An integer, between 0 and 100 inclusive, that describes the completion of the task.
        /// </summary>
        [JsonProperty("percent", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? Percent { get; set; }

        /// <summary>
        /// Any message or data describing the completion of the task. Must be a string value, and the 64KB request limit applies.
        /// </summary>
        [JsonProperty("msg", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Message { get; set; }
    }
}