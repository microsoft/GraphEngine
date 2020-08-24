using System;

namespace Trinity.SF.GraphEngineRemotingClient
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    internal class UseExtensionAttribute : Attribute
    {
        public UseExtensionAttribute(Type _) { }
    }
}

