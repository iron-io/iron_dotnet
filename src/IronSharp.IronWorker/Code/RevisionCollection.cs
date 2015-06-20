using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;

namespace IronIO.IronWorker
{
    public class RevisionCollection
    {
        private List<CodeInfo> _revisions;

        [JsonProperty("revisions")]
        public List<CodeInfo> Revisions
        {
            get { return LazyInitializer.EnsureInitialized(ref _revisions); }
            set { _revisions = value; }
        }
    }
}