using System;
using ProtoBuf.Meta;

namespace IronSharp.Extras.ProtoBufSerializers
{
    public class ProtoBufConfig
    {
        public Func<string, byte[]> ConvertToBytes { get; set; }

        public Func<byte[], string> ConvertToString { get; set; }

        public RuntimeTypeModel TypeModel { get; set; }
    }
}