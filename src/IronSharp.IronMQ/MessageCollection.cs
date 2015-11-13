using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;

namespace IronSharp.IronMQ
{
    public class MessageCollection
    {
        [JsonProperty("messages")] private List<QueueMessage> _messages;

        public MessageCollection()
        {
        }

        public MessageCollection(QueueMessage message)
        {
            Messages.Add(message);
        }

        public MessageCollection(IEnumerable<QueueMessage> messages)
        {
            Messages.AddRange(messages);
        }

        public MessageCollection(string message, int? delay = null)
        {
            Messages.Add(new QueueMessage(message, delay));
        }

        public MessageCollection(IEnumerable<string> messages, int? delay = null)
        {
            foreach (string message in messages)
            {
                Messages.Add(new QueueMessage(message, delay));
            }
        }

        [JsonIgnore]
        public List<QueueMessage> Messages
        {
            get { return LazyInitializer.EnsureInitialized(ref _messages); }
            set { _messages = value; }
        }

        public static implicit operator MessageCollection(string message)
        {
            return new MessageCollection(message);
        }
    }
}