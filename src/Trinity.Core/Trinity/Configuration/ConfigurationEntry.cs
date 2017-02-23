using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Configuration
{
    /// <summary>
    /// Represents a configuration entry. A configuration entry is a collection
    /// of settings that specify the behavior of a subsystem or module, for example
    /// storage, networking, etc.
    /// </summary>
    public class ConfigurationEntry
    {
        //TODO: add a link for Trinity Configuration File Format Specification v1.
        /// <summary>
        /// Constructs a new configuration entry from an XML element.
        /// </summary>
        /// <param name="entry">The XML element.</param>
        public ConfigurationEntry(System.Xml.Linq.XElement entry)
        {
            Name = entry.Name.LocalName;
            var _settings = new Dictionary<string, ConfigurationSetting>();
            foreach(var attribute in entry.Attributes())
            {
                _settings.Add(attribute.Name.LocalName, new ConfigurationSetting(attribute));
            }
            Settings = _settings;
        }

        // For compatibility with legacy code.
        internal ConfigurationEntry(string name, Dictionary<string, string> entry)
        {
            Name = name;
            var _settings = new Dictionary<string, ConfigurationSetting>();
            foreach(var kvp in entry)
            {
                _settings.Add(kvp.Key, new ConfigurationSetting(kvp.Key, kvp.Value));
            }
            Settings = _settings;
        }

        /// <summary>
        /// Represents the name of the configuration entry.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Represents the collection of settings in the configuration entry.
        /// </summary>
        public IReadOnlyDictionary<string, ConfigurationSetting> Settings { get; private set; }
        
        //TODO children entry support.
        //public IDictionary<string, ConfigurationEntry> ChildrenEntries { get; set; }
    }
}
