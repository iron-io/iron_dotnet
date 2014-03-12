using System;
using System.Collections.Generic;
using System.Threading;
using IronSharp.Core;
using Newtonsoft.Json;

namespace IronSharp.IronMQ
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
            foreach (Alert alert in alerts)
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