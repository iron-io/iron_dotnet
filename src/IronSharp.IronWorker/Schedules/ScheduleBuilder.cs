using System;

namespace IronSharp.IronWorker
{
    public static class ScheduleBuilder
    {
        public static readonly TimeSpan Month = TimeSpan.FromDays(30.4368);

        public static readonly TimeSpan Week = TimeSpan.FromDays(7);

        /// <summary>
        /// Creates a schedule builder instance
        /// </summary>
        /// <param name="now"> The current date and time relative the timezone setting under the account in the HUD on Iron.io (defaults to current local time) </param>
        public static ScheduleOptionsBuilder Build(DateTime? now = null)
        {
            return new ScheduleOptionsBuilder(now.GetValueOrDefault(DateTime.Now));
        }
    }
}