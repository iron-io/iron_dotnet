using System;

namespace IronSharp.IronWorker
{
    [Flags]
    public enum TaskStates
    {
        /// <summary>
        /// No state information
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// in the queue, waiting to run
        /// </summary>
        Queued = 0x0001,

        /// <summary>
        /// running
        /// </summary>
        Running = 0x0002,

        /// <summary>
        /// finished running
        /// </summary>
        Complete = 0x0004,

        /// <summary>
        /// error during processing
        /// </summary>
        Error = 0x0008,

        /// <summary>
        /// cancelled by user
        /// </summary>
        Cancelled = 0x0010,

        /// <summary>
        /// killed by system
        /// </summary>
        Killed = 0x0020,

        /// <summary>
        /// exceeded processing time threshold
        /// </summary>
        Timeout = 0x0040
    }
}