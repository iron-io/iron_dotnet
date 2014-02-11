using System.Collections.Generic;
using System.Threading;
using IronSharp.Core;
using Newtonsoft.Json;

namespace IronSharp.IronMQ
{
    public class MessageIdCollection : IMsg, IInspectable, IIdCollection
    {
        private List<string> _ids;

        public MessageIdCollection()
        {
        }

        public MessageIdCollection(IEnumerable<string> messageIds)
        {
            Ids.AddRange(messageIds);
        }

        [JsonProperty("ids")]
        public List<string> Ids
        {
            get { return LazyInitializer.EnsureInitialized(ref _ids); }
            set { _ids = value; }
        }

        [JsonProperty("msg", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonIgnore]
        public bool Success
        {
            get { return this.HasExpectedMessage("Messages put on queue."); }
        }

        public static implicit operator bool(MessageIdCollection collection)
        {
            return collection.Success;
        }

        IEnumerable<string> IIdCollection.GetIds()
        {
            return Ids;
        }
    }
}