// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.Utilities;

namespace Trinity.Configuration
{
    /// <summary>
    /// Contains settings for the configuration section "Storage".
    /// </summary>
    public sealed class LoggingConfig
    {
        #region Singleton
        static LoggingConfig s_instance = new LoggingConfig();
        private LoggingConfig() { CTrinityConfig.CLogSetEchoOnConsole(false);  LoggingLevel = c_DefaultLogLevel; }
        /// <summary>
        /// Gets the configuration entry singleton instance.
        /// </summary>
        [ConfigInstance]
        public static LoggingConfig Instance { get { return s_instance; } }
        [ConfigEntryName]
        internal static string ConfigEntry { get { return ConfigurationConstants.Tags.LOGGING.LocalName; } }
        #endregion

        #region Fields
        internal const  LogLevel c_DefaultLogLevel  = LogLevel.Info;
        internal const  bool     c_DefaultEchoOnConsole    = ConfigurationConstants.Values.DEFAULT_VALUE_TRUE;
        private LogLevel         m_LogLevel         = c_DefaultLogLevel;
        private string           m_LogDir           = ConfigurationConstants.Values.BLANK;
        private bool?            m_EchoOnConsole    = ConfigurationConstants.Values.DEFAULT_VALUE_TRUE;
        private bool             m_LogToFile        = ConfigurationConstants.Values.DEFAULT_VALUE_TRUE;
        #endregion

        #region Private helpers
        private static string DefaultLogDirectory
        {
            get
            {
                return Path.Combine(AssemblyUtility.MyAssemblyPath, "trinity-log");
            }
        }

        private static void ThrowCreatingLogDirectoryException(string log_dir)
        {
            throw new IOException("WARNNING: Error occurs when creating LogDirectory: " + log_dir);
        }

        #endregion

        /// <summary>
        /// Represents the logging level threshold
        /// </summary>
        [ConfigSetting(Optional: true)]
        public LogLevel LoggingLevel
        {
            get { return m_LogLevel; }
            set { m_LogLevel = value; CTrinityConfig.CLogSetLogLevel(m_LogLevel); }
        }

        /// <summary>
        /// Represents the path to store log files. defaults to AssemblyPath\trinity-log\.
        /// </summary>
        [ConfigSetting(Optional: true)]
        public string LogDirectory
        {
            get
            {
                if (m_LogDir == null || m_LogDir.Length == 0)
                    m_LogDir = DefaultLogDirectory;

                return m_LogDir;
            }
            set
            {
                m_LogDir = value;

                if (m_LogDir == null || m_LogDir.Length == 0)
                {
                    m_LogDir = DefaultLogDirectory;
                }

                Log.SetLogDirectory(m_LogDir);
            }
        }

        /// <summary>
        /// Represents a value indicating whether the logged messages are echoed to the Console.
        /// if the value is true, it will be echoed to the Console.
        /// </summary>
        [ConfigSetting(Optional: true)]
        public bool LogEchoOnConsole
        {
            get
            {
                //TODO these config props really need to be reactive
                if (!m_EchoOnConsole.HasValue) { LogEchoOnConsole = c_DefaultEchoOnConsole; }
                return m_EchoOnConsole.Value;
            }
            set { m_EchoOnConsole = value; CTrinityConfig.CLogSetEchoOnConsole(value); }
        }

        /// <summary>
        /// Represents a value indicating whether to store log entries to a file on disk.
        /// if the value is true, it will be stored to disk. 
        /// </summary>
        [ConfigSetting(Optional: true)]
        public bool LogToFile
        {
            get { return m_LogToFile; }
            set { m_LogToFile = value; Log.SetLogToFile(value); }
        }
    }
}
