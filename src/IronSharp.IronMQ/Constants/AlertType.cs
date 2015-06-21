using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IronIO.IronMQ
{
    [JsonConverter(typeof (StringEnumConverter))]
    public enum AlertType
    {
        [EnumMember(Value = "fixed")]
        Fixed,

        [EnumMember(Value = "progressive")]
        Progressive
    }
}