using IronIO.Core;

namespace IronSharp.Extras.ServiceStackSerializers
{
    public static class ExtensionsForISharpConfig
    {
        public static void UseServiceStackJsonSerializer(this IIronSharpConfig config)
        {
            config.SharpConfig.ValueSerializer = new ServiceStackJsonSerializer();
        }

        public static void UseServiceStackTypeSerializer(this IIronSharpConfig config)
        {
            config.SharpConfig.ValueSerializer = new ServiceStackTypeSerializer();
        }

        public static void UseServiceStackXmlSerializer(this IIronSharpConfig config)
        {
            config.SharpConfig.ValueSerializer = new ServiceStackXmlSerializer();
        }
    }
}