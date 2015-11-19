using System;
using Newtonsoft.Json;

namespace IronIO.Core
{
    public class DefaultValueSerializer : IValueSerializer
    {
        private readonly JsonSerializerSettings _settings;

        public DefaultValueSerializer() : this(null)
        {
        }

        public DefaultValueSerializer(JsonSerializerSettings settings)
        {
            _settings = settings;
        }

        public string Generate(object value)
        {
            return JSON.Generate(value, _settings);
        }

        public object Parse(string value, Type type)
        {
            return JSON.Parse(value, type);
        }

        public T Parse<T>(string value)
        {
            return JSON.Parse<T>(value, _settings);
        }
    }

    public static class ExtensionsForDefaultSerializer
    {
        public static void UseDefaultSerializer(this IIronSharpConfig config)
        {
            config.SharpConfig.ValueSerializer = null;
        }
    }
}