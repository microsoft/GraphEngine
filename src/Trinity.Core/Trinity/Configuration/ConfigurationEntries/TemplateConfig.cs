using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Configuration
{
    internal sealed class TemplateConfig
    {
        static TemplateConfig templateConfig = new TemplateConfig();
        private TemplateConfig() { }
        [ConfigInstance]
        internal static TemplateConfig Instance { get { return templateConfig; } }
        [ConfigEntryName]
        internal static string ConfigEntry { get { return "Template"; } }

        [ConfigSetting(Optional: false)]
        public int Id
        {
            get;
            set;
        }
    }
}
