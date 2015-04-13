using System.Threading;
using System.Threading.Tasks;
using IronIO.Core;

namespace IronIO.IronCache
{
    public class IronTaskThatReturnsCacheItem<TValue> : IronTaskThatReturnsJson<CacheItem<TValue>>, IIronTask<TValue>
    {
        private readonly CacheClient _cacheClient;

        public IronTaskThatReturnsCacheItem(IronTaskRequestBuilder taskBuilder, CacheClient cacheClient)
            : base(taskBuilder)
        {
            _cacheClient = cacheClient;
        }

        public new TValue Send()
        {
            var result = base.Send();
            
            if (CacheItem.IsDefaultValue(result))
            {
                return default(TValue);
            }
            
            result.Client = _cacheClient;
            
            return result.ReadValueAs();
        }

        public new async Task<TValue> SendAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await base.SendAsync(cancellationToken);
            
            if (CacheItem.IsDefaultValue(result))
            {
                return default(TValue);
            }
            
            result.Client = _cacheClient;
            
            return result.ReadValueAs();
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