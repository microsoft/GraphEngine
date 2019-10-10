using System;

namespace InKnowWorks.ServiceFabric.HelloWorldGraphService
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    internal class UseExtensionAttribute : Attribute
    {
        public UseExtensionAttribute(Type _)
        {
        }
    }
}

