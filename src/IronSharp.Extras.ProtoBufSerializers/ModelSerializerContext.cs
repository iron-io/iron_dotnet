using System;
using System.IO;
using System.Threading;
using ProtoBuf.Meta;

namespace IronSharp.Extras.ProtoBufSerializers
{
    public static class ModelSerializerContext
    {
        private static RuntimeTypeModel _model;

        public static RuntimeTypeModel Model
        {
            get { return LazyInitializer.EnsureInitialized(ref _model, TypeModel.Create); }
            set { _model = value; }
        }

        public static object DeserializeFromStream(Type type, Stream source)
        {
            return Model.Deserialize(source, null, type);
        }

        /// <summary>
        /// Deserializers the protobuf byte stream into an instance of the specified type of <typeparam name="T"></typeparam>.
        /// </summary>
        public static T FromProtoBuf<T>(this byte[] bytes)
        {
            return (T)bytes.FromProtoBuf(typeof(T));
        }

        /// <summary>
        /// Deserializers the protobuf byte stream into an instance of the specified <paramref name="type"/>.
        /// </summary>
        public static object FromProtoBuf(this byte[] bytes, Type type)
        {
            using (var ms = new MemoryStream(bytes))
            {
                return DeserializeFromStream(type, ms);
            }
        }

        public static void SerializeToStream(object value, Stream destination)
        {
            Model.Serialize(destination, value);
        }

        /// <summary>
        /// Serializes the object graph into a protobuf type stream
        /// </summary>
        public static byte[] ToProtoBuf<T>(this T value)
        {
            using (var ms = new MemoryStream())
            {
                SerializeToStream(value, ms);
                return ms.ToArray();
            }
        }
    }
}