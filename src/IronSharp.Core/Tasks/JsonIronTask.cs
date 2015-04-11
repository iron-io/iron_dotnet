using System.Net.Http;
using System.Threading;

namespace IronSharp.Core
{
    public class JsonIronTask<TResult> : IronTask<TResult>
    {
        public JsonIronTask(HttpClient httpClient, HttpRequestMessage request, CancellationToken? cancellationToken = null) : base(httpClient, request, cancellationToken)
        {
        }
    }
}