using System;
using System.Threading;
using System.Threading.Tasks;
using IronIO.Core;

namespace IronSharp.IronCache
{
    public class IronTaskThatGetsOrSetsCacheItem : IronTaskThatReturnsCacheItem
    {
        private readonly CacheClient _cacheClient;
        private readonly string _key;
        private readonly Func<CacheItem> _valueFactory;

        public IronTaskThatGetsOrSetsCacheItem(IronTaskRequestBuilder taskBuilder, CacheClient cacheClient, string key, Func<CacheItem> valueFactory)
            : base(taskBuilder, cacheClient)
        {
            _cacheClient = cacheClient;
            _key = key;
            _valueFactory = valueFactory;
        }

        public override CacheItem Send()
        {
            var result = base.Send();
            if (CacheItem.IsDefaultValue(result))
            {
                result = _valueFactory();
                _cacheClient.Put(_key, result).Send();
            }
            result.Client = _cacheClient;
            return result;
        }

        public new async Task<CacheItem> SendAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await base.SendAsync(cancellationToken);
            if (CacheItem.IsDefaultValue(result))
            {
                CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                result = _valueFactory();
                await _cacheClient.Put(_key, result).SendAsync(cts.Token);
            }
            result.Client = _cacheClient;
            return result;
        }
    }

    public class IronTaskThatGetsOrSetsCacheItem<TValue> : IronTaskThatReturnsCacheItem<CacheItem<TValue>>, IIronTask<TValue>
    {
        private readonly CacheClient _cacheClient;
        private readonly string _key;
        private readonly Func<TValue> _valueFactory;
        private readonly CacheItemOptions _options;

        public IronTaskThatGetsOrSetsCacheItem(IronTaskRequestBuilder taskBuilder, CacheClient cacheClient, string key, Func<TValue> valueFactory, CacheItemOptions options = null)
            : base(taskBuilder)
        {
            _cacheClient = cacheClient;
            _key = key;
            _valueFactory = valueFactory;
            _options = options;
        }

        public new TValue Send()
        {
            var result = base.Send();
            if (CacheItem.IsDefaultValue(result))
            {
                TValue value = _valueFactory();
                _cacheClient.Put(_key, value, _options).Send();
                return value;
            }
            return result.ReadValueAs();
        }

        public new async Task<TValue> SendAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await base.SendAsync(cancellationToken);
            if (CacheItem.IsDefaultValue(result))
            {
                CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                TValue value = _valueFactory();
                await _cacheClient.Put(_key, value, _options).SendAsync(cts.Token);
                return value;
            }
            return result.ReadValueAs();
        }
    }
}