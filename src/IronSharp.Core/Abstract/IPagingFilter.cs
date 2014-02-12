namespace IronSharp.Core
{
    public interface IPagingFilter
    {
        /// <summary>
        /// The 0-based page to view. The default is 0.
        /// </summary>
        int Page { get; set; }

        /// <summary>
        /// The number of queues to return per page. The default is 30, the maximum is 100.
        /// </summary>
        int PerPage { get; set; }
    }
}