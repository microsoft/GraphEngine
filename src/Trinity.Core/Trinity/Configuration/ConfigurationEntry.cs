using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Trinity.Configuration
{
    /// <summary>
    /// Represents a configuration entry. A configuration entry is a collection
    /// of settings that specify the behavior of a subsystem or module, for example
    /// storage, networking, etc.
    /// </summary>
    public class ConfigurationEntry
    {
        Dictionary<string, ConfigurationSetting> m_settings;
        List<ConfigurationEntry> m_children;

        /// <summary>
        /// Represents the name of the configuration entry.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Represents the collection of settings in the configuration entry.
        /// </summary>
        public IReadOnlyDictionary<string, ConfigurationSetting> Settings { get { return m_settings; } }

        /// <summary>
        /// Represents the children configuration entries.
        /// </summary>
        public IReadOnlyList<ConfigurationEntry> Children { get { return m_children; } }

        internal ConfigurationEntry(string name)
        {
            Name = name;
            m_settings = new Dictionary<string, ConfigurationSetting>();
            m_children = new List<ConfigurationEntry>();
        }

        //TODO: add a link for Trinity Configuration File Format Specification v1.
        /// <summary>
        /// Constructs a new configuration entry from an XML element.
        /// </summary>
        /// <param name="entry">The XML element.</param>
        public ConfigurationEntry(System.Xml.Linq.XElement entry) :
            this(entry.Name.LocalName)
        {
            foreach (var attribute in entry.Attributes())
            {
                m_settings.Add(attribute.Name.LocalName, new ConfigurationSetting(attribute));
            }

            foreach (var child in entry.Descendants())
            {
                m_children.Add(new ConfigurationEntry(child));
            }
        }

        // For compatibility with legacy code.
        internal ConfigurationEntry(string name, Dictionary<string, string> entry)
            : this(name)
        {
            foreach (var kvp in entry)
            {
                AddSetting(kvp.Key, kvp.Value);
            }
        }

        private void AddSetting(string key, string value)
        {
            m_settings.Add(key, new ConfigurationSetting(key, value));
        }

        /// <summary>
        /// Converts a configuration entry back to xml.
        /// </summary>
        public static explicit operator XElement(ConfigurationEntry entry)
        {
            XElement node = new XElement(ConfigurationConstants.NS + entry.Name);
            foreach (var attr in entry.Settings)
            {
                node.SetAttributeValue(attr.Key, attr.Value.Literal);
            }
            foreach (var child in entry.Children)
            {
                node.Add((XElement)child);
            }

            return node;
        }

        internal void Apply(object instance)
        {
            var props = GetConfigProperties(instance);
            var settings = from prop in props
                           join setting in m_settings.Values
                           on prop.Name equals setting.Key
                           select Tuple.Create(prop, setting);

            var children = from prop in props
                           join child in m_children
                           on prop.Name equals child.Name
                           group child by prop;

            var mandatory_props = GetMandatoryConfigProperties(instance);
            foreach (var p in mandatory_props)
            {
                if (!ContainsSetting(p.Name))
                {
                    throw new InvalidOperationException($"Missing non-optional setting '{p.Name}'");
                }
            }

            foreach (var tuple in settings)
            {
                var prop = tuple.Item1;
                var setting = tuple.Item2;
                try
                {
                    prop.SetValue(instance, setting.GetValue(prop.PropertyType));
                }
                catch
                {
                    //TODO log down the error?
                }
            }

            foreach (var group in children)
            {
                var prop = group.Key;
                var l = MakeList(prop);
                foreach (var child in group)
                {
                    l.Add(MakeElement(prop, child));
                }
                prop.SetValue(instance, l);
            }
        }

        private bool ContainsSetting(string name)
        {
            return m_settings.ContainsKey(name) || m_children.Any(_ => _.Name == name);
        }

        private object MakeElement(PropertyInfo prop, ConfigurationEntry child)
        {
            Type t = prop.PropertyType.GetGenericArguments()[0];
            object o = t.GetConstructor(new Type[] { }).Invoke(new object[] { });
            child.Apply(o);
            return o;
        }

        private IList MakeList(PropertyInfo prop)
        {
            return prop.PropertyType.GetConstructor(new Type[] { }).Invoke(new object[] { }) as IList;
        }

        /// <summary>
        /// Extracts settings from a configuration instance.
        /// </summary>
        internal static ConfigurationEntry ExtractConfigurationEntry(ConfigurationInstance instance)
        {
            return ExtractConfigurationEntry(instance.Instance, instance.EntryName);
        }

        private static ConfigurationEntry ExtractConfigurationEntry(object instance, string name)
        {
            ConfigurationEntry entry = new ConfigurationEntry(name);

            foreach (var prop in GetConfigProperties(instance))
            {
                if (IsChild(prop))
                {
                    var children = prop.GetValue(instance) as IEnumerable;
                    if (children == null) continue;
                    foreach (object child in children)
                    {
                        entry.m_children.Add(ExtractConfigurationEntry(child, prop.Name));
                    }
                }
                else
                {
                    entry.AddSetting(prop.Name, prop.GetValue(instance).ToString());
                }
            }

            return entry;
        }

        private static IEnumerable<PropertyInfo> GetConfigProperties(object instance)
        {
            return instance.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(_ => _.GetCustomAttribute<ConfigSettingAttribute>() != null);
        }

        private static IEnumerable<PropertyInfo> GetMandatoryConfigProperties(object instance)
        {
            return instance.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(_ => { var attr = _.GetCustomAttribute<ConfigSettingAttribute>(); return attr != null && !attr.Optional; });
        }


        private static ConfigurationEntry ExtractConfigurationEntry(object value, PropertyInfo prop)
        {
            ConfigurationEntry entry = new ConfigurationEntry(prop.Name);
            throw new NotImplementedException();
        }

        private static bool IsChild(PropertyInfo prop)
        {
            var t = prop.PropertyType;
            return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>));
        }
    }
}
