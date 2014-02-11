using System;
using System.Threading;
using Newtonsoft.Json;

namespace IronSharp.Core
{
    public static class JSON
    {
        private static JsonSerializerSettings _settings;

        public static JsonSerializerSettings Settings
        {
            get { return LazyInitializer.EnsureInitialized(ref _settings, GetDefaultSerializerSettings); }
            set { _settings = value; }
        }

        public static string Generate(object value, JsonSerializerSettings opts = null)
        {
            return Generate(value, null, opts);
        }

        public static string Generate(object value, Type type, JsonSerializerSettings opts = null)
        {
            return JsonConvert.SerializeObject(value, type, Formatting.None, opts ?? Settings);
        }

        public static T Parse<T>(string value, JsonSerializerSettings opts = null)
        {
            return (T)Parse(value, typeof(T), opts);
        }

        public static object Parse(string value, Type type, JsonSerializerSettings opts = null)
        {
            return DeserializeObject(value, type, opts);
        }
        
        private static object DeserializeObject(string value, Type type, JsonSerializerSettings opts)
        {
            // If the target type is already string, then there is no need to Deserialize the value.
            if (type == typeof (string))
            {
                return value;
            }
            return JsonConvert.DeserializeObject(value, type, opts ?? Settings);
        }

        private static JsonSerializerSettings GetDefaultSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };
        }
    }
}