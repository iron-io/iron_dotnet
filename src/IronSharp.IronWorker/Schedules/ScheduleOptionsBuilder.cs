using System;

namespace IronIO.IronWorker
{
    public class ScheduleOptionsBuilder : ScheduleOptions
    {
        public ScheduleOptionsBuilder(DateTime now)
        {
            Now = now;
        }

        public DateTime Now { get; set; }

        public ScheduleOptionsBuilder Delay(TimeSpan delay)
        {
            return StartingOn(Now + delay);
        }

        public ScheduleOptionsBuilder StopAt(DateTime endAt)
        {
            EndAt = endAt;
            return this;
        }

        public ScheduleOptionsBuilder NeverStop()
        {
            EndAt = null;
            RunTimes = null;
            return this;
        }

        public ScheduleOptionsBuilder StopAfterNumberOfRuns(int times)
        {
            RunTimes = times;
            return this;
        }

        public ScheduleOptionsBuilder RunFor(TimeSpan duration)
        {
            return StopAt(Now + duration);
        }

        public ScheduleOptionsBuilder StartingOn(DateTime startAt)
        {
            StartAt = startAt;
            return this;
        }

        public ScheduleOptionsBuilder WithFrequency(TimeSpan frequency)
        {
            RunEvery = frequency.Seconds;
            return this;
        }

        public ScheduleOptionsBuilder WithPriority(TaskPriority priority)
        {
            Priority = priority;
            return this;
        }
    }
}