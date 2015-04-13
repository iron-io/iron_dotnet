using System.Net.Http;
using System.Threading.Tasks;

namespace IronIO.Core
{
    public class IronTaskThatReturnsResponse : IronTask<HttpResponseMessage>
    {
        public IronTaskThatReturnsResponse(IronTaskRequestBuilder taskBuilder) : base(taskBuilder)
        {
        }

        protected override Task<HttpResponseMessage> ReadResultAsync(HttpResponseMessage response)
        {
            return Task.FromResult(response);
        }

        protected override HttpResponseMessage ReadResultSync(HttpResponseMessage response)
        {
            return response;
        }
    }
}