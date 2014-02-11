using System.Threading;
using Newtonsoft.Json;

namespace IronSharp.Core
{
    public class IronSharpConfig : IInspectable
    {
        private IValueSerializer _valueSerializer;

        [JsonProperty("back_off_factor")]
        public double BackoffFactor { get; set; }

        [JsonIgnore]
        public IValueSerializer ValueSerializer
        {
            get { return LazyInitializer.EnsureInitialized(ref _valueSerializer, () => new DefaultValueSerializer()); }
            set { _valueSerializer = value; }
        }
    }
}