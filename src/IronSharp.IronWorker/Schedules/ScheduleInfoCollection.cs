using System.Collections.Generic;
using System.Linq;
using System.Threading;
using IronIO.Core;
using Newtonsoft.Json;

namespace IronSharp.IronWorker
{
    public class ScheduleInfoCollection : IInspectable, IIdCollection
    {
        private List<ScheduleInfo> _schedules;

        [JsonProperty("schedules")]
        public List<ScheduleInfo> Schedules
        {
            get { return LazyInitializer.EnsureInitialized(ref _schedules); }
            set { _schedules = value; }
        }

        /// <summary>
        /// Returns a list of IDs
        /// </summary>
        public IEnumerable<string> GetIds()
        {
            return Schedules.Select(x => x.Id);
        }
    }
}