using System;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace IronSharp.Core
{
    internal static class ExponentialBackoff
    {
        public static void Sleep(double backoffFactor, int attempt)
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(Math.Pow(backoffFactor, attempt)));
        }

        public static bool IsRetriableStatusCode(HttpResponseMessage response)
        {
            return response != null && response.StatusCode == HttpStatusCode.ServiceUnavailable;
        }
    }
}