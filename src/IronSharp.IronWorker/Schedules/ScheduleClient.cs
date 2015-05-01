using System;
using System.Diagnostics.Contracts;
using IronSharp.Core;

namespace IronSharp.IronWorker
{
    public class ScheduleClient
    {
        private readonly IronWorkerRestClient _client;
        protected readonly RestClient _restClient; 

        public ScheduleClient(IronWorkerRestClient client)
        {
            if (client == null) throw new ArgumentNullException("client");
            Contract.EndContractBlock();

            _client = client;
            _restClient = client.RestClient;
        }

        public string EndPoint
        {
            get { return string.Format("{0}/schedules", _client.EndPoint); }
        }

        public IValueSerializer ValueSerializer
        {
            get { return _client.Config.SharpConfig.ValueSerializer; }
        }

        public bool Cancel(string scheduleId)
        {
            return _restClient.Post<ResponseMsg>(_client.Config, ScheduleEndPoint(scheduleId) + "/cancel").HasExpectedMessage("Cancelled");
        }

        public ScheduleIdCollection Create(string codeName, object payload, ScheduleOptions options)
        {
            return Create(codeName, ValueSerializer.Generate(payload), options);
        }

        public ScheduleIdCollection Create(string codeName, string payload, ScheduleOptions options)
        {
            return Create(new SchedulePayloadCollection(codeName, payload, options));
        }

        public ScheduleIdCollection Create(SchedulePayloadCollection collection)
        {
            return _restClient.Post<ScheduleIdCollection>(_client.Config, EndPoint, collection);
        }

        public ScheduleInfo Get(string scheduleId)
        {
            return _restClient.Get<ScheduleInfo>(_client.Config, ScheduleEndPoint(scheduleId));
        }

        /// <summary>
        /// List Scheduled Tasks
        /// </summary>
        /// <param name="filter"> </param>
        /// <remarks>
        /// http://dev.iron.io/worker/reference/api/#list_scheduled_tasks
        /// </remarks>
        public ScheduleInfoCollection List(PagingFilter filter = null)
        {
            return _restClient.Get<ScheduleInfoCollection>(_client.Config, EndPoint, filter);
        }

        public string ScheduleEndPoint(string scheduleId)
        {
            return string.Format("{0}/{1}", EndPoint, scheduleId);
        }
    }
}