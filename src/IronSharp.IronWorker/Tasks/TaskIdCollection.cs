using System.Collections.Generic;
using System.Linq;
using System.Threading;
using IronIO.Core;
using IronIO.Core.Extensions;
using Newtonsoft.Json;

namespace IronSharp.IronWorker
{
    public class TaskIdCollection : IMsg, IInspectable, IIdCollection
    {
        private List<TaskId> _tasks;

        [JsonIgnore]
        public bool Success
        {
            get { return this.HasExpectedMessage("Queued up"); }
        }

        [JsonProperty("tasks")]
        public List<TaskId> Tasks
        {
            get { return LazyInitializer.EnsureInitialized(ref _tasks); }
            set { _tasks = value; }
        }

        /// <summary>
        /// Returns a list of IDs
        /// </summary>
        public IEnumerable<string> GetIds()
        {
            return Tasks.Select(x => x.Id);
        }

        [JsonProperty("msg", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Message { get; set; }

        public static implicit operator bool(TaskIdCollection collection)
        {
            return collection.Success;
        }
    }
}