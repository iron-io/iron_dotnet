using System.Collections.Generic;
using System.Threading;
using IronIO.Core;
using Newtonsoft.Json;

namespace IronSharp.IronWorker
{
    public class TaskPayloadCollection : IInspectable
    {
        private List<TaskPayload> _schedules;

        public TaskPayloadCollection(TaskPayload payload)
        {
            Tasks.Add(payload);
        }

        public TaskPayloadCollection(IEnumerable<TaskPayload> payloads)
        {
            Tasks.AddRange(payloads);
        }

        public TaskPayloadCollection(string codeName, string payload, TaskOptions options = null)
        {
            Tasks.Add(new TaskPayload(codeName, payload, options));
        }

        public TaskPayloadCollection(string codeName, IEnumerable<string> payloads, TaskOptions options = null)
        {
            foreach (string payload in payloads)
            {
                Tasks.Add(new TaskPayload(codeName, payload, options));
            }
        }

        [JsonProperty("tasks")]
        public List<TaskPayload> Tasks
        {
            get { return LazyInitializer.EnsureInitialized(ref _schedules); }
            set { _schedules = value; }
        }
    }
}