namespace IronIO.Core
{
    public interface IIronTaskEndpointConfig
    {
        IronClientConfig Config { get; }
        ITokenContainer TokenContainer { get; }
    }
}