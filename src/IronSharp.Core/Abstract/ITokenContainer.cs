using System.Threading.Tasks;

namespace IronIO.Core
{
    public interface ITokenContainer
    {
        AuthToken GetToken();
        Task<AuthToken> GetTokenAsync();
    }
}