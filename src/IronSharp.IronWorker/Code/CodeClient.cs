﻿using System.IO;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using IronSharp.Core;

namespace IronSharp.IronWorker
{
    public class CodeClient
    {
        private readonly IronWorkerRestClient _client;
        private readonly string _codeId;
        protected readonly RestClient _restClient; 

        public CodeClient(IronWorkerRestClient client, string codeId)
        {
            _client = client;
            _codeId = codeId;
            _restClient = client.RestClient;
        }

        public string EndPoint
        {
            get { return string.Format("{0}/codes/{1}", _client.EndPoint, _codeId); }
        }

        /// <summary>
        /// Delete a code package
        /// </summary>
        /// <remarks>
        /// http://dev.iron.io/worker/reference/api/#delete_a_code_package
        /// </remarks>
        public bool Delete()
        {
            return _restClient.Delete<ResponseMsg>(_client.Config, EndPoint).HasExpectedMessage("Deleted");
        }

        /// <summary>
        /// Download a Code Package
        /// </summary>
        /// <remarks>
        /// http://dev.iron.io/worker/reference/api/#download_a_code_package
        /// </remarks>
        public Task<HttpResponseMessage> Download()
        {
            return _restClient.Execute(_client.Config, new RestClientRequest
            {
                EndPoint = EndPoint + "/download",
                Method = HttpMethod.Get,
                Accept = MediaTypeNames.Application.Octet
            });
        }

        /// <summary>
        /// Get info about a code package
        /// </summary>
        /// <remarks>
        /// http://dev.iron.io/worker/reference/api/#get_info_about_a_code_package
        /// </remarks>
        public CodeInfo Info()
        {
            return _restClient.Get<CodeInfo>(_client.Config, EndPoint);
        }

        public RevisionCollection Revisions(int? page = null, int? perPage = null)
        {
            return Revisions(new PagingFilter(page, perPage));
        }

        /// <summary>
        /// List Code Package Revisions
        /// </summary>
        /// <remarks>
        /// http://dev.iron.io/worker/reference/api/#list_code_package_revisions
        /// </remarks>
        public RevisionCollection Revisions(PagingFilter filter = null)
        {
            return _restClient.Get<RevisionCollection>(_client.Config, EndPoint + "/revisions", filter);
        }

        /// <summary>
        /// Upload a Code Package
        /// </summary>
        /// <remarks>
        /// http://dev.iron.io/worker/reference/api/#upload_or_update_a_code_package
        /// </remarks>
        public Task<HttpResponseMessage> Upload(Stream zipFile, WorkerOptions options)
        {
            return _restClient.Execute(_client.Config, new RestClientRequest
            {
                EndPoint = EndPoint,
                Method = HttpMethod.Post,
                Content = new MultipartFormDataContent
                {
                    new JsonContent(options),
                    new StreamContent(zipFile)
                }
            });
        }
    }
}