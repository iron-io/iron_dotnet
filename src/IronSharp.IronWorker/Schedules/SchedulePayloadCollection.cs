using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;

namespace IronSharp.IronWorker
{
    public class SchedulePayloadCollection
    {
        private List<SchedulePayload> _schedules;

        public SchedulePayloadCollection(SchedulePayload payload)
        {
            Schedules.Add(payload);
        }

        public SchedulePayloadCollection(string codeName, string payload, ScheduleOptions options = null)
        {
            Schedules.Add(new SchedulePayload(codeName, payload, options));
        }

        [JsonProperty("schedules")]
        public List<SchedulePayload> Schedules
        {
            get { return LazyInitializer.EnsureInitialized(ref _schedules); }
            set { _schedules = value; }
        }
    }
}