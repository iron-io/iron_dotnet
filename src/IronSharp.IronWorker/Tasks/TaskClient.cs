using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using IronSharp.Core;

namespace IronSharp.IronWorker
{
    public class TaskClient
    {
        private readonly IronWorkerRestClient _client;
        private readonly RestClient _restClient = new RestClient();

        public TaskClient(IronWorkerRestClient client)
        {
            if (client == null) throw new ArgumentNullException("client");
            Contract.EndContractBlock();

            _client = client;
        }

        public string EndPoint
        {
            get { return string.Format("{0}/tasks", _client.EndPoint); }
        }

        public IValueSerializer ValueSerializer
        {
            get { return _client.Config.SharpConfig.ValueSerializer; }
        }

        /// <summary>
        /// Cancel a Task
        /// </summary>
        /// <param name="taskId"> The ID of the task you want to cancel. </param>
        /// <remarks>
        /// http://dev.iron.io/worker/reference/api/#cancel_a_task
        /// </remarks>
        public bool Cancel(string taskId)
        {
            return _restClient.Post<ResponseMsg>(_client.Config, string.Format("{0}/cancel", TaskEndPoint(taskId))).HasExpectedMessage("Cancelled");
        }

        /// <summary>
        /// Creates a single task
        /// </summary>
        /// <param name="codeName"> The task Code Name </param>
        /// <param name="payload"> The task payload </param>
        /// <param name="options"> The task options </param>
        /// <returns> The task id </returns>
        public string Create(string codeName, object payload, TaskOptions options = null)
        {
            return Create(codeName, ValueSerializer.Generate(payload), options);
        }

        /// <summary>
        /// Creates a single task
        /// </summary>
        /// <param name="codeName"> The task Code Name </param>
        /// <param name="payload"> The task payload </param>
        /// <param name="options"> The task options </param>
        /// <returns> The task id </returns>
        public string Create(string codeName, string payload, TaskOptions options = null)
        {
            return Create(new TaskPayload(codeName, payload, options));
        }

        /// <summary>
        /// Creates a single task
        /// </summary>
        /// <param name="payload"> The task payload </param>
        /// <returns> The task id </returns>
        public string Create(TaskPayload payload)
        {
            TaskIdCollection result = Create(new TaskPayloadCollection(payload));

            if (result.Success)
            {
                return result.GetIds().FirstOrDefault();
            }

            throw new IronSharpException(string.Format("Task was not queued successfully: {0}", result.Message));
        }

        public TaskIdCollection Create(string codeName, IEnumerable<object> payloads, TaskOptions options = null)
        {
            return Create(codeName, payloads.Select(ValueSerializer.Generate), options);
        }

        public TaskIdCollection Create(string codeName, IEnumerable<string> payloads, TaskOptions options = null)
        {
            return Create(new TaskPayloadCollection(codeName, payloads, options));
        }

        public TaskIdCollection Create(IEnumerable<TaskPayload> payloads)
        {
            return Create(new TaskPayloadCollection(payloads));
        }

        /// <summary>
        /// Queue a Task
        /// </summary>
        /// <param name="collection"> </param>
        /// <remarks>
        /// http://dev.iron.io/worker/reference/api/#queue_a_task
        /// </remarks>
        public TaskIdCollection Create(TaskPayloadCollection collection)
        {
            return _restClient.Post<TaskIdCollection>(_client.Config, EndPoint, collection);
        }

        /// <summary>
        /// Get info about a task
        /// </summary>
        /// <param name="taskId"> The ID of the task you want details on. </param>
        /// <remarks>
        /// http://dev.iron.io/worker/reference/api/#get_info_about_a_task
        /// </remarks>
        public TaskInfo Get(string taskId)
        {
            return _restClient.Get<TaskInfo>(_client.Config, TaskEndPoint(taskId));
        }

        /// <summary>
        /// </summary>
        /// <param name="codeName"> The name of your worker (code package). </param>
        /// <param name="filter"> List filtering options, to filter by Status use Status = TaskStates.Running | TaskStates.Queued to get all Running or Queued tasks </param>
        /// <remarks>
        /// http://dev.iron.io/worker/reference/api/#list_tasks
        /// </remarks>
        public TaskInfoCollection List(string codeName, TaskListFilter filter = null)
        {
            var query = new NameValueCollection
            {
                {"code_name", codeName},
            };

            if (filter != null)
            {
                ApplyPageRangeFilter(query, filter.Page, filter.PerPage);

                ApplyDateRangeFilters(query, filter.FromTime, filter.ToTime);

                ApplyStatusFilter(query, filter.Status);
            }

            return _restClient.Get<TaskInfoCollection>(_client.Config, EndPoint, query).Result;
        }

        /// <summary>
        /// Get a Task’s Log
        /// </summary>
        /// <param name="taskId"> The ID of the task whose log you are retrieving </param>
        /// <remarks>
        /// http://dev.iron.io/worker/reference/api/#get_a_tasks_log
        /// </remarks>
        public string Log(string taskId)
        {
            return _restClient.Get<string>(_client.Config, string.Format("{0}/log", TaskEndPoint(taskId))).Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Set a Task’s Progress
        /// </summary>
        /// <param name="taskId"> The ID of the task whose progress you are updating. </param>
        /// <param name="taskProgress"> The task progress request </param>
        /// <remarks>
        /// http://dev.iron.io/worker/reference/api/#set_a_tasks_progress
        /// </remarks>
        public bool Progress(string taskId, TaskProgress taskProgress)
        {
            return _restClient.Post<ResponseMsg>(_client.Config, string.Format("{0}/progress", TaskEndPoint(taskId)), taskProgress).HasExpectedMessage("Progress set");
        }

        /// <summary>
        /// Retry a task
        /// </summary>
        /// <param name="taskId"> The ID of the task you want to retry. </param>
        /// <param name="delay"> The number of seconds the task should be delayed before it runs again. </param>
        /// <remarks>
        /// http://dev.iron.io/worker/reference/api/#retry_a_task
        /// </remarks>
        public TaskIdCollection Retry(string taskId, int? delay = null)
        {
            object payload = null;

            if (delay.HasValue)
            {
                payload = new {delay};
            }

            return _restClient.Post<TaskIdCollection>(_client.Config, string.Format("{0}/retry", TaskEndPoint(taskId)), payload);
        }

        public string TaskEndPoint(string taskId)
        {
            return string.Format("{0}/tasks/{1}", _client.EndPoint, taskId);
        }

        public Uri Webhook(string codeName, string token = null)
        {
            if (codeName == null) throw new ArgumentNullException("codeName");

            IRestClientRequest request = new RestClientRequest
            {
                EndPoint = string.Format("{0}/webhook", EndPoint),
                AuthTokenLocation = AuthTokenLocation.Querystring,
                Query = new NameValueCollection
                {
                    {"code_name", codeName}
                }
            };
            return _restClient.BuildRequestUri(_client.Config, request, token);
        }

        private static void ApplyDateRangeFilters(NameValueCollection query, DateTime? fromTime, DateTime? toTime)
        {
            if (fromTime.HasValue)
            {
                query.Add("from_time", Convert.ToString(DateTimeHelpers.SecondsSinceEpoch(fromTime.Value)));
            }

            if (toTime.HasValue)
            {
                query.Add("to_time", Convert.ToString(DateTimeHelpers.SecondsSinceEpoch(toTime.Value)));
            }
        }

        private static void ApplyPageRangeFilter(NameValueCollection query, int? page, int? perPage)
        {
            if (page.HasValue && page.Value > 0)
            {
                query.Add("page", Convert.ToString(page.Value));
            }

            if (perPage.HasValue && perPage.Value > 0)
            {
                query.Add("per_page", Convert.ToString(perPage.Value));
            }
        }

        private static void ApplyStatusFilter(NameValueCollection query, TaskStates statusFilter)
        {
            var statesToCheck = new[] {TaskStates.Queued, TaskStates.Running, TaskStates.Complete, TaskStates.Error, TaskStates.Cancelled, TaskStates.Killed, TaskStates.Timeout};

            foreach (TaskStates state in statesToCheck.Where(x => statusFilter.HasFlag(x)))
            {
                query.Add(Convert.ToString(state).ToLower(), "1");
            }
        }
    }
}