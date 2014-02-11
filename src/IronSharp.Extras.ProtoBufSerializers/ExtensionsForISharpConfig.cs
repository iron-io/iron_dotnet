using IronSharp.Core;

namespace IronSharp.Extras.ProtoBufSerializers
{
    public static class ExtensionsForISharpConfig
    {
        public static void UseProtoBufSerializer(this IIronSharpConfig config, ProtoBufConfig protoBufConfig = null)
        {
            ProtoBufValueSerializer serializer = null;

            if (protoBufConfig != null)
            {
                ModelSerializerContext.Model = protoBufConfig.TypeModel;

                if (protoBufConfig.ConvertToBytes != null && protoBufConfig.ConvertToString != null)
                {
                    serializer = new ProtoBufValueSerializer(protoBufConfig.ConvertToBytes, protoBufConfig.ConvertToString);
                }
            }
            config.SharpConfig.ValueSerializer = serializer ?? new ProtoBufValueSerializer();
        }
    }
}