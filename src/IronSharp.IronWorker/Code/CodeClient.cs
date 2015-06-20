using System.IO;
using System.Net.Http;
using System.Net.Mime;
using IronIO.Core;

namespace IronIO.IronWorker
{
    public class CodeClient
    {
        private readonly IronWorkerRestClient _client;
        private readonly string _codeId;

        public CodeClient(IronWorkerRestClient client, string codeId)
        {
            _client = client;
            _codeId = codeId;
        }

        public string EndPoint => $"{_client.EndPoint}/codes/{_codeId}";

        /// <summary>
        ///     Delete a code package
        /// </summary>
        /// <remarks>
        ///     http://dev.iron.io/worker/reference/api/#delete_a_code_package
        /// </remarks>
        public IIronTask<bool> Delete()
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Delete,
                Path = EndPoint
            };

            return new IronTaskThatReturnsAnExpectedResult(builder, "Deleted");
        }

        /// <summary>
        ///     Download a Code Package
        /// </summary>
        /// <remarks>
        ///     http://dev.iron.io/worker/reference/api/#download_a_code_package
        /// </remarks>
        public IIronTask<HttpResponseMessage> Download()
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Get,
                Path = $"{EndPoint}/download",
                Accept = MediaTypeNames.Application.Octet
            };

            return new IronTaskThatReturnsResponse(builder);
        }

        /// <summary>
        ///     Get info about a code package
        /// </summary>
        /// <remarks>
        ///     http://dev.iron.io/worker/reference/api/#get_info_about_a_code_package
        /// </remarks>
        public IIronTask<CodeInfo> Info()
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Get,
                Path = EndPoint
            };

            return new IronTaskThatReturnsJson<CodeInfo>(builder);
        }

        public IIronTask<RevisionCollection> Revisions(int? page = null, int? perPage = null)
        {
            return Revisions(new PagingFilter(page, perPage));
        }

        /// <summary>
        ///     List Code Package Revisions
        /// </summary>
        /// <remarks>
        ///     http://dev.iron.io/worker/reference/api/#list_code_package_revisions
        /// </remarks>
        public IIronTask<RevisionCollection> Revisions(PagingFilter filter = null)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Get,
                Path = $"{EndPoint}/revisions"
            };

            if (filter != null)
            {
                builder.Query.Add(filter);
            }

            return new IronTaskThatReturnsJson<RevisionCollection>(builder);
        }

        /// <summary>
        ///     Upload a Code Package
        /// </summary>
        /// <remarks>
        ///     http://dev.iron.io/worker/reference/api/#upload_or_update_a_code_package
        /// </remarks>
        public IIronTask<HttpResponseMessage> Upload(Stream zipFile, WorkerOptions options)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Post,
                Path = EndPoint + "/download",
                HttpContent = new MultipartFormDataContent
                {
                    new JsonContent(options),
                    new StreamContent(zipFile)
                }
            };

            return new IronTaskThatReturnsResponse(builder);
        }
    }
}