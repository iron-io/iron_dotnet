using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IronSharp.IronMQ
{
    [JsonConverter(typeof (StringEnumConverter))]
    public enum AlertDirection
    {
        [EnumMember(Value = "asc")]
        Asc,

        [EnumMember(Value = "desc")]
        Desc
    }
}