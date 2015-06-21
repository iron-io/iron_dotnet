using System.Collections.Generic;
using System.Threading;
using IronIO.Core;
using Newtonsoft.Json;

namespace IronIO.IronMQ
{
    public class AlertCollection : IInspectable
    {
        [JsonProperty("alerts", DefaultValueHandling = DefaultValueHandling.Ignore)] private List<Alert> _alerts;

        public AlertCollection()
        {
        }

        public AlertCollection(Alert alert) : this(new[] {alert})
        {
        }

        public AlertCollection(IEnumerable<Alert> alerts)
        {
            foreach (var alert in alerts)
            {
                Alerts.Add(alert);
            }
        }

        [JsonIgnore]
        public List<Alert> Alerts
        {
            get { return LazyInitializer.EnsureInitialized(ref _alerts); }
            set { _alerts = value; }
        }
    }
}