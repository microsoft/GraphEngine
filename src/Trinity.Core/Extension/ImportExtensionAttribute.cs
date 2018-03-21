using System;
using System.Collections.Generic;
using System.Text;

namespace Trinity.Extension
{
    /// <summary>
    /// Declares dependency on a Graph Engine extension.
    /// This attribute should be used for extension composition
    /// and dependency injection, when the target extension
    /// is not explicitly used elsewhere in your project.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class ImportExtensionAttribute : Attribute
    {
        /// <summary>
        /// Declares dependency on a Graph Engine extension.
        /// </summary>
        /// <param name="type">A type defined in the target extension assembly</param>
        public ImportExtensionAttribute(Type type) { }
    }
}
