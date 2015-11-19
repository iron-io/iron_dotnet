using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;

namespace IronIO.IronWorker
{
    public class CodeInfoCollection
    {
        private List<CodeInfo> _codes;

        [JsonProperty("codes")]
        public List<CodeInfo> Codes
        {
            get { return LazyInitializer.EnsureInitialized(ref _codes); }
            set { _codes = value; }
        }
    }
}