using IronIO.Core;

namespace IronIO.IronMQ
{
    public class ReservationOptions
    {
        /// <summary>
        /// After timeout (in seconds), item will be placed back onto queue. You must delete the message from the queue to ensure it does not go back onto the queue. If not set, value
        /// from queue is used. Default is 60 seconds, minimum is 30 seconds, and maximum is 86,400 seconds (24 hours).
        /// </summary>
        public IronTimespan Timeout { get; set; }

        /// <summary>
        /// Time to long poll for messages, in seconds. Max is 30 seconds. Default 0.
        /// </summary>
        public IronTimespan Wait { get; set; }

        /// <summary>
        /// If true, do not put each message back on to the queue after reserving. Default false.
        /// </summary>
        public bool? Delete { get; set; }
    }
}