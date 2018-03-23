using System;

namespace Trinity.FFI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FFIPrefixAttribute : Attribute
    {
        public FFIPrefixAttribute(string prefix) { Prefix = prefix; }

        public string Prefix { get; }
    }
}