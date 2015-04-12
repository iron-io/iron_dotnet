using IronIO.Core;
using IronIO.Core.Extensions;
using Newtonsoft.Json;

namespace IronSharp.IronWorker
{
    public class TaskWebhookResponse : IMsg, IInspectable
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonIgnore]
        public bool Success
        {
            get { return this.HasExpectedMessage("Queued up."); }
        }

        [JsonProperty("msg", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Message { get; set; }

        public static implicit operator bool(TaskWebhookResponse collection)
        {
            return collection.Success;
        }
    }
}