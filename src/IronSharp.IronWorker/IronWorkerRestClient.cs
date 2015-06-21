using System.Net.Http;
using System.Threading;
using IronIO.Core;

namespace IronIO.IronWorker
{
    public class IronWorkerRestClient
    {
        private readonly IronTaskEndpointConfig _endpointConfig;

        internal IronWorkerRestClient(IronClientConfig config)
        {
            LazyInitializer.EnsureInitialized(ref config);

            if (string.IsNullOrEmpty(config.Host))
            {
                config.Host = IronWorkCloudHosts.DEFAULT;
            }

            config.ApiVersion = config.ApiVersion.GetValueOrDefault(2);

            _endpointConfig = new IronTaskEndpointConfig(config);
        }

        public IIronTaskEndpointConfig EndpointConfig => _endpointConfig;

        public string EndPoint => "/projects/{Project ID}";

        #region Code

        public CodeClient Code(string codeId)
        {
            return new CodeClient(this, codeId);
        }

        public IIronTask<CodeInfoCollection> Codes(int? page = null, int? perPage = null)
        {
            return Codes(new PagingFilter
            {
                Page = page.GetValueOrDefault(),
                PerPage = perPage.GetValueOrDefault()
            });
        }

        /// <summary>
        /// List code packages
        /// </summary>
        /// <param name="filter"> </param>
        /// <remarks>
        /// http://dev.iron.io/worker/reference/api/#list_code_packages
        /// </remarks>
        public IIronTask<CodeInfoCollection> Codes(PagingFilter filter = null)
        {
            var builder = new IronTaskRequestBuilder(_endpointConfig)
            {
                HttpMethod = HttpMethod.Get,
                Path = $"{EndPoint}/codes"
            };

            return new IronTaskThatReturnsJson<CodeInfoCollection>(builder);
        }

        #endregion

        #region Task

        public TaskClient Tasks => new TaskClient(this);

        #endregion

        #region Schedule

        public ScheduleClient Schedules => new ScheduleClient(this);

        #endregion
    }
}