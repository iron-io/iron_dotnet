using System.Net.Http;
using System.Threading.Tasks;

namespace IronSharp.Core
{
    public class IronTaskThatReturnsString : IronTask<string>
    {
        public IronTaskThatReturnsString(IronTaskRequestBuilder taskBuilder) : base(taskBuilder)
        {
        }

        protected override Task<string> ReadResultAsync(HttpResponseMessage response)
        {
            return response.Content.ReadAsStringAsync();
        }
    }
}