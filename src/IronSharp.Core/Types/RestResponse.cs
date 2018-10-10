using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IronSharp.Core
{
    public class RestResponse<T> : IMsg where T : class
    {
        private T _result;

        public RestResponse(HttpResponseMessage responseMessage)
        {
            ResponseMessage = responseMessage;
        }

        public HttpContent Content
        {
            get { return ResponseMessage.Content; }
        }

        public HttpResponseMessage ResponseMessage { get; set; }

        public T Result
        {
            get
            {
                SetResult();
                return _result;
            }
        }

        string IMsg.Message
        {
            get
            {
                ResponseMsg msg = Msg();
                return msg == null ? null : msg.Message;
            }
        }

        public static implicit operator bool(RestResponse<T> value)
        {
            return value.ResponseMessage != null && value.ResponseMessage.IsSuccessStatusCode;
        }

        public static implicit operator T(RestResponse<T> value)
        {
            return value.Result;
        }

        public bool CanReadResult()
        {
            try
            {
                SetResult();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public ResponseMsg Msg()
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseMsg>(Content.ReadAsStringAsync().Result);
        }

        public async Task<T> ReadResultAsync()
        {
            var text = await Content.ReadAsStringAsync();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(text);
        }

        private void SetResult()
        {
            LazyInitializer.EnsureInitialized(ref _result, () => ReadResultAsync().Result);
        }
    }
}