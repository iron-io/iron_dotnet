using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IronSharp.Core
{
    public class ExpectedResultIronTask : IronTask<ResponseMsg>
    {
        private readonly string _expectedResultMessage;

        public ExpectedResultIronTask(string expectedResultMessage, HttpClient httpClient, HttpRequestMessage request,
            CancellationToken? cancellationToken = null) : base(httpClient, request, cancellationToken)
        {
            _expectedResultMessage = expectedResultMessage;
        }

        public new bool Send()
        {
            return IsExpectedResult(base.Send());
        }

        public new async Task<bool> SendAsync()
        {
            return IsExpectedResult(await base.SendAsync());
        }

        private bool IsExpectedResult(IMsg result)
        {
            return result.HasExpectedMessage(_expectedResultMessage);
        }
    }
}