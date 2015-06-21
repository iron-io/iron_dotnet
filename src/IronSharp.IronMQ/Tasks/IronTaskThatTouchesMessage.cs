using IronIO.Core;

namespace IronIO.IronMQ
{
    public class IronTaskThatTouchesMessage : IronTaskThatReturnsJson<MessageOptions>
    {
        private readonly QueueMessage _message;

        public IronTaskThatTouchesMessage(IronTaskRequestBuilder taskBuilder, QueueMessage message) : base(taskBuilder)
        {
            _message = message;
        }

        protected override MessageOptions InspectResultAndReturn(MessageOptions result)
        {
            if (result != null)
            {
                _message.ReservationId = result.ReservationId;
            }
            return result;
        }
    }
}