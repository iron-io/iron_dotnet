using System;
using System.Threading;

namespace IronSharp.Core
{
    internal static class ExponentialBackoff
    {
        public static void Sleep(double backoffFactor, int attempt)
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(Math.Pow(backoffFactor, attempt)));
        }
    }
}