using System.Threading;
using System.Threading.Tasks;
using IronIO.Core;

namespace IronSharp.IronCache
{
    public class IronTaskThatReturnsCacheItem<TValue> : IronTaskThatReturnsJson<CacheItem<TValue>>, IIronTask<TValue>
    {
        public IronTaskThatReturnsCacheItem(IronTaskRequestBuilder taskBuilder)
            : base(taskBuilder)
        {

        }

        public new TValue Send()
        {
            var result = base.Send();
            return CacheItem.IsDefaultValue(result) ? default(TValue) : result.ReadValueAs();
        }

        public new async Task<TValue> SendAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await base.SendAsync(cancellationToken);
            return CacheItem.IsDefaultValue(result) ? default(TValue) : result.ReadValueAs();
        }
    }

    public class IronTaskThatReturnsCacheItem : IronTaskThatReturnsJson<CacheItem>
    {
        private readonly CacheClient _cacheClient;

        public IronTaskThatReturnsCacheItem(IronTaskRequestBuilder taskBuilder, CacheClient cacheClient)
            : base(taskBuilder)
        {
            _cacheClient = cacheClient;
        }

        protected override CacheItem InspectResultAndReturn(CacheItem result)
        {
            if (result != null)
            {
                result.Client = _cacheClient;
            }
            return result;
        }
    }
}