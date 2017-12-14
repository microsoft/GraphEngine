using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Configuration
{
    /// <summary>
    /// Represents a property of a configuration entry.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=false)]
    public class ConfigSettingAttribute : Attribute
    {
        /// <summary>
        /// Constructs a setting attribute.
        /// </summary>
        public ConfigSettingAttribute() { Optional = false; }
        /// <summary>
        /// Constructs a setting attribute, and specify if this property is optional
        /// </summary>
        /// <param name="Optional"></param>
        public ConfigSettingAttribute(bool Optional) { this.Optional = Optional; }
        /// <summary>
        /// Represents whether this setting is optional
        /// </summary>
        public bool Optional { get; internal set; }
    }

    /// <summary>
    /// Represents a public static instance configuration entry singleton.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=false)]
    public class ConfigInstanceAttribute : Attribute { }

    /// <summary>
    /// Represents a static string property indicating the name of the configuration entry.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=false)]
    public class ConfigEntryNameAttribute : Attribute { }
}
