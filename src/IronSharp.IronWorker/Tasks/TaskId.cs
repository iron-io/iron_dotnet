using IronSharp.Core;
using Newtonsoft.Json;

namespace IronSharp.IronWorker
{
    public class TaskId : IInspectable
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        public override string ToString()
        {
            return Id;
        }

        public static implicit operator string(TaskId taskId)
        {
            return taskId == null ? null : taskId.Id;
        }
    }
}