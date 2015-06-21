using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace IronIO.Core.Extensions
{
    public static class ExtensionsForDictionary
    {
        public static void Add(this NameValueCollection collection, string key, object value)
        {
            Contract.Requires(collection != null);
            collection.Add(key, Convert.ToString(value));
        }

        public static void AddValues(this IDictionary<string, object> dictionary, object value)
        {
            if (value == null)
            {
                return;
            }
            foreach (PropertyDescriptor item in TypeDescriptor.GetProperties(value))
            {
                dictionary.Add(item.Name, item.GetValue(value));
            }
        }

        public static TValue Get<TValue>(this IDictionary dictionary, object key)
        {
            Contract.Requires(dictionary != null);
            if (!dictionary.Contains(key))
            {
                return default(TValue);
            }
            return (TValue) dictionary[key];
        }

        public static TValue Get<TValue>(this IDictionary<string, object> dictionary, string key, TValue defaultValue = default(TValue))
        {
            Contract.Requires(dictionary != null);
            object value;
            if (dictionary.TryGetValue(key, out value))
            {
                try
                {
                    return (TValue) value;
                }
                catch (InvalidCastException)
                {
                    return Convert.ToString(value).As<TValue>();
                }
            }
            return defaultValue;
        }

        public static TValue GetOrAdd<TValue>(this IDictionary dictionary, object key, Func<object, TValue> valueFactory)
        {
            Contract.Requires(dictionary != null);
            if (dictionary.Contains(key))
            {
                return (TValue) dictionary[key];
            }
            TValue instance = valueFactory.Invoke(key);
            dictionary[key] = instance;
            return instance;
        }

        public static TValue GetOrAdd<TValue>(this IDictionary<string, object> dictionary, string key, TValue defaultValue)
        {
            return GetOrAdd(dictionary, key, () => defaultValue);
        }

        public static TValue GetOrAdd<TValue>(this IDictionary<string, object> dictionary, string key, Func<TValue> valueFactory)
        {
            Contract.Requires(dictionary != null);
            lock (dictionary)
            {
                if (!dictionary.ContainsKey(key))
                {
                    dictionary.Set(key, valueFactory.Invoke());
                }
                return (TValue) dictionary[key];
            }
        }

        public static TValue GetOrAdd<TValue>(this IDictionary<string, object> dictionary, string key, Func<string, TValue> valueFactory)
        {
            Contract.Requires(dictionary != null);
            lock (dictionary)
            {
                if (!dictionary.ContainsKey(key))
                {
                    dictionary.Set(key, valueFactory.Invoke(key));
                }
                return (TValue) dictionary[key];
            }
        }

        public static void Set(this IDictionary dictionary, object key, object value)
        {
            Contract.Requires(dictionary != null);
            dictionary[key] = value;
        }


        public static void Set<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            Contract.Requires(dictionary != null);
            dictionary[key] = value;
        }
    }
}