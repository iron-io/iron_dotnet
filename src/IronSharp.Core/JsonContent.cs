using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace IronIO.Core
{
    public class JsonContent : StringContent
    {
        public JsonContent(Object content)
            : base(JSON.Generate(content))
        {
            Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }
    }
}