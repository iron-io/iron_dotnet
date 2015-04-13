using System;
using IronIO.Core;
using Newtonsoft.Json;

namespace IronIO.IronWorker
{
    public class PriorityOption : IInspectable
    {
        /// <summary>
        /// The priority queue to run the job in. Valid values are 0, 1, and 2. The default is 0. Higher values means tasks spend less time in the queue once they come off the schedule.
        /// </summary>
        [JsonIgnore]
        public TaskPriority Priority { get; set; }

        [JsonProperty("priority", DefaultValueHandling = DefaultValueHandling.Ignore)]
        protected int? PriorityValue
        {
            get
            {
                switch (Priority)
                {
                    case TaskPriority.Default:
                        return null;
                    case TaskPriority.Medium:
                        return 1;
                    case TaskPriority.High:
                        return 2;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set { Priority = (TaskPriority) value.GetValueOrDefault(); }
        }
    }
}