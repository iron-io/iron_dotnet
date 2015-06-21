using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace IronIO.Core
{
    public abstract class IronTask<TResult> : IIronTask<TResult>
    {
        private HttpClient _httpClient;

        protected IronTask(IronTaskRequestBuilder taskBuilder)
        {
            TaskBuilder = taskBuilder;
        }

        public HttpClient HttpClient
        {
            get { return LazyInitializer.EnsureInitialized(ref _httpClient, () => RestUtility.DefaultInstance); }
            set { _httpClient = value; }
        }

        public IronTaskRequestBuilder TaskBuilder { get; }

        public virtual void FireAndForget(CancellationToken cancellationToken = new CancellationToken())
        {
            if (HostingEnvironment.IsHosted)
            {
                Func<CancellationToken, Task> task = bgWorkerCancellationToken =>
                    GetResponseAsync(CreateLinkedToken(bgWorkerCancellationToken, cancellationToken));

                HostingEnvironment.QueueBackgroundWorkItem(task);
            }
            else
            {
                Task.Run(() => GetResponseAsync(cancellationToken), cancellationToken).ConfigureAwait(false);
            }
        }

        public virtual TResult Send()
        {
            var response = GetResponseSync();
            SendExecuted(response);
            var result = ReadResultSync(response);
            return InspectResultAndReturn(result);
        }

        public virtual async Task<TResult> SendAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var response = await GetResponseAsync(cancellationToken).ConfigureAwait(false);
            SendExecuted(response);
            var result = await ReadResultAsync(response).ConfigureAwait(false);
            return InspectResultAndReturn(result);
        }

        protected virtual async Task<HttpResponseMessage> GetResponseAsync(
            CancellationToken cancellationToken = new CancellationToken())
        {
            var request = await TaskBuilder.BuildAsync();
            SendExecuting(HttpClient, request);
            return await HttpClient.SendAsync(request, cancellationToken);
        }

        protected virtual HttpResponseMessage GetResponseSync()
        {
            var request = TaskBuilder.Build();
            SendExecuting(HttpClient, request);
            return HttpClient.SendAsync(request).Result;
        }

        protected virtual TResult InspectResultAndReturn(TResult result)
        {
            return result;
        }

        protected virtual Task<TResult> ReadResultAsync(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<TResult>();
        }

        protected virtual TResult ReadResultSync(HttpResponseMessage response)
        {
            return ReadResultAsync(response).Result;
        }

        protected virtual void SendExecuted(HttpResponseMessage response)
        {
        }

        protected virtual void SendExecuting(HttpClient httpClient, HttpRequestMessage request)
        {
        }

        private static CancellationToken CreateLinkedToken(CancellationToken t1, CancellationToken? t2)
        {
            return t2.HasValue ? CancellationTokenSource.CreateLinkedTokenSource(t1, t2.Value).Token : t1;
        }
    }
}