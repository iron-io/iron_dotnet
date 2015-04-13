using System;
using System.IO;
using System.Net.Http;

namespace IronIO.Core
{
    public class RestResponseException : IronIOException
    {
        public RestResponseException(string message, HttpResponseMessage response)
            : base(message + AppendDebugInfo(response))
        {
            Response = response;
        }

        public RestResponseException(string message, HttpResponseMessage response, Exception innerException)
            : base(message + AppendDebugInfo(response), innerException)
        {
            Response = response;
        }

        public HttpRequestMessage RequestMessage
        {
            get { return Response.RequestMessage; }
        }

        public HttpResponseMessage Response { get; set; }

        private static string AppendDebugInfo(HttpResponseMessage response)
        {
            if (response == null || response.RequestMessage == null)
            {
                return null;
            }

            using (var sw = new StringWriter())
            {
                HttpRequestMessage request = response.RequestMessage;

                sw.WriteLine();

                sw.WriteLine("RequestUri: {0} {1}", request.Method, request.RequestUri);

                if (request.Content != null)
                {
                    try
                    {
                        sw.WriteLine(request.Content.ReadAsStringAsync().Result);
                    }
                    catch (Exception e)
                    {
                        sw.WriteLine("Unable to read response content. " + e.Message);
                    }                    
                }

                return sw.ToString();
            }
        }
    }
}