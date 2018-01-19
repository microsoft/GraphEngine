using System;
using System.Runtime.Serialization;

namespace Trinity.DynamicCluster.Storage
{
    [Serializable]
    internal class NotMasterException : Exception
    {
        public NotMasterException()
        {
        }

        public NotMasterException(string message) : base(message)
        {
        }

        public NotMasterException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotMasterException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}