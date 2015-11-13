using System;
using IronIO.Core;
using ServiceStack.Text;

namespace IronSharp.Extras.ServiceStackSerializers
{
    public class ServiceStackTypeSerializer : IValueSerializer
    {
        public string Generate(object value)
        {
            return TypeSerializer.SerializeToString(value);
        }

        public object Parse(string value, Type type)
        {
            return TypeSerializer.DeserializeFromString(value, type);
        }

        public T Parse<T>(string value)
        {
            return TypeSerializer.DeserializeFromString<T>(value);
        }
    }
}