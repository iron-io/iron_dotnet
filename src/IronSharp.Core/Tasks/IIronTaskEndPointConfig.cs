namespace IronIO.Core
{
    public interface IIronTaskEndPointConfig
    {
        IronClientConfig Config { get; }
        ITokenContainer TokenContainer { get; }
    }
}