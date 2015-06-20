using System.Collections.Generic;
using System.Linq;
using System.Threading;
using IronIO.Core;
using Newtonsoft.Json;

namespace IronIO.IronWorker
{
    public class TaskInfoCollection : IInspectable, IIdCollection
    {
        private List<TaskInfo> _tasks;

        [JsonIgnore]
        public bool IsEmpty
        {
            get { return _tasks == null || _tasks.Count == 0; }
        }

        [JsonProperty("tasks")]
        public List<TaskInfo> Tasks
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
    }
}