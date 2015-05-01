using System.Collections.Generic;
using System.Threading;
using IronSharp.Core;
using IronSharp.Core.Attributes;

namespace IronSharp.IronMQ
{
    public class IronMqRestClient
    {
        private readonly IronClientConfig _config;
        private readonly RestClient _restClient;

        internal IronMqRestClient(IronClientConfig config)
        {
            _restClient = new RestClient();
            _config = LazyInitializer.EnsureInitialized(ref config);

            if (string.IsNullOrEmpty(Config.Host))
            {
                Config.Host = IronMqCloudHosts.DEFAULT;
            }

            if (config.ApiVersion == default (int))
            {
                config.ApiVersion = 1;
            }
        }

        public IronClientConfig Config
        {
            get { return _config; }
        }

        public RestClient RestClient
        {
            get { return _restClient; }
        }

        public string EndPoint
        {
            get { return "/projects/{Project ID}/queues"; }
        }

        public QueueClient<T> Queue<T>()
        {
            return new QueueClient<T>(this, QueueNameAttribute.GetName<T>());
        }

        public QueueClient Queue(string name)
        {
            return new QueueClient(this, name);
        }

        /// <summary>
        /// Get a list of all queues in a project.
        /// By default, 30 queues are listed at a time.
        /// To see more, use the page parameter or the per_page parameter.
        /// Up to 100 queues may be listed on a single page.
        /// </summary>
        /// <param name="filter"> </param>
        /// <returns> </returns>
        public IEnumerable<QueueInfo> Queues(PagingFilter filter = null)
        {
            return _restClient.Get<IEnumerable<QueueInfo>>(_config, EndPoint, filter).Result;
        }
    }
}