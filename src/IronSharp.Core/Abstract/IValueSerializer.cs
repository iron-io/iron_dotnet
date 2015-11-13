using System;

namespace IronIO.Core
{
    public interface IValueSerializer
    {
        string Generate(object value);

        object Parse(string value, Type type);

        T Parse<T>(string value);
    }
}