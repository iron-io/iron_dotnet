namespace IronIO.IronWorker
{
    public enum TaskPriority
    {
        /// <summary>
        /// Default
        /// </summary>
        Default = 0,

        /// <summary>
        /// Medium
        /// </summary>
        Medium = 1,

        /// <summary>
        /// High (less time in queue)
        /// </summary>
        High = 2
    }
}