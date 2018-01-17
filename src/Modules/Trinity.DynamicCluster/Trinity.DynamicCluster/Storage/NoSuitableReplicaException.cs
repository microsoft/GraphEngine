using System;
using System.Runtime.Serialization;

namespace Trinity.DynamicCluster.Storage
{
    [Serializable]
    internal class NoSuitableReplicaException : Exception
    {
        public NoSuitableReplicaException()
        {
        }

        public NoSuitableReplicaException(string message) : base(message)
        {
        }

        public NoSuitableReplicaException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoSuitableReplicaException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}