using Newtonsoft.Json;

namespace IronSharp.IronWorker
{
    public class WorkerOptions
    {
        /// <summary>
        /// A unique name for your worker. This will be used to assign tasks to the worker as well as to update the code. If a worker with this name already exists, the code you are uploading
        /// will be added as a new revision.
        /// </summary>
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// The name of the file within the zip that will be executed when a task is run.
        /// </summary>
        [JsonProperty("file_name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string FileName { get; set; }

        /// <summary>
        /// An arbitrary string (usually YAML or JSON) that, if provided, will be available in a file that your worker can access. File location will be passed in via the -config argument.
        /// The config cannot be larger than 64KB in size.
        /// </summary>
        [JsonProperty("config", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Config { get; set; }

        /// <summary>
        /// The maximum number of workers that should be run in parallel. This is useful for keeping your workers from hitting API quotas or overloading databases that are not prepared to
        /// handle the highly-concurrent worker environment. If omitted, there will be no limit on the number of concurrent workers.
        /// </summary>
        [JsonProperty("max_concurrency", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? MaxConcurrency { get; set; }

        /// <summary>
        /// The maximum number of times failed tasks should be retried, in the event that there’s an error while running them. If omitted, tasks will not be retried. Tasks cannot be retried
        /// more than ten times.
        /// </summary>
        [JsonProperty("retries", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? Retries { get; set; }

        /// <summary>
        /// The number of seconds to wait before retries. If omitted, tasks will be immediately retried.
        /// </summary>
        [JsonProperty("retries_delay", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? RetriesDelay { get; set; }
    }
}