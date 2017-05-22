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
    public sealed class LoggingConfig
    {
        #region Singleton
        static LoggingConfig s_instance = new LoggingConfig();
        private LoggingConfig() { LoggingLevel = c_DefaultLogLevel; }
        [ConfigInstance]
        public static LoggingConfig Instance { get { return s_instance; } }
        [ConfigEntryName]
        internal static string ConfigEntry { get { return ConfigurationConstants.Tags.LOGGING; } }
        #endregion

        #region Fields
        internal const  LogLevel c_DefaultLogLevel  = LogLevel.Info;
        private LogLevel         m_LogLevel         = c_DefaultLogLevel;
        private string           m_LogDir           = ConfigurationConstants.DefaultValue.BLANK;
        private bool             m_EchoOnConsole    = ConfigurationConstants.DefaultValue.DEFAULT_VALUE_TRUE;
        private bool             m_LogToFile        =ConfigurationConstants.DefaultValue.DEFAULT_VALUE_TRUE;
        #endregion

        #region Private helpers
        private static string DefaultLogDirectory
        {
            get
            {
                return Path.Combine(AssemblyPath.MyAssemblyPath,"trinity-log");
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
        [ConfigSetting(Optional:true)]
        public LogLevel LoggingLevel
        {
            get { return m_LogLevel; }
            set { m_LogLevel = value; CTrinityConfig.CLogSetLogLevel(m_LogLevel); }
        }

        /// <summary>
        /// Represents the path to store log files. defaults to AssemblyPath\trinity-log\.
        /// </summary>
        [ConfigSetting(Optional:true)]
        public string LogDirectory
        {
            get
            {
                if (m_LogDir == null || m_LogDir.Length == 0)
                    m_LogDir = DefaultLogDirectory;


                if (!Directory.Exists(m_LogDir))
                {
                    try
                    {
                        Directory.CreateDirectory(m_LogDir);
                    }
                    catch
                    {
                        ThrowCreatingLogDirectoryException(m_LogDir);
                    }
                }

                try
                {
                    CTrinityConfig.CLogInitializeLogger(m_LogDir);
                }
                catch { }

                return m_LogDir;
            }
            set
            {
                m_LogDir = value;

                if (m_LogDir == null || m_LogDir.Length == 0)
                {
                    m_LogDir = DefaultLogDirectory;
                }

                if (m_LogDir[m_LogDir.Length - 1] != Path.DirectorySeparatorChar)
                {
                    m_LogDir += Path.DirectorySeparatorChar;
                }

                if (!Directory.Exists(m_LogDir))
                {
                    try
                    {
                        Directory.CreateDirectory(m_LogDir);
                    }
                    catch (Exception)
                    {
                        ThrowCreatingLogDirectoryException(m_LogDir);
                    }
                }

                try
                {
                    CTrinityConfig.CLogInitializeLogger(m_LogDir);
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// Represents a value indicating whether the logged messages are echoed to the Console.
        /// </summary>
        [ConfigSetting(Optional: true)]
        public bool LogEchoOnConsole
        {
            get { return m_EchoOnConsole; }
            set { m_EchoOnConsole = value; CTrinityConfig.CLogSetEchoOnConsole(value); }
        }

        /// <summary>
        /// Represents a value indicating whether to stream log entries to a file on disk.
        /// </summary>
        [ConfigSetting(Optional:true)]
        public bool LogToFile
        {
            get { return m_LogToFile; }
            set { m_LogToFile = value;}
        }
    }
}
