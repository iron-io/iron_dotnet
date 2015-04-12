using System.Threading;
using System.Threading.Tasks;

namespace IronSharp.Core
{
    public class IronTaskThatReturnsAnExpectedResult : IronTask<ResponseMsg>, IIronTask<bool>
    {
        private readonly string _expectedResultMessage;

        public IronTaskThatReturnsAnExpectedResult(IronTaskRequestBuilder taskBuilder, string expectedResultMessage)
            : base(taskBuilder)
        {
            _expectedResultMessage = expectedResultMessage;
        }

        public new bool Send()
        {
            return IsExpectedResult(base.Send());
        }

        public new async Task<bool> SendAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return IsExpectedResult(await base.SendAsync(cancellationToken));
        }

        private bool IsExpectedResult(IMsg result)
        {
            return result.HasExpectedMessage(_expectedResultMessage);
        }
    }
}