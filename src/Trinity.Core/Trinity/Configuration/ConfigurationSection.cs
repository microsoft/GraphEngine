using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Trinity.Network;
using Trinity.Utilities;

namespace Trinity.Configuration
{
    /// <summary>
    /// Contains the information for a configuration section.
    /// </summary>
    public class ConfigurationSection : Dictionary<string, ConfigurationEntry>
    {
        internal ConfigurationSection() { }

        internal ConfigurationSection(XElement configSection)
        {
            foreach (var entry in configSection.Elements())
            {
                this.Add(entry.Name.LocalName, new ConfigurationEntry(entry));
            }
        }
        /// <summary>
        /// Merge the other configuration section into this configuration section.
        /// Entries with the same key in the other will override entries in this section.
        /// </summary>
        /// <param name="other">The other configuration section.</param>
        internal void Merge(ConfigurationSection other)
        {
            foreach (var kvp in other)
            {
                this[kvp.Key] = kvp.Value;
            }
        }
    }
}
