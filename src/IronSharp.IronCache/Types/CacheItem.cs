using System;
using Newtonsoft.Json;

namespace IronIO.IronCache
{
    public class CacheItem<TValue> : CacheItem
    {
        public CacheItem()
        {
        }

        public CacheItem(int value, CacheItemOptions options = null) 
            : base(value, options)
        {
        }

        public CacheItem(string value, CacheItemOptions options = null)
            : base(value, options)
        {
        }

        public TValue ReadValueAs()
        {
            if (Value is TValue)
            {
                return (TValue)Value;
            }

            return Client.ValueSerializer.Parse<TValue>(Convert.ToString(Value));
        }
    }

    public class CacheItem : CacheItemOptions
    {
        public CacheItem()
        {
        }

        public CacheItem(int value, CacheItemOptions options = null)
        {
            Value = value;

            if (options == null) return;

            ExpiresIn = options.ExpiresIn;
            Replace = options.Replace;
            Add = options.Add;
            Cas = options.Cas;
        }

        public CacheItem(string value, CacheItemOptions options = null)
        {
            Value = value;

            if (options == null) return;

            ExpiresIn = options.ExpiresIn;
            Replace = options.Replace;
            Add = options.Add;
            Cas = options.Cas;
        }

        [JsonProperty("value")]
        public object Value { get; set; }

        [JsonProperty("cache", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Cache { get; set; }

        [JsonProperty("key", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Key { get; set; }

        [JsonIgnore]
        internal CacheClient Client { get; set; }

        public T ReadValueAs<T>()
        {
            if (Value is T)
            {
                return (T) Value;
            }

            return Client.ValueSerializer.Parse<T>(Convert.ToString(Value));
        }

        public static bool IsDefaultValue(CacheItem item)
        {
            return item == null || item.Value == null || string.IsNullOrEmpty(Convert.ToString(item.Value));
        }
    }
}