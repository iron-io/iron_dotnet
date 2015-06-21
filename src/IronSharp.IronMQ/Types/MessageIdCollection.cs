using System.Collections.Generic;
using System.Threading;
using IronIO.Core;
using IronIO.Core.Extensions;
using Newtonsoft.Json;

namespace IronIO.IronMQ
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

        [JsonIgnore]
        public bool Success => this.HasExpectedMessage("Messages put on queue.");

        IEnumerable<string> IIdCollection.GetIds()
        {
            return Ids;
        }

        [JsonProperty("msg", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Message { get; set; }

        public static implicit operator bool(MessageIdCollection collection)
        {
            return collection.Success;
        }
    }
}