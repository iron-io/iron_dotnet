using System;
using System.Diagnostics.Contracts;
using System.Net.Http;
using IronIO.Core;

namespace IronSharp.IronCache
{
    public class CacheClient
    {
        private readonly string _cacheName;
        private readonly IronCacheRestClient _client;

        public CacheClient(IronCacheRestClient client, string cacheName)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (string.IsNullOrEmpty(cacheName)) throw new ArgumentNullException("cacheName");
            Contract.EndContractBlock();

            _client = client;
            _cacheName = cacheName;
        }

        public IValueSerializer ValueSerializer
        {
            get { return _client.EndPointConfig.Config.SharpConfig.ValueSerializer; }
        }

        /// <summary>
        ///     Delete all items in a cache. This cannot be undone.
        /// </summary>
        /// <remarks>
        ///     http://dev.iron.io/cache/reference/api/#clear_a_cache
        /// </remarks>
        public IIronTask<bool> Clear()
        {
            var builder = new IronTaskRequestBuilder(_client.EndPointConfig)
            {
                HttpMethod = HttpMethod.Post,
                Path = string.Format("{0}/clear", CacheNameEndPoint())
            };

            return new IronTaskThatReturnsAnExpectedResult(builder, "Deleted.");
        }

        public IIronTask<bool> Delete(string key)
        {
            var builder = new IronTaskRequestBuilder(_client.EndPointConfig)
            {
                HttpMethod = HttpMethod.Delete,
                Path = CacheItemEndPoint(key)
            };

            return new IronTaskThatReturnsAnExpectedResult(builder, "Deleted.");
        }

        /// <summary>
        ///     This call retrieves an item from the cache. The item will not be deleted.
        /// </summary>
        /// <param name="key"> The key the item is stored under in the cache. </param>
        /// <remarks>
        ///     http://dev.iron.io/cache/reference/api/#get_an_item_from_a_cache
        /// </remarks>
        public IIronTask<CacheItem> Get(string key)
        {
            var builder = new IronTaskRequestBuilder(_client.EndPointConfig)
            {
                HttpMethod = HttpMethod.Get,
                Path = CacheItemEndPoint(key)
            };

            return new IronTaskThatReturnsCacheItem(builder, this);
        }

        public IIronTask<T> Get<T>(string key)
        {
            var builder = new IronTaskRequestBuilder(_client.EndPointConfig)
            {
                HttpMethod = HttpMethod.Get,
                Path = CacheItemEndPoint(key)
            };

            return new IronTaskThatReturnsCacheItem<T>(builder);
        }

        public IIronTask<T> GetOrAdd<T>(string key, Func<T> valueFactory, CacheItemOptions options = null)
        {
            var builder = new IronTaskRequestBuilder(_client.EndPointConfig)
            {
                HttpMethod = HttpMethod.Get,
                Path = CacheItemEndPoint(key)
            };

            return new IronTaskThatGetsOrSetsCacheItem<T>(builder, this, key, valueFactory, options);
        }

        public IIronTask<CacheItem> GetOrAdd(string key, Func<CacheItem> valueFactory)
        {
            var builder = new IronTaskRequestBuilder(_client.EndPointConfig)
            {
                HttpMethod = HttpMethod.Get,
                Path = CacheItemEndPoint(key)
            };

            return new IronTaskThatGetsOrSetsCacheItem(builder, this, key, valueFactory);
        }

        /// <summary>
        ///     This call increments the numeric value of an item in the cache. The amount must be a number and attempting to
        ///     increment non-numeric values results in an error.
        ///     Negative amounts may be passed to decrement the value.
        ///     The increment is atomic, so concurrent increments will all be observed.
        /// </summary>
        /// <param name="key"> The key of the item to increment </param>
        /// <param name="amount"> The amount to increment the value, as an integer. If negative, the value will be decremented. </param>
        /// <remarks>
        ///     http://dev.iron.io/cache/reference/api/#increment_an_items_value
        /// </remarks>
        public IIronTask<bool> Increment(string key, int amount = 1)
        {
            var builder = new IronTaskRequestBuilder(_client.EndPointConfig)
            {
                HttpMethod = HttpMethod.Post,
                Path = string.Format("{0}/increment", CacheItemEndPoint(key)),
                HttpContent = new JsonContent(new {amount})
            };

            return new IronTaskThatReturnsAnExpectedResult<CacheIncrementResult>(builder, "Added");
        }

        /// <summary>
        ///     This call gets general information about a cache.
        /// </summary>
        /// <remarks>
        ///     http://dev.iron.io/cache/reference/api/#get_info_about_a_cache
        /// </remarks>
        public IIronTask<CacheInfo> Info()
        {
            var builder = new IronTaskRequestBuilder(_client.EndPointConfig)
            {
                HttpMethod = HttpMethod.Get,
                Path = CacheNameEndPoint()
            };

            return new IronTaskThatReturnsJson<CacheInfo>(builder);
        }

        public IIronTask<bool> Put(string key, object value, CacheItemOptions options = null)
        {
            return Put(key, ValueSerializer.Generate(value), options);
        }

        public IIronTask<bool> Put(string key, int value, CacheItemOptions options = null)
        {
            return Put(key, new CacheItem(value, options));
        }

        public IIronTask<bool> Put(string key, string value, CacheItemOptions options = null)
        {
            return Put(key, new CacheItem(value, options));
        }

        /// <summary>
        ///     This call puts an item into a cache.
        /// </summary>
        /// <param name="key"> The key to store the item under in the cache. </param>
        /// <param name="item"> The item’s data </param>
        /// <remarks>
        ///     http://dev.iron.io/cache/reference/api/#put_an_item_into_a_cache
        /// </remarks>
        public IIronTask<bool> Put(string key, CacheItem item)
        {
            var builder = new IronTaskRequestBuilder(_client.EndPointConfig)
            {
                HttpMethod = HttpMethod.Put,
                Path = string.Format("{0}/increment", CacheItemEndPoint(key)),
                HttpContent = new JsonContent(item)
            };

            return new IronTaskThatReturnsAnExpectedResult(builder, "Stored.");
        }

        private string CacheItemEndPoint(string key)
        {
            return string.Format("{0}/items/{1}", CacheNameEndPoint(), key);
        }

        private string CacheNameEndPoint()
        {
            return string.Format("{0}/{1}", _client.EndPoint, _cacheName);
        }
    }
}