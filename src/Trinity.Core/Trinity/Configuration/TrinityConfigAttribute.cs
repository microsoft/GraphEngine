using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Configuration
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=false)]
    public class ConfigSettingAttribute : Attribute
    {
        public ConfigSettingAttribute() { Optional = false; }
        public ConfigSettingAttribute(bool Optional) { this.Optional = Optional; }
        public bool Optional { get; internal set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=false)]
    public class ConfigInstanceAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=false)]
    public class ConfigEntryNameAttribute : Attribute { }
}
