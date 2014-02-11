using System;
using System.Collections.Generic;
using System.Threading;
using IronSharp.Core;
using Newtonsoft.Json;

namespace IronSharp.IronMQ
{
    public class SubscriberCollection : IInspectable
    {
        [JsonProperty("subscribers", DefaultValueHandling = DefaultValueHandling.Ignore)] private List<Subscriber> _subscribers;

        public SubscriberCollection()
        {
        }

        public SubscriberCollection(Uri subscriber) : this(new[] {subscriber})
        {
        }

        public SubscriberCollection(IEnumerable<Uri> subscribers)
        {
            foreach (Uri subscriber in subscribers)
            {
                Subscribers.Add(subscriber);
            }
        }

        public SubscriberCollection(string subscriber)
            : this(new[] {subscriber})
        {
        }

        public SubscriberCollection(IEnumerable<string> subscribers)
        {
            foreach (string subscriber in subscribers)
            {
                Subscribers.Add(subscriber);
            }
        }

        [JsonIgnore]
        public List<Subscriber> Subscribers
        {
            get { return LazyInitializer.EnsureInitialized(ref _subscribers); }
            set { _subscribers = value; }
        }

        public static implicit operator SubscriberCollection(string url)
        {
            return new SubscriberCollection(url);
        }

        public static implicit operator SubscriberCollection(Uri url)
        {
            return new SubscriberCollection(url);
        }
    }
}