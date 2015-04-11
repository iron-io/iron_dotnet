using System;
using System.Threading.Tasks;

namespace IronSharp.Core
{
    public interface ITokenContainer
    {
        String GetToken();
        Task<string> GetTokenAsync();
    }
}