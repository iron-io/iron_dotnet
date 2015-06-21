using System.Net.Http;
using System.Threading;
using IronIO.Core;
using IronIO.Core.Attributes;
using IronIO.Core.Extensions;

namespace IronSharp.IronMQ
{
    public class IronMqRestClient
    {
        internal IronMqRestClient(IronClientConfig config)
        {
            LazyInitializer.EnsureInitialized(ref config);

            if (string.IsNullOrEmpty(config.Host))
            {
                config.Host = IronMqCloudHosts.DEFAULT;
            }

            config.ApiVersion = config.ApiVersion.GetValueOrDefault(3);

            ITokenContainer tokenContainer = null;

            if (config.Keystone != null)
            {
                if (config.KeystoneKeysExist())
                {
                    tokenContainer = KeystoneContainer.GetOrCreateInstance(config.Keystone);
                }
                else
                {
                    throw new IronIOException("Keystone keys missing");
                }
            }

            EndpointConfig = new IronTaskEndpointConfig(config, tokenContainer);
        }

        public string EndPoint => "/projects/{Project ID}/queues";
        public IIronTaskEndpointConfig EndpointConfig { get; }

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
        public IIronTask<QueuesInfo> Queues(QueuesFilter filter = null)
        {
            var builder = new IronTaskRequestBuilder(EndpointConfig)
            {
                HttpMethod = HttpMethod.Get,
                Path = EndPoint
            };

            SetQueueFilters(filter, builder);

            return new IronTaskThatReturnsJson<QueuesInfo>(builder);
        }

        private static void SetQueueFilters(QueuesFilter filter, IronTaskRequestBuilder builder)
        {
            if (filter == null)
            {
                return;
            }

            if (filter.PerPage.HasValue)
            {
                builder.Query.Add("per_page", filter.PerPage);
            }

            if (!string.IsNullOrEmpty(filter.Previous))
            {
                builder.Query.Add("previous", filter.Previous);
            }

            if (!string.IsNullOrEmpty(filter.Prefix))
            {
                builder.Query.Add("prefix", filter.Prefix);
            }
        }
    }
}