using System;

namespace Trinity.SampleApplication.ServiceFabric
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    internal class UseExtensionAttribute : Attribute
    {
        public UseExtensionAttribute(Type _)
        {
        }
    }
}

