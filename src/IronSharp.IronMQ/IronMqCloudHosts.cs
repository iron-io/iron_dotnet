namespace IronSharp.IronMQ
{
    /// <summary>
    /// http://dev.iron.io/mq/reference/clouds/
    /// </summary>
    public static class IronMqCloudHosts
    {
        public const string DEFAULT = AWS_US_EAST_HOST;

        /// <summary>
        /// Default
        /// </summary>
        public const string AWS_US_EAST_HOST = "mq-aws-us-east-1.iron.io";

        /// <summary>
        /// Rackspace Chicago Datacenter
        /// </summary>
        public const string RACKSPACE_ORD = "mq-rackspace-ord.iron.io";

        /// <summary>
        /// Deprecated - please use ORD.
        /// </summary>
        public const string RACKSPACE_DFW = "mq-rackspace-dfw.iron.io";
    }
}