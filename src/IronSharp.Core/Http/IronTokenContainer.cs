using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronSharp.Core
{
    public class IronTokenContainer: ITokenContainer
    {
        private String token;

        public IronTokenContainer(String token)
        {
            this.token = token;
        }

        public String getToken()
        {
            return token;
        }
        
        public void setToken(String token)
        {
            this.token = token;
        }
    }
}
