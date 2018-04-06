using System;
using System.Runtime.Serialization;

namespace GraphEngine.Jit.Native
{
    [Serializable]
    internal class AsmJitException : Exception
    {
        public AsmJitException()
        {
        }

        public AsmJitException(string message) : base(message)
        {
        }

        public AsmJitException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AsmJitException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}