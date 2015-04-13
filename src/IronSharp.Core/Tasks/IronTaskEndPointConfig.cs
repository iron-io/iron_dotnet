namespace IronIO.Core
{
    public class IronTaskEndpointConfig : IIronTaskEndpointConfig
    {
        public IronTaskEndpointConfig(IronClientConfig config, ITokenContainer tokenContainer = null)
        {
            if (tokenContainer == null)
            {
                tokenContainer = new IronConfigTokenContainer(config);
            }

            Config = config;
            TokenContainer = tokenContainer;
        }

        public IronClientConfig Config { get; protected set; }

        public ITokenContainer TokenContainer { get; set; }
    }
}