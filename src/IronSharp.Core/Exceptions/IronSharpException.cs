using System;

namespace IronIO.Core
{
    public class IronSharpException : ApplicationException
    {
        public IronSharpException(string message) : base(message)
        {
        }

        public IronSharpException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}