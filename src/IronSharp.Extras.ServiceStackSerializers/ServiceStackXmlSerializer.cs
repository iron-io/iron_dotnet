using System;
using IronSharp.Core;
using ServiceStack.Text;

namespace IronSharp.Extras.ServiceStackSerializers
{
    public class ServiceStackXmlSerializer : IValueSerializer
    {
        public string Generate(object value)
        {
            return XmlSerializer.SerializeToString(value);
        }

        public object Parse(string value, Type type)
        {
            return XmlSerializer.DeserializeFromString(value, type);
        }

        public T Parse<T>(string value)
        {
            return XmlSerializer.DeserializeFromString<T>(value);
        }
    }
}