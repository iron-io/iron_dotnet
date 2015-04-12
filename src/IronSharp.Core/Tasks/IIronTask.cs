using System.Threading;
using System.Threading.Tasks;

namespace IronSharp.Core
{
    public interface IIronTask<TResult> : IHideObjectMembers
    {
        /// <summary>
        /// Queues the iron.io task to run on a background worker.
        /// </summary>
        void FireAndForget(CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Sends the iron.io API method request synchronously.
        /// </summary>
        TResult Send();

        /// <summary>
        /// Sends the iron.io API method request asynchronously. Example: var result = await task.SendAsync();
        /// </summary>
        Task<TResult> SendAsync(CancellationToken cancellationToken = new CancellationToken());
    }
}