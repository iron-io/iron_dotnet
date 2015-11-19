using System.Threading;
using System.Threading.Tasks;

namespace IronIO.Core
{
    /// <summary>
    /// A type of IIronTask for returning short-curcuited results that require no implementations.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class NoOpIronTaskResult<TResult> : IIronTask<TResult>
    {

        public NoOpIronTaskResult(TResult result)
        {
            Result = result;
        }
        public TResult Result { get; set; }
        
        public virtual void FireAndForget(CancellationToken cancellationToken = new CancellationToken())
        {
        }

        public virtual TResult Send()
        {
            return Result;
        }

        public Task<TResult> SendAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.FromResult(Result);
        }
    }
}