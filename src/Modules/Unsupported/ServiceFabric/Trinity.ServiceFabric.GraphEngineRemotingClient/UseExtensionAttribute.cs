using System;

namespace Trinity.ServiceFabric.GraphEngineRemotingClient
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    internal class UseExtensionAttribute : Attribute
    {
        public UseExtensionAttribute(Type _) { }
    }
}
