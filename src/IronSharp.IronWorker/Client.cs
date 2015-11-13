﻿using IronIO.Core;

namespace IronIO.IronWorker
{
    /// <summary>
    /// http://dev.iron.io/worker/reference/api/#list_code_packages
    /// </summary>
    public static class Client
    {
        /// <summary>
        /// Creates a new client using the environmental configuration JSON heirarchy.
        /// </summary>
        public static IronWorkerRestClient @New()
        {
            return New(null);
        }

        /// <summary>
        /// Creates a new client using the specified values
        /// </summary>
        public static IronWorkerRestClient @New(string projectId, string token, string host = null)
        {
            return New(new IronClientConfig
            {
                Host = host,
                ProjectId = projectId,
                Token = token
            });
        }

        /// <summary>
        /// Loads the config from a specific enivonment key
        /// </summary>
        /// <param name="env">The environment key as it appears in the iron.json file.  See http://dev.iron.io/worker/reference/cli/#upload_with_multiple_environments for an example layout</param>
        public static IronWorkerRestClient @New(string env)
        {
            return New(null, env);
        }

        /// <summary>
        /// Creates a new client using the specified configuration and optional <param name="env"></param> paramter to specificy which enivonment to use when loading default values;
        /// </summary>
        /// <param name="config">The explicitly specified configuration values</param>
        /// <param name="env">The environment key as it appears in the iron.json file.  See http://dev.iron.io/worker/reference/cli/#upload_with_multiple_environments for an example layout</param>
        public static IronWorkerRestClient @New(IronClientConfig config, string env = null)
        {
            IronClientConfig hierarchyConfig = IronDotConfigManager.Load(IronProduct.IronWorker, config, env);

            return new IronWorkerRestClient(hierarchyConfig);
        }
    }
}