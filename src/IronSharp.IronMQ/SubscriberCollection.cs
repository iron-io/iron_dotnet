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

        public SubscriberCollection(string name, string url) : this(name, url, null)
        {
        }

        public SubscriberCollection(string name, Uri uri) : this(name, uri.ToString(), null)
        {
        }

        public SubscriberCollection(string name, string url, Dictionary<string, string> headers)
        {
            Subscribers.Add(new Subscriber(name, url, headers));
        }

        public SubscriberCollection(string name, Uri uri, Dictionary<string,string> headers)
            : this(name, uri.ToString(), headers)
        {
        }

        [JsonIgnore]
        public List<Subscriber> Subscribers
        {
            get { return LazyInitializer.EnsureInitialized(ref _subscribers); }
            set { _subscribers = value; }
        }
    }
}