using System;

namespace IronIO.Core
{
    public class IronIORestException : IronIOException
    {
        public IronTaskRequestBuilder Builder { get; set; }
        public object Result { get; set; }

        public IronIORestException(string message, IronTaskRequestBuilder builder, object result) : base(message)
        {
            Builder = builder;
            Result = result;
        }

        public IronIORestException(string message, IronTaskRequestBuilder builder, object result, Exception innerException) : base(message, innerException)
        {
            Builder = builder;
            Result = result;
        }
    }
}