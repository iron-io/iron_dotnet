using System.Collections.Generic;
using System.Linq;
using System.Threading;
using IronIO.Core;
using Newtonsoft.Json;

namespace IronIO.IronMQ
{
    public class ReservedMessageIdCollection : IInspectable
    {
        private List<MessageIdContainer> _ids;

        public ReservedMessageIdCollection(IEnumerable<string> messageIds)
        {
            _ids = new List<MessageIdContainer>();
            _ids.AddRange(messageIds.Select(id => new MessageIdContainer { Id = id }));
        }
        public ReservedMessageIdCollection(MessageCollection collection)
        {
            _ids = new List<MessageIdContainer>();
            _ids.AddRange(collection.Messages.ConvertAll(m => new MessageIdContainer { Id = m.Id, ReservationId = m.ReservationId, SubscriberName = m.SubscriberName}));
        }

        [JsonProperty("ids", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<MessageIdContainer> Ids
        {
            get { return LazyInitializer.EnsureInitialized(ref _ids); }
            set { _ids = value; }
        }
    }
}