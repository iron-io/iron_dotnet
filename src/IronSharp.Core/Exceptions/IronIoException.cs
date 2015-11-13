using System;

namespace IronIO.Core
{
    public class IronIOException : ApplicationException
    {
        public IronIOException(string message) : base(message)
        {
        }

        public IronIOException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}