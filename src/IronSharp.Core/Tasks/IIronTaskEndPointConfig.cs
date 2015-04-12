namespace IronSharp.Core
{
    public interface IIronTaskEndPointConfig
    {
        IronClientConfig Config { get; }
        ITokenContainer TokenContainer { get; }
    }
}