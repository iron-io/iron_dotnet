namespace IronIO.IronMQ
{
    public class QueueMessageContext<T>
    {
        public QueueMessage Message { get; set; }

        public QueueClient<T> Client { get; set; }

        public bool Success { get; set; }
    }
}