using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IronSharp.IronMQ
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AlertType
    {
        [EnumMember(Value = "fixed")]
        Fixed,
        [EnumMember(Value = "progressive")]
        Progressive
    }
}
