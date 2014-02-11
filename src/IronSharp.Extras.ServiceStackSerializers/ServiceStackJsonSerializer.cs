using System;
using IronSharp.Core;
using ServiceStack.Text;

namespace IronSharp.Extras.ServiceStackSerializers
{
    public class ServiceStackJsonSerializer : IValueSerializer
    {
        public string Generate(object value)
        {
            return JsonSerializer.SerializeToString(value);
        }

        public object Parse(string value, Type type)
        {
            return JsonSerializer.DeserializeFromString(value, type);
        }

        public T Parse<T>(string value)
        {
            return JsonSerializer.DeserializeFromString<T>(value);
        }
    }
}