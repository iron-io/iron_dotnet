namespace IronIO.Core
{
    public class IronTaskEndPointConfig : IIronTaskEndPointConfig
    {
        public IronTaskEndPointConfig(IronClientConfig config)
        {
            Config = config;
            TokenContainer = new IronConfigTokenContainer(config);
        }

        public IronClientConfig Config { get; protected set; }

        public ITokenContainer TokenContainer { get; set; }
    }
}