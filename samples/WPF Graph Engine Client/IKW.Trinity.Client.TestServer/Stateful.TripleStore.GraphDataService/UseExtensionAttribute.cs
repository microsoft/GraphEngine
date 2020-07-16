using System;

namespace Stateful.TripleStore.GraphDataService
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    internal class UseExtensionAttribute : Attribute
    {
        public UseExtensionAttribute(Type _) { }
    }
}
