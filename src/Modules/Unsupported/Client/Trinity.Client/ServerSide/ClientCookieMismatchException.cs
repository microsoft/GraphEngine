using System;
using System.Runtime.Serialization;

namespace Trinity.Client.TrinityClientModule
{
    [Serializable]
    public class ClientCookieMismatchException : Exception
    {
        public ClientCookieMismatchException()
        {
        }

        public ClientCookieMismatchException(string message) : base(message)
        {
        }

        public ClientCookieMismatchException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ClientCookieMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}