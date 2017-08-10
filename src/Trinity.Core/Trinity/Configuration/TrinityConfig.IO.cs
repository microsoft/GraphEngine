// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.IO;
using System.Net;
using Trinity.Utilities;
using Trinity.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Trinity.Configuration;
using System.Collections.ObjectModel;
using System.Xml;
using Trinity.Network;
namespace Trinity
{
    public static partial class TrinityConfig
    {
        #region Fields
        private const string c_defaultConfigFile = ConfigurationConstants.Tags.DEFAULT_CONFIG_FILE;
        private static readonly string[] c_builtInSectionNames = new string[] { ConfigurationConstants.Tags.SERVER, ConfigurationConstants.Tags.PROXY };
        internal static ConfigurationSection s_localConfigurationSection = new ConfigurationSection();
        #endregion

        #region Property
        /// <summary>
        /// Gets path of the default configuration file.
        /// </summary>
        internal static string DefaultConfigFile
        {
            get
            {
                return Path.Combine(AssemblyUtility.MyAssemblyPath, c_defaultConfigFile);
            }
        }
        #endregion

        /// <summary>
        /// Applies the configuration settings from the given dictionary of setting entries, into the configuration object.
        /// </summary>
        /// <param name="entries"></param>
        internal static void ApplyConfigurationSettings(Dictionary<string, ConfigurationEntry> entries)
        {
            foreach (var config_instance in GetConfigurationInstances())
            {
                if (!entries.ContainsKey(config_instance.EntryName)) { continue; }
                ConfigurationEntry config_entry = entries[config_instance.EntryName];

                var config_setting_props = config_instance.Type
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(_ => _.GetCustomAttribute<ConfigSettingAttribute>() != null);

                foreach (var config_setting in config_setting_props)
                {
                    if (config_entry.Settings != null && config_entry.Settings.ContainsKey(config_setting.Name))
                    {
                        try
                        {
                            config_setting.SetValue(config_instance.Instance, config_entry.Settings[config_setting.Name].GetValue(config_setting.PropertyType));
                        }
                        catch
                        {
                            //TODO log down the error?
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Gets all the configuration instances
        /// </summary>
        /// <returns></returns>
        internal static List<ConfigurationInstance> GetConfigurationInstances()
        {
            return AssemblyUtility.GetAllClassInstances(CreateConfigurationInstance).ToList();
        }

        /// <summary>
        /// A type is a configuration entry type if:
        ///   1. It contains a static property annotated with [ConfigInstance], returning an instance of the type
        ///   2. It contains a static property annotated with [ConfigEntryName], returning a string
        /// </summary>
        private static ConfigurationInstance CreateConfigurationInstance(Type type)
        {
            do
            {
                try
                {
                    var static_properties = type.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    var singleton_instance_prop = static_properties.FirstOrDefault(_ => _.GetCustomAttribute<ConfigInstanceAttribute>() != null && _.PropertyType == type);
                    if (singleton_instance_prop == null) break;
                    var entry_name_prop = static_properties.FirstOrDefault(_ => _.GetCustomAttribute<ConfigEntryNameAttribute>() != null && _.PropertyType == typeof(String));
                    if (entry_name_prop == null) break;
                    // conditions satisfied.
                    object singleton_instance = singleton_instance_prop.GetValue(null);
                    string entry_name = (string)entry_name_prop.GetValue(null);
                    return new ConfigurationInstance(singleton_instance, entry_name, type);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "An error occured while searching for configuration sections: {0}", ex.ToString());
                }
            } while (false);
            return null;
        }

        /// <summary>
        /// Save current configuration to the default config file trinity.xml. The default location of trinity.xml is the directory containing current Trinity assembly.
        /// </summary>
        public static void SaveConfig()
        {
            try
            {
                SaveConfig(DefaultConfigFile);
            }
            catch (Exception e)
            {
                Log.WriteLine(LogLevel.Error, e.Message);
                throw;
            }
        }


        /// <summary>
        /// Save current configuration to the specified file.
        /// </summary>
        /// <param name="configFile">The config file to which the current configuration is written to.</param>
        public static void SaveConfig(string configFile)
        {
            try
            {
                if (File.Exists(configFile))
                {
                    try
                    {
                        File.Delete(configFile);
                    }
                    catch (Exception) { }
                }
                string dir = Path.GetDirectoryName(configFile);
                if (dir == string.Empty) dir = Global.MyAssemblyPath;

                FileUtility.CompletePath(Path.GetDirectoryName(configFile), true);
                #region create basic xml info
                XmlDocument configXml = new XmlDocument();
                XmlDeclaration declaration = configXml.CreateXmlDeclaration("1.0", "utf-8", null);
                XmlNode rootNode = configXml.CreateElement(ConfigurationConstants.Tags.ROOT_NODE);
                XmlAttribute version = configXml.CreateAttribute(ConfigurationConstants.Attrs.CONFIG_VERSION);
                version.Value = ConfigurationConstants.Tags.CURRENTVER;
                rootNode.Attributes.Append(version);
                #endregion
                #region Get Local Setting Info
                XmlNode localNode = configXml.CreateElement(ConfigurationConstants.Tags.LOCAL);
                foreach (var _ in GetConfigurationInstances())
                {
                    XmlNode entry = configXml.CreateElement(_.EntryName);
                    foreach (var attribute in _.Instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        entry.Attributes.Append(SetAttribute(configXml, attribute.Name, attribute.GetValue(_.Instance).ToString()));
                    }
                    localNode.AppendChild(entry);
                }
                rootNode.AppendChild(localNode);
                #endregion
                #region Get cluster config
                foreach (var cluster in s_clusterConfigurations)
                {
                    XmlNode clusterNode = configXml.CreateElement(ConfigurationConstants.Tags.CLUSTER);
                    if (cluster.Key != ConfigurationConstants.Tags.DEFAULT_CLUSTER)
                        clusterNode.Attributes.Append(SetAttribute(configXml, ConfigurationConstants.Attrs.ID, cluster.Key));
                    var ags = new List<List<AvailabilityGroup>> { cluster.Value.Servers, cluster.Value.Proxies };

                    for (int i = 0; i < ags.Count; ++i)
                        foreach (var ag in ags[i])
                        {
                            foreach (var server in ag.Instances)
                            {
                                XmlNode serverNode = configXml.CreateElement(c_builtInSectionNames[i]);
                                serverNode.Attributes.Append(SetAttribute(configXml, ConfigurationConstants.Attrs.ENDPOINT, String.Format("{0}:{1}", server.HostName, server.Port)));
                                if (server.AssemblyPath != null)
                                    serverNode.Attributes.Append(SetAttribute(configXml, ConfigurationConstants.Attrs.ASSEMBLY_PATH, server.AssemblyPath));
                                if (server.Id != null)
                                    serverNode.Attributes.Append(SetAttribute(configXml, ConfigurationConstants.Attrs.AVAILABILITY_GROUP, server.Id));

                                var instances = GetConfigurationInstances();

                                foreach (var entry in server)
                                {
                                    var currentInstance = instances.Where(_ => _.EntryName == entry.Key).FirstOrDefault();
                                    if (currentInstance != null)
                                    {
                                        XmlNode entryNode = configXml.CreateElement(entry.Value.Name);
                                        foreach (var setting in entry.Value.Settings)
                                        {
                                            if (setting.Value.Literal != null)
                                                entryNode.Attributes.Append(SetAttribute(configXml, setting.Key, setting.Value.Literal));
                                        }
                                        if (entryNode.Attributes.Count > 0)
                                            serverNode.AppendChild(entryNode);
                                    }
                                }
                                clusterNode.AppendChild(serverNode);
                            }
                        }
                    rootNode.AppendChild(clusterNode);
                }
                #endregion
                configXml.AppendChild(rootNode);
                configXml.InsertBefore(declaration, configXml.DocumentElement);
                configXml.Save(configFile);
            }
            catch (Exception e)
            {
                Log.WriteLine(LogLevel.Error, e.Message);
                throw;
            }
        }

        /// <summary>
        /// Load configuration from the default config file trinity.xml. The default location of trinity.xml is the directory containing current Trinity assembly.
        /// </summary>
        public static void LoadConfig()
        {
            try
            {
                LoadTrinityConfig(true);
            }
            catch (Exception e)
            {
                Log.WriteLine(LogLevel.Error, e.Message);
                throw;
            }
        }

        /// <summary>
        /// Load configuration from the specified configuration file.
        /// </summary>
        /// <param name="configFile">The config file to read.</param>
        public static void LoadConfig(string configFile)
        {
            try
            {
                LoadTrinityConfig(configFile, true);
            }
            catch (Exception e)
            {
                Log.WriteLine(LogLevel.Error, e.Message);
                throw;
            }
        }

        internal static void LoadTrinityConfig(bool forceLoad = false)
        {
            LoadTrinityConfig(DefaultConfigFile, forceLoad);
        }

        internal static void LoadTrinityConfig(string trinity_config_file, bool forceLoad = false)
        {
            lock (config_load_lock)
            {
                if (is_config_loaded && !forceLoad)
                    return;
                if (!File.Exists(trinity_config_file))
                    return;

                var config = XmlConfiguration.Load(trinity_config_file);

                s_clusterConfigurations.Clear();
                s_localConfigurationSection.Clear();

                if (config.RootConfigVersion == ConfigurationConstants.Tags.LEGACYVER)
                {
                    LoadConfigLegacy(trinity_config_file);
                }
                else if (config.RootConfigVersion == ConfigurationConstants.Tags.CURRENTVER)
                {
                    s_localConfigurationSection = new ConfigurationSection(config.LocalSection);
                    var clusterSections = config.ClusterSections;
                    foreach (var clusterSection in clusterSections)
                    {
                        if (clusterSection.Attribute(ConfigurationConstants.Attrs.ID) != null)
                        {
                            s_clusterConfigurations.Add(clusterSection.Attribute(ConfigurationConstants.Attrs.ID).Value, new ClusterConfig(clusterSection));
                        }
                    }

                    // The default cluster config is the one without an Id.
                    s_current_cluster_config = new ClusterConfig(clusterSections.FirstOrDefault(
                        _ => _.Attribute(ConfigurationConstants.Attrs.ID) == null) ??
                            new XElement(ConfigurationConstants.Tags.CLUSTER));

                    s_clusterConfigurations.Add(ConfigurationConstants.Tags.DEFAULT_CLUSTER, s_current_cluster_config);
                }
                else
                {
                    throw new TrinityConfigException("Unrecognized " + ConfigurationConstants.Attrs.CONFIG_VERSION);
                }
                //LOCAL override CLUSTER 
                var entries = new Network.ServerInfo();
                var server_entries = s_current_cluster_config.GetMyServerInfo();
                var proxy_entries = s_current_cluster_config.GetMyProxyInfo();
                if (proxy_entries != null)
                    entries.Merge(proxy_entries);
                if (server_entries != null)
                    entries.Merge(server_entries);

                foreach (var entry in s_localConfigurationSection)
                {
                    entries[entry.Key] = entry.Value;
                }

                ApplyConfigurationSettings(entries);

                is_config_loaded = true;
            }
        }

        /// <summary>
        /// !Caller holds config_load_lock
        /// </summary>
        /// <param name="trinity_config_file"></param>
        private static void LoadConfigLegacy(string trinity_config_file)
        {
            XMLConfig xml_config = new XMLConfig(trinity_config_file);

            //construct local configuration section  
            XElement localSection = new XElement(ConfigurationConstants.Tags.LOCAL);
            XElement loggingEntry = new XElement(ConfigurationConstants.Tags.LOGGING);
            XElement storageEntry = new XElement(ConfigurationConstants.Tags.STORAGE);
            XElement networkEntry = new XElement(ConfigurationConstants.Tags.NETWORK);
            loggingEntry.SetAttributeValue(ConfigurationConstants.Attrs.LOGGING_DIRECTLY, xml_config.GetEntryValue(ConfigurationConstants.Tags.LOGGING, ConfigurationConstants.Attrs.LOGGING_DIRECTLY));
            loggingEntry.SetAttributeValue(ConfigurationConstants.Attrs.LOGGING_LEVEL, xml_config.GetEntryValue(ConfigurationConstants.Tags.LOGGING, ConfigurationConstants.Attrs.LOGGING_LEVEL));
            storageEntry.SetAttributeValue(ConfigurationConstants.Attrs.STORAGE_ROOT, xml_config.GetEntryValue(ConfigurationConstants.Tags.STORAGE, ConfigurationConstants.Attrs.STORAGE_ROOT));
            networkEntry.SetAttributeValue(ConfigurationConstants.Attrs.HTTP_PORT, xml_config.GetEntryValue(ConfigurationConstants.Tags.NETWORK, ConfigurationConstants.Attrs.HTTP_PORT));
            networkEntry.SetAttributeValue(ConfigurationConstants.Attrs.CLIENT_MAX_CONN, xml_config.GetEntryValue(ConfigurationConstants.Tags.NETWORK, ConfigurationConstants.Attrs.CLIENT_MAX_CONN));
            if (loggingEntry.Attributes().Count() > 0)
                localSection.Add(loggingEntry);
            if (storageEntry.Attributes().Count() > 0)
                localSection.Add(storageEntry);
            if (networkEntry.Attributes().Count() > 0)
                localSection.Add(networkEntry);

            //construct local ConfigurationSection
            s_localConfigurationSection = new ConfigurationSection(localSection);

            //construct a clusterSections
            s_current_cluster_config = ClusterConfig._LegacyLoadClusterConfig(trinity_config_file);

            s_clusterConfigurations.Add(ConfigurationConstants.Tags.DEFAULT_CLUSTER, s_current_cluster_config);
        }

        private static XmlAttribute SetAttribute(XmlDocument configXml, string attrName, string value)
        {
            XmlAttribute attribute = configXml.CreateAttribute(attrName);
            try
            {
                if (bool.Parse(value))
                    value = value.ToUpper();
            }
            catch { }
            attribute.Value = value;
            return attribute;
        }
        /// <summary>
        /// Return the main infomation of current configuration
        /// </summary>
        /// <returns></returns>
        internal static string OutputCurrentConfig()
        {
            CodeWriter cw = new CodeWriter();
            cw.WL();
            cw.WL("StorageRoot : {0}", StorageRoot);
            cw.WL("LogDirectory: {0}", LogDirectory);
            cw.WL("LoggingLevel: {0}", LoggingLevel);
            cw.WL("HttpPort:     {0}", HttpPort);
            cw.WL();
            return cw.ToString();
        }
    }
}
