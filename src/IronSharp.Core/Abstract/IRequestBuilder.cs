using System;
using System.Collections.Specialized;

namespace IronSharp.Core
{
    public interface IRequestBuilder : IRequestAuthBuilder
    {
        Uri BuildUri(IronClientConfig config, string path, NameValueCollection query);
    }
}