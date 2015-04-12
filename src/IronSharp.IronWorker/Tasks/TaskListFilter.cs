using System;
using IronIO.Core;

namespace IronSharp.IronWorker
{
    public class TaskListFilter : PagingFilter
    {
        /// <summary>
        /// Limit the retrieved tasks to only those that were created after the time specified in the value. Time should be formatted as the number of seconds since
        /// the Unix epoch.
        /// </summary>
        public DateTime? FromTime { get; set; }

        /// <summary>
        /// Limit the retrieved tasks to only those that were created before the time specified in the value. Time should be formatted as the number of seconds since the
        /// Unix epoch.
        /// </summary>
        public DateTime? ToTime { get; set; }

        /// <summary>
        /// the parameters queued, running, complete, error, cancelled, killed, and timeout will all filter by their respective status when given a value of 1.
        /// These parameters can be mixed and matched to return tasks that fall into any of the status filters.
        /// If no filters are provided, tasks will be displayed across all statuses.
        /// </summary>
        public TaskStates Status { get; set; }
    }
}