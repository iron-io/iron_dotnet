﻿using System.Collections.Generic;
using System.Threading;
using IronIO.Core;
using Newtonsoft.Json;

namespace IronIO.IronMQ
{
    public class MessageCollection : IInspectable
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

        public MessageCollection(string message, MessageOptions options = null)
        {
            Messages.Add(new QueueMessage(message, options));
        }

        public MessageCollection(IEnumerable<string> messages, MessageOptions options = null)
        {
            foreach (var message in messages)
            {
                Messages.Add(new QueueMessage(message, options));
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