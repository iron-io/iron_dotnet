using System.Net.Http;

namespace IronIO.Core
{
    public class MaximumRetryAttemptsExceededException : IronIOException
    {
        public MaximumRetryAttemptsExceededException(HttpRequestMessage request, int maxAttempts)
            : base($"The maximum number of retry attempts ({maxAttempts}) has been exceeded.")
        {
            Request = request;
        }

        public HttpRequestMessage Request { get; }
    }
}