using System;
using System.Net.Http;
using System.Threading;
using IronIO.Core;

namespace IronSharp.IronCache
{
    public class IronCacheRestClient
    {
        private readonly IIronTaskEndPointConfig _endPointConfig;

        internal IronCacheRestClient(IronClientConfig config)
        {
            LazyInitializer.EnsureInitialized(ref config);

            if (string.IsNullOrEmpty(config.Host))
            {
                config.Host = IronCacheCloudHosts.DEFAULT;
            }

            if (config.ApiVersion == default (int))
            {
                config.ApiVersion = 1;
            }

            _endPointConfig = new IronTaskEndPointConfig(config);
        }

        public IIronTaskEndPointConfig EndPointConfig
        {
            get { return _endPointConfig; }
        }

        public string EndPoint
        {
            get { return "/projects/{Project ID}/caches"; }
        }

        public CacheClient Cache(string cacheName)
        {
            return new CacheClient(this, cacheName);
        }

        /// <summary>
        ///     Delete a cache and all items in it.
        /// </summary>
        /// <param name="cacheName"> The name of the cache </param>
        /// <remarks>
        ///     http://dev.iron.io/cache/reference/api/#delete_a_cache
        /// </remarks>
        public IIronTask<bool> Delete(string cacheName)
        {
            var builder = new IronTaskRequestBuilder(_endPointConfig)
            {
                HttpMethod = HttpMethod.Delete,
                Path = string.Format("{0}/{1}", EndPoint, cacheName)
            };

            return new IronTaskThatReturnsAnExpectedResult(builder, "Deleted.");
        }

        /// <summary>
        ///     Get a list of all caches in a project. 100 caches are listed at a time. To see more, use the page parameter.
        /// </summary>
        /// <param name="page"> The current page </param>
        /// <remarks>
        ///     http://dev.iron.io/cache/reference/api/#list_caches
        /// </remarks>
        public IIronTask<CacheInfo[]> List(int? page)
        {
            var builder = new IronTaskRequestBuilder(_endPointConfig)
            {
                HttpMethod = HttpMethod.Get,
                Path = EndPoint
            };

            if (page.HasValue)
            {
                builder.Query["page"] = Convert.ToString(page);
            }

            return new IronTaskThatReturnsJson<CacheInfo[]>(builder);
        }
    }
}