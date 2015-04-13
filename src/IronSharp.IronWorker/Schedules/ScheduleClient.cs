using System;
using System.Diagnostics.Contracts;
using System.Net.Http;
using IronIO.Core;

namespace IronIO.IronWorker
{
    public class ScheduleClient
    {
        private readonly IronWorkerRestClient _client;

        public ScheduleClient(IronWorkerRestClient client)
        {
            if (client == null) throw new ArgumentNullException("client");
            Contract.EndContractBlock();

            _client = client;
        }

        public string EndPoint
        {
            get { return string.Format("{0}/schedules", _client.EndPoint); }
        }

        public IValueSerializer ValueSerializer
        {
            get { return _client.EndpointConfig.Config.SharpConfig.ValueSerializer; }
        }

        public IIronTask<bool> Cancel(string scheduleId)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Post,
                Path = ScheduleEndPoint(scheduleId) + "/cancel"
            };

            return new IronTaskThatReturnsAnExpectedResult(builder, "Cancelled");
        }

        public IIronTask<ScheduleIdCollection> Create(string codeName, object payload, ScheduleOptions options)
        {
            return Create(codeName, ValueSerializer.Generate(payload), options);
        }

        public IIronTask<ScheduleIdCollection> Create(string codeName, string payload, ScheduleOptions options)
        {
            return Create(new SchedulePayloadCollection(codeName, payload, options));
        }

        public IIronTask<ScheduleIdCollection> Create(SchedulePayloadCollection collection)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Post,
                Path = EndPoint,
                HttpContent = new JsonContent(collection)
            };

            return new IronTaskThatReturnsJson<ScheduleIdCollection>(builder);
        }

        public IIronTask<ScheduleInfo> Get(string scheduleId)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Get,
                Path = ScheduleEndPoint(scheduleId)
            };

            return new IronTaskThatReturnsJson<ScheduleInfo>(builder);
        }

        /// <summary>
        /// List Scheduled Tasks
        /// </summary>
        /// <param name="filter"> </param>
        /// <remarks>
        /// http://dev.iron.io/worker/reference/api/#list_scheduled_tasks
        /// </remarks>
        public IIronTask<ScheduleInfoCollection> List(PagingFilter filter = null)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Get,
                Path = EndPoint
            };

            if (filter != null)
            {
                builder.Query.Add(filter);
            }

            return new IronTaskThatReturnsJson<ScheduleInfoCollection>(builder);
        }

        public string ScheduleEndPoint(string scheduleId)
        {
            return string.Format("{0}/{1}", EndPoint, scheduleId);
        }
    }
}