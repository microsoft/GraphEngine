// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.IO;

using Trinity.Utilities;
using Trinity.Diagnostics;
using System.Reflection;

namespace Trinity
{
    public static partial class TrinityConfig
    {
        private const string default_config_file = "trinity.xml";
        private static string MyAssemblyPath
        {
            get
            {
                return System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar;
            }
        }

        internal static string DefaultConfigFile
        {
            get
            {
                return MyAssemblyPath + default_config_file;
            }
        }

        /// <summary>
        /// Save current configuration to the default config file trinity.xml. The default location of trinity.xml is the directory containing current Trinity assembly.
        /// </summary>
        public static void SaveConfig()
        {
            SaveConfig(Path.Combine(MyAssemblyPath, default_config_file));
        }

        /// <summary>
        /// Save current configuration to the specified file.
        /// </summary>
        /// <param name="configFile">The config file to which the current configuration is written to.</param>
        public static void SaveConfig(string configFile)
        {
            if (File.Exists(configFile))
            {
                try
                {
                    File.Delete(configFile);
                }
                catch (Exception) { }
            }

            FileUtility.CompletePath(Path.GetDirectoryName(configFile), true);

            XMLConfig xml_config = new XMLConfig(configFile);
            bool updated = false;

            if (storage_root != null && storage_root.Length != 0 && (!DefaultStorageRoot.Equals(storage_root))) //! Do not change the storage_root to StorageRoot.
            {
                xml_config.SetEntryValue("Storage", "StorageRoot", storage_root);
                updated = true;
            }

            if(LogDirectory != DefaultLogDirectory)
            {
                xml_config.SetEntryValue("Logging", "LogDirectory", LogDirectory);
                updated = true;
            }

            if (LoggingLevel != c_DefaultLogLevel)
            {
                xml_config.SetEntryValue("Logging", "LoggingLevel", LoggingLevel);
                updated = true;
            }

            if (updated)
                xml_config.Save();
        }

        /// <summary>
        /// Load configuration from the default config file trinity.xml. The default location of trinity.xml is the directory containing current Trinity assembly.
        /// </summary>
        public static void LoadConfig()
        {
            LoadTrinityConfig(true);
        }

        /// <summary>
        /// Load configuration from the specified configuration file.
        /// </summary>
        /// <param name="configFile">The config file to read.</param>
        public static void LoadConfig(string configFile)
        {
            LoadTrinityConfig(configFile, true);
        }

        internal static void LoadTrinityConfig(bool forceLoad = false)
        {
            LoadTrinityConfig(DefaultConfigFile, forceLoad);
        }

        internal static void LoadTrinityConfig(string trinity_config_file, bool forceLoad = false)
        {
            lock (config_load_lock)
            {
                try
                {
                    if (is_config_loaded && !forceLoad)
                        return;
                    if (!File.Exists(trinity_config_file))
                        return;

                    XMLConfig xml_config = new XMLConfig(trinity_config_file);
                    LogDirectory = xml_config.GetEntryValue("Logging", "LogDirectory", DefaultLogDirectory);
                    LoggingLevel = (LogLevel)Enum.Parse(typeof(LogLevel), xml_config.GetEntryValue("Logging", "LoggingLevel", c_DefaultLogLevel.ToString()), true);
                    StorageRoot  = xml_config.GetEntryValue("Storage", "StorageRoot", DefaultStorageRoot);                    
                   
                    current_cluster_config = new ClusterConfig(trinity_config_file);
                    is_config_loaded = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("There are errors in your configuration file.");
                    Console.WriteLine(e.ToString());
                    Environment.Exit(1);
                }
            }
        }

        internal static string OutputCurrentConfig()
        {
            CodeWriter cw = new CodeWriter();
            cw.WL();
            cw.WL("StorageRoot : {0}", StorageRoot);
            cw.WL("LogDirectory: {0}", LogDirectory);
            cw.WL("LoggingLevel: {0}", LoggingLevel);
            cw.WL();
            return cw.ToString();
        }
    }
}
