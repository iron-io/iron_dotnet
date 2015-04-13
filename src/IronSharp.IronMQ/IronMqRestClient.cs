using System.Collections.Generic;
using System.Threading;
using IronSharp.Core;
using System;
using System.Net.Http;
using IronIO.Core;
using IronIO.Core.Attributes;

namespace IronSharp.IronMQ
{
    public class IronMqRestClient
    {
        private readonly IIronTaskEndpointConfig _endpointConfig;

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

            _endpointConfig = new IronTaskEndpointConfig(config, tokenContainer);
        }



        public string EndPoint
        {
            get { return "/projects/{Project ID}/queues"; }
        }

        public IIronTaskEndpointConfig EndpointConfig
        {
            get { return _endpointConfig; }
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
        public IIronTask<QueuesInfo> Queues(MqPagingFilter filter = null)
        {
            var builder = new IronTaskRequestBuilder(_endpointConfig)
            {
                HttpMethod = HttpMethod.Get,
                Path = EndPoint
            };

            if (filter != null)
            {
                builder.Query.Add(filter);
            }

            return new IronTaskThatReturnsJson<QueuesInfo>(builder);
        }
    }
}