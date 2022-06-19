using System;
using System.Runtime.Serialization;

namespace Trinity.ServiceFabric.Infrastructure
{
    [Serializable]
    internal class ConfigurationNotLoadedException : Exception
    {
        public ConfigurationNotLoadedException() { }

        public ConfigurationNotLoadedException(string message) : base(message) { }

        public ConfigurationNotLoadedException(string message, Exception innerException) : base(message, innerException) { }

        protected ConfigurationNotLoadedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}