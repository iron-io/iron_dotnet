using System.Net.Http;
using System.Threading;

namespace IronSharp.Core
{
    public class IronTaskThatReturnsJson<TResult> : IronTask<TResult>
    {
        public IronTaskThatReturnsJson(IronTaskRequestBuilder taskBuilder) : base(taskBuilder)
        {
        }
    }
}