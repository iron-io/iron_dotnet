using System.Threading.Tasks;

namespace IronIO.Core
{
    public interface ITokenContainer
    {
        AuthToken GetWebHookToken(string tokenId);
        AuthToken GetToken();
        Task<AuthToken> GetTokenAsync();
    }
}