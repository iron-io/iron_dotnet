using IronIO.Core;

namespace IronIO.IronMQ
{
    public class MessageReservationOptions : ReservationOptions, IInspectable
    {
        /// <summary>
        /// The maximum number of messages to get. Default is 1. Maximum is 100. Note: You may not receive all n messages on every request, the more sparse the queue, the less likely you are
        /// to receive all n messages.
        /// </summary>
        public int Number { get; set; }
    }
}