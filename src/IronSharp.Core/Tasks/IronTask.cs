using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace IronSharp.Core
{
    public abstract class IronTask<TResult> : IIronTask<TResult>
    {
        private readonly CancellationToken? _cancellationToken;
        private readonly HttpClient _httpClient;
        private readonly HttpRequestMessage _request;

        protected IronTask(HttpClient httpClient, HttpRequestMessage request,
            CancellationToken? cancellationToken = null)
        {
            _httpClient = httpClient;
            _request = request;
            _cancellationToken = cancellationToken;
        }

        public virtual void FireAndForget()
        {
            if (HostingEnvironment.IsHosted)
            {
                Func<CancellationToken, Task> task = bgWorkerCancellationToken =>
                        ExecuteSendAsync(CreateLinkedToken(bgWorkerCancellationToken, _cancellationToken));

                HostingEnvironment.QueueBackgroundWorkItem(task);
            }
            else
            {
                Task.Run(() => ExecuteSendAsync(), _cancellationToken.GetValueOrDefault()).ConfigureAwait(false);
            }
        }

        public virtual TResult Send()
        {
            var response = GetResponseSync();
            var result = ReadResultSync(response);
            return InspectResultAndReturn(result);
        }

        public virtual async Task<TResult> SendAsync()
        {
            var response = await ExecuteSendAsync().ConfigureAwait(false);
            var result = await ReadResultAsync(response).ConfigureAwait(false);
            return InspectResultAndReturn(result);
        }

        protected virtual Task<HttpResponseMessage> ExecuteSendAsync()
        {
            return ExecuteSendAsync(_cancellationToken);
        }

        protected virtual Task<HttpResponseMessage> ExecuteSendAsync(CancellationToken? cancellationToken)
        {
            return cancellationToken.HasValue
                ? _httpClient.SendAsync(_request, cancellationToken.Value)
                : _httpClient.SendAsync(_request);
        }

        protected virtual HttpResponseMessage GetResponseSync()
        {
            return ExecuteSendAsync().Result;
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

        private static CancellationToken CreateLinkedToken(CancellationToken t1, CancellationToken? t2)
        {
            return t2.HasValue ? CancellationTokenSource.CreateLinkedTokenSource(t1, t2.Value).Token : t1;
        }
    }
}