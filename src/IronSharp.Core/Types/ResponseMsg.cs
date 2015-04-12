using Newtonsoft.Json;

namespace IronIO.Core
{
    public class ResponseMsg : IMsg
    {
        [JsonProperty("msg")]
        public string Message { get; set; }
    }
}