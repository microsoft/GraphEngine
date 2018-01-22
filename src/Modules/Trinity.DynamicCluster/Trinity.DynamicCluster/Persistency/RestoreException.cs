using System;
using System.Runtime.Serialization;

namespace Trinity.DynamicCluster.Storage
{
    [Serializable]
    internal class RestoreException : Exception
    {
        public RestoreException()
        {
        }

        public RestoreException(string message) : base(message)
        {
        }

        public RestoreException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RestoreException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}