using System;
using System.Net.Http;
using System.Threading;
using IronIO.Core;

namespace IronIO.IronCache
{
    public class IronCacheRestClient
    {
        internal IronCacheRestClient(IronClientConfig config)
        {
            LazyInitializer.EnsureInitialized(ref config);

            if (string.IsNullOrEmpty(config.Host))
            {
                config.Host = IronCacheCloudHosts.DEFAULT;
            }

            config.ApiVersion = config.ApiVersion.GetValueOrDefault(1);

            EndpointConfig = new IronTaskEndpointConfig(config);
        }

        public IIronTaskEndpointConfig EndpointConfig { get; }

        public string ProjectPath => "/projects/{Project ID}/caches";

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
            var builder = new IronTaskRequestBuilder(EndpointConfig)
            {
                HttpMethod = HttpMethod.Delete,
                Path = $"{ProjectPath}/{cacheName}"
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CC0021:You should use nameof instead of program element name string")]
        public IIronTask<CacheInfo[]> List(int? page)
        {
            var builder = new IronTaskRequestBuilder(EndpointConfig)
            {
                HttpMethod = HttpMethod.Get,
                Path = ProjectPath
            };

            if (page.HasValue)
            {
                builder.Query["page"] = Convert.ToString(page);
            }

            return new IronTaskThatReturnsJson<CacheInfo[]>(builder);
        }
    }
}