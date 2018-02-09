using System;
using System.Runtime.Serialization;

namespace Trinity.Client.ServerSide
{
    [Serializable]
    public class ClientInstanceNotFoundException : Exception
    {
        public ClientInstanceNotFoundException()
        {
        }

        public ClientInstanceNotFoundException(string message) : base(message)
        {
        }

        public ClientInstanceNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ClientInstanceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}