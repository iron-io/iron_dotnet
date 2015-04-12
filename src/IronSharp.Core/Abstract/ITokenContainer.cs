using System.Threading.Tasks;

namespace IronSharp.Core
{
    public interface ITokenContainer
    {
        AuthToken GetToken();
        Task<AuthToken> GetTokenAsync();
    }
}