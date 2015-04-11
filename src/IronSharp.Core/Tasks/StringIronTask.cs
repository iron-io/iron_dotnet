using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IronSharp.Core
{
    public class StringIronTask : IronTask<string>
    {
        public StringIronTask(HttpClient httpClient, HttpRequestMessage request, CancellationToken? cancellationToken = null)
            : base(httpClient, request, cancellationToken)
        {
        }

        protected override Task<string> ReadResultAsync(HttpResponseMessage response)
        {
            return response.Content.ReadAsStringAsync();
        }
    }
}