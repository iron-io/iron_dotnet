using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IronIO.Core;

namespace IronIO.IronWorker
{
    public class TaskClient
    {
        private static readonly TaskStates[] StatesToCheck =
        {
            TaskStates.Queued,
            TaskStates.Running,
            TaskStates.Complete,
            TaskStates.Error,
            TaskStates.Cancelled,
            TaskStates.Killed,
            TaskStates.Timeout
        };

        private readonly IronWorkerRestClient _client;

        public TaskClient(IronWorkerRestClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            Contract.EndContractBlock();

            _client = client;
        }

        public string EndPoint => $"{_client.EndPoint}/tasks";

        public IValueSerializer ValueSerializer => _client.EndpointConfig.Config.SharpConfig.ValueSerializer;

        /// <summary>
        /// Cancel a Task
        /// </summary>
        /// <param name="taskId"> The ID of the task you want to cancel. </param>
        /// <remarks>
        /// http://dev.iron.io/worker/reference/api/#cancel_a_task
        /// </remarks>
        public IIronTask<bool> Cancel(string taskId)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Post,
                Path = $"{TaskEndPoint(taskId)}/cancel"
            };

            return new IronTaskThatReturnsAnExpectedResult(builder, "Cancelled");
        }

        /// <summary>
        /// Creates a single task
        /// </summary>
        /// <param name="codeName"> The task Code Name </param>
        /// <param name="payload"> The task payload </param>
        /// <param name="options"> The task options </param>
        /// <returns> The task id </returns>
        public IIronTask<string> Create(string codeName, object payload, TaskOptions options = null)
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
        public IIronTask<string> Create(string codeName, string payload, TaskOptions options = null)
        {
            return Create(new TaskPayload(codeName, payload, options));
        }

        /// <summary>
        /// Creates a single task
        /// </summary>
        /// <param name="payload"> The task payload </param>
        /// <returns> The task id </returns>
        public IIronTask<string> Create(TaskPayload payload)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Post,
                Path = EndPoint,
                HttpContent = new JsonContent(new TaskPayloadCollection(payload))
            };

            return new IronTaskThatReturnsFirstTaskId(builder);
        }

        public IIronTask<TaskIdCollection> Create(string codeName, IEnumerable<object> payloads,
            TaskOptions options = null)
        {
            return Create(codeName, payloads.Select(ValueSerializer.Generate), options);
        }

        public IIronTask<TaskIdCollection> Create(string codeName, IEnumerable<string> payloads,
            TaskOptions options = null)
        {
            return Create(new TaskPayloadCollection(codeName, payloads, options));
        }

        public IIronTask<TaskIdCollection> Create(IEnumerable<TaskPayload> payloads)
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
        public IIronTask<TaskIdCollection> Create(TaskPayloadCollection collection)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Post,
                Path = EndPoint,
                HttpContent = new JsonContent(collection)
            };

            return new IronTaskThatReturnsJson<TaskIdCollection>(builder);
        }

        /// <summary>
        /// Get info about a task
        /// </summary>
        /// <param name="taskId"> The ID of the task you want details on. </param>
        /// <remarks>
        /// http://dev.iron.io/worker/reference/api/#get_info_about_a_task
        /// </remarks>
        public IIronTask<TaskInfo> Get(string taskId)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Get,
                Path = TaskEndPoint(taskId)
            };

            return new IronTaskThatReturnsJson<TaskInfo>(builder);
        }

        /// <summary>
        /// </summary>
        /// <param name="codeName"> The name of your worker (code package). </param>
        /// <param name="filter">
        /// List filtering options, to filter by Status use Status = TaskStates.Running | TaskStates.Queued
        /// to get all Running or Queued tasks
        /// </param>
        /// <remarks>
        /// http://dev.iron.io/worker/reference/api/#list_tasks
        /// </remarks>
        public IIronTask<TaskInfoCollection> List(string codeName, TaskListFilter filter = null)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Get,
                Path = EndPoint
            };

            builder.Query["code_name"] = codeName;

            if (filter != null)
            {
                ApplyPageRangeFilter(builder.Query, filter.Page, filter.PerPage);

                ApplyDateRangeFilters(builder.Query, filter.FromTime, filter.ToTime);

                ApplyStatusFilter(builder.Query, filter.Status);
            }

            return new IronTaskThatReturnsJson<TaskInfoCollection>(builder);
        }

        /// <summary>
        /// Get a Task’s Log
        /// </summary>
        /// <param name="taskId"> The ID of the task whose log you are retrieving </param>
        /// <remarks>
        /// http://dev.iron.io/worker/reference/api/#get_a_tasks_log
        /// </remarks>
        public IIronTask<string> Log(string taskId)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Get,
                Path = $"{TaskEndPoint(taskId)}/log"
            };

            return new IronTaskThatReturnsString(builder);
        }

        /// <summary>
        /// Set a Task’s Progress
        /// </summary>
        /// <param name="taskId"> The ID of the task whose progress you are updating. </param>
        /// <param name="taskProgress"> The task progress request </param>
        /// <remarks>
        /// http://dev.iron.io/worker/reference/api/#set_a_tasks_progress
        /// </remarks>
        public IIronTask<bool> Progress(string taskId, TaskProgress taskProgress)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Post,
                Path = $"{TaskEndPoint(taskId)}/progress"
            };

            return new IronTaskThatReturnsAnExpectedResult(builder, "Progress set");
        }

        /// <summary>
        /// Retry a task
        /// </summary>
        /// <param name="taskId"> The ID of the task you want to retry. </param>
        /// <param name="delay"> The number of seconds the task should be delayed before it runs again. </param>
        /// <remarks>
        /// http://dev.iron.io/worker/reference/api/#retry_a_task
        /// </remarks>
        public IIronTask<TaskIdCollection> Retry(string taskId, int? delay = null)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Post,
                Path = $"{TaskEndPoint(taskId)}/retry"
            };

            object payload = null;

            if (delay.HasValue)
            {
                payload = new {delay};
            }

            builder.HttpContent = new JsonContent(payload);

            return new IronTaskThatReturnsJson<TaskIdCollection>(builder);
        }

        public string TaskEndPoint(string taskId)
        {
            return $"{_client.EndPoint}/tasks/{taskId}";
        }

        public Uri Webhook(string codeName, string token = null)
        {
            if (codeName == null)
            {
                throw new ArgumentNullException(nameof(codeName));
            }

            var endpointConfig = _client.EndpointConfig;

            var webhookAuth = endpointConfig.TokenContainer.GetWebHookToken(token);

            var builder = new IronTaskRequestBuilder(endpointConfig)
            {
                HttpMethod = HttpMethod.Get,
                Path = $"{EndPoint}/webhook",
                AuthToken = webhookAuth
            };

            builder.Query["code+name"] = codeName;

            return builder.Build().RequestUri;
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
            foreach (var state in StatesToCheck.Where(x => statusFilter.HasFlag(x)))
            {
                query.Add(Convert.ToString(state).ToLower(), "1");
            }
        }

        public class IronTaskThatReturnsFirstTaskId : IronTaskThatReturnsJson<TaskIdCollection>, IIronTask<string>
        {
            public IronTaskThatReturnsFirstTaskId(IronTaskRequestBuilder taskBuilder) : base(taskBuilder)
            {
            }

            public new string Send()
            {
                var result = base.Send();

                if (result.Success)
                {
                    return result.GetIds().FirstOrDefault();
                }

                throw new IronIOException($"Task was not queued successfully: {result.Message}");
            }

            public new async Task<string> SendAsync(CancellationToken cancellationToken = new CancellationToken())
            {
                var result = await base.SendAsync(cancellationToken);

                if (result.Success)
                {
                    return result.GetIds().FirstOrDefault();
                }

                throw new IronIOException($"Task was not queued successfully: {result.Message}");
            }
        }
    }
}