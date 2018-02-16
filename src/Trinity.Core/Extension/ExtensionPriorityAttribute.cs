using System;

namespace Trinity.Extension
{
    /// <summary>
    /// Specifies the default extension loading priority for a type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ExtensionPriorityAttribute : Attribute
    {
        /// <summary>
        /// The priority of the type;
        /// </summary>
        public int Priority { get; private set; }
        /// <summary>
        /// Specifies the default extension loading priority for a type.
        /// </summary>
        /// <param name="priority">The priority of the type.</param>
        public ExtensionPriorityAttribute(int priority)
        {
            Priority = priority;
        }
    }
}
