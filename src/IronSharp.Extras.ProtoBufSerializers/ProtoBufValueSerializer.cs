using System;
using IronIO.Core;

namespace IronSharp.Extras.ProtoBufSerializers
{
    public class ProtoBufValueSerializer : IValueSerializer
    {
        private readonly Func<string, byte[]> _convertToBytes;

        private readonly Func<byte[], string> _convertToString;

        /// <summary>
        /// Creates a new instance of the ProtoBufValueSerializer using Base64 string conversions
        /// </summary>
        public ProtoBufValueSerializer()
            : this(Convert.FromBase64String, Convert.ToBase64String)
        {
        }

        /// <summary>
        /// Creates a new instance of the ProtoBufValueSerializer using custom string conversions
        /// </summary>
        public ProtoBufValueSerializer(Func<string, byte[]> convertToBytes, Func<byte[], string> convertToString)
        {
            _convertToBytes = convertToBytes;
            _convertToString = convertToString;
        }

        public string Generate(object value)
        {
            return _convertToString(value.ToProtoBuf());
        }

        public object Parse(string value, Type type)
        {
            return _convertToBytes(value).FromProtoBuf(type);
        }

        public T Parse<T>(string value)
        {
            return _convertToBytes(value).FromProtoBuf<T>();
        }
    }
}