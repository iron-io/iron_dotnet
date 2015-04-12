using System;
using IronIO.Core;
using Newtonsoft.Json;

namespace IronSharp.IronCache
{
    public class CacheItemOptions : IInspectable
    {
        public CacheItemOptions()
        {
        }

        public CacheItemOptions(TimeSpan expiresIn)
        {
            ExpiresIn = expiresIn.Seconds;
        }

        /// <summary>
        /// How long in seconds to keep the item in the cache before it is deleted. By default, items do not expire. Maximum is 2,592,000 seconds (30 days).
        /// </summary>
        [JsonProperty("expires_in", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? ExpiresIn { get; set; }

        /// <summary>
        ///  If set to true, only set the item if the item is already in the cache. If the item is not in the cache, do not create it.
        /// </summary>
        [JsonProperty("replace", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? Replace { get; set; }

        /// <summary>
        ///  If set to true, only set the item if the item is not already in the cache. If the item is in the cache, do not overwrite it.
        /// </summary>
        [JsonProperty("Add", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? Add { get; set; }

        /// <summary>
        ///  If set, the new item will only be placed in the cache if there is an existing item with a matching key and cas value. An item’s cas value is automatically generated and is
        /// included when the item is retrieved.
        /// </summary>
        [JsonProperty("cas", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Cas { get; set; }
    }
}