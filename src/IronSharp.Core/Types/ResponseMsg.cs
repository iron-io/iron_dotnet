using Newtonsoft.Json;

namespace IronSharp.Core
{
    public class ResponseMsg : IMsg
    {
        [JsonProperty("msg")]
        public string Message { get; set; }
    }
}