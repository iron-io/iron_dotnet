namespace IronIO.Core
{
    public static class HttpClientOptions
    {
        static HttpClientOptions()
        {
            EnableRetry = true;
            RetryLimit = 4;
        }

        public static bool EnableRetry { get; set; }

        public static int RetryLimit { get; set; }
    }
}