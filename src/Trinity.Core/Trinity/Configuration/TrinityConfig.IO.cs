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
        private const string c_defaultConfigFile = ConfigurationConstants.Values.DEFAULT_CONFIG_FILE;
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

        #region Helpers
        /// <summary>
        /// null node, name or value is ignored.
        /// </summary>
        private static void SetAttribute(XElement node, string name, string value)
        {
            if (node == null || name == null || value == null) return;

            try
            {
                if (bool.Parse(value))
                    value = value.ToUpper();
            }
            catch { }
            node.SetAttributeValue(name, value);
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

        /// <summary>
        /// Extracts config entries from the config instances.
        /// </summary>
        internal static IEnumerable<XElement> ExtractConfigurationSettings()
        {
            return GetConfigurationInstances().Select(ConfigurationEntry.ExtractConfigurationEntry).OfType<XElement>();
        }

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

                config_entry.Apply(config_instance.Instance);
            }
        }

        /// <summary>
        /// Gets all the configuration instances
        /// </summary>
        /// <returns></returns>
        internal static List<ConfigurationInstance> GetConfigurationInstances()
        {
            return Enumerable.OfType<ConfigurationInstance>(AssemblyUtility.GetAllClassInstances<object>(CreateConfigurationInstance)).ToList();
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


        private static XElement CreateServerSetting(ServerInfo server, XName tag)
        {
            XElement serverNode = new XElement(tag);
            SetAttribute(serverNode, ConfigurationConstants.Attrs.ENDPOINT, String.Format("{0}:{1}", server.HostName, server.Port));
            SetAttribute(serverNode, ConfigurationConstants.Attrs.ASSEMBLY_PATH, server.AssemblyPath);
            SetAttribute(serverNode, ConfigurationConstants.Attrs.AVAILABILITY_GROUP, server.Id);

            foreach(var entry in Enumerable.OfType<XElement>(server.Values))
            {
                serverNode.Add(entry);
            }

            return serverNode;
        }

        private static IEnumerable<XElement> CreateServerSettingList(List<AvailabilityGroup> AGs, XName tag)
        {
            return AGs.SelectMany(ag => ag.Instances.Select(_ => CreateServerSetting(_, tag)));
        }

        private static ServerInfo GetLocalConfiguration()
        {
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

            return entries;
        }

        private static bool IsDefaultClusterConfiguration(string ID)
        {
            return ID == null || ID == "";
        }

        private static bool IsDefaultClusterConfiguration(XElement _)
        {
            var attr = _.Attribute(ConfigurationConstants.Attrs.ID);
            return IsDefaultClusterConfiguration(attr?.Value);
        }
        #endregion

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
                string config_dir = Path.GetDirectoryName(configFile);
                if (config_dir == string.Empty) config_dir = Global.MyAssemblyPath;
                FileUtility.CompletePath(config_dir, true);
                configFile = Path.Combine(config_dir, Path.GetFileName(configFile));

                if (File.Exists(configFile))
                {
                    try { File.Delete(configFile); }
                    catch (Exception) { }
                }

                #region create basic xml info
                XDocument configXml = new XDocument();
                configXml.Declaration = new XDeclaration("1.0", "utf-8", null);
                XElement rootNode = new XElement(ConfigurationConstants.Tags.ROOT_NODE);
                SetAttribute(rootNode, ConfigurationConstants.Attrs.CONFIG_VERSION, ConfigurationConstants.Values.CURRENTVER);
                configXml.Add(rootNode);
                XElement localNode = new XElement(ConfigurationConstants.Tags.LOCAL);
                rootNode.Add(localNode);
                #endregion

                foreach (var setting in ExtractConfigurationSettings())
                {
                    localNode.Add(setting);
                }

                foreach (var cluster in s_clusterConfigurations)
                {
                    XElement clusterNode = new XElement(ConfigurationConstants.Tags.CLUSTER);
                    if (!IsDefaultClusterConfiguration(cluster.Key))
                        SetAttribute(clusterNode, ConfigurationConstants.Attrs.ID, cluster.Key);

                    var servers = Enumerable.Concat(
                        CreateServerSettingList(cluster.Value.Servers, ConfigurationConstants.Tags.SERVER),
                        CreateServerSettingList(cluster.Value.Proxies, ConfigurationConstants.Tags.PROXY));

                    foreach(var server in servers)
                        clusterNode.Add(server);

                    rootNode.Add(clusterNode);
                }

                configXml.Save(configFile);
            }
            catch (Exception e)
            {
                Log.WriteLine(LogLevel.Error, e.ToString());
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

                if (config.RootConfigVersion == ConfigurationConstants.Values.LEGACYVER)
                {
                    LoadConfigLegacy(trinity_config_file);
                }
                else if (config.RootConfigVersion == ConfigurationConstants.Values.CURRENTVER)
                {
                    LoadConfigCurrrentVer(config);
                }
                else
                {
                    throw new TrinityConfigException("Unrecognized " + ConfigurationConstants.Attrs.CONFIG_VERSION);
                }

                ApplyConfigurationSettings(GetLocalConfiguration());

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
            loggingEntry.SetAttributeValue(ConfigurationConstants.Attrs.LOGGING_DIRECTORY, xml_config.GetEntryValue(ConfigurationConstants.Tags.LOGGING.LocalName, ConfigurationConstants.Attrs.LOGGING_DIRECTORY));
            loggingEntry.SetAttributeValue(ConfigurationConstants.Attrs.LOGGING_LEVEL, xml_config.GetEntryValue(ConfigurationConstants.Tags.LOGGING.LocalName, ConfigurationConstants.Attrs.LOGGING_LEVEL));
            storageEntry.SetAttributeValue(ConfigurationConstants.Attrs.STORAGE_ROOT, xml_config.GetEntryValue(ConfigurationConstants.Tags.STORAGE.LocalName, ConfigurationConstants.Attrs.STORAGE_ROOT));
            networkEntry.SetAttributeValue(ConfigurationConstants.Attrs.HTTP_PORT, xml_config.GetEntryValue(ConfigurationConstants.Tags.NETWORK.LocalName, ConfigurationConstants.Attrs.HTTP_PORT));
            networkEntry.SetAttributeValue(ConfigurationConstants.Attrs.CLIENT_MAX_CONN, xml_config.GetEntryValue(ConfigurationConstants.Tags.NETWORK.LocalName, ConfigurationConstants.Attrs.CLIENT_MAX_CONN));
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

            s_clusterConfigurations.Add(ConfigurationConstants.Values.DEFAULT_CLUSTER, s_current_cluster_config);
        }

        private static void LoadConfigCurrrentVer(XmlConfiguration config)
        {
            s_localConfigurationSection = new ConfigurationSection(config.LocalSection);
            var clusterSections = config.ClusterSections;
            foreach (var clusterSection in clusterSections)
            {
                if (!IsDefaultClusterConfiguration(clusterSection))
                {
                    s_clusterConfigurations.Add(clusterSection.Attribute(ConfigurationConstants.Attrs.ID).Value, new ClusterConfig(clusterSection));
                }
            }

            // The default cluster config is the one without an Id.
            s_current_cluster_config = 
                new ClusterConfig(
                    clusterSections.FirstOrDefault(IsDefaultClusterConfiguration) ??
                    new XElement(ConfigurationConstants.Tags.CLUSTER));

            s_clusterConfigurations.Add(ConfigurationConstants.Values.DEFAULT_CLUSTER, s_current_cluster_config);
        }

    }
}
