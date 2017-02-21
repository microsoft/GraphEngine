// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using Trinity.Network;
using Trinity.Diagnostics;
using Trinity.Core.Lib;
using System.IO;
using System.Runtime.InteropServices;

namespace Trinity
{
    /// <summary>
    /// Specifies the running modes supported by Trinity. This is obsolete.
    /// </summary>
    public enum RunningMode : int
    {
        /// <summary>
        /// Undefined running mode.
        /// </summary>
        Undefined,

        /// <summary>
        ///Embedded (in-process) mode.
        /// </summary>
        Embedded,

        /// <summary>
        /// As a server.
        /// </summary>
        Server,

        /// <summary>
        /// As a proxy.
        /// </summary>
        Proxy,

        /// <summary>
        /// As a client.
        /// </summary>
        Client,
    }

    /// <summary>
    /// A class for accessing and manipulating various system parameters.
    /// </summary>
    public unsafe static partial class TrinityConfig
    {
        internal static RunningMode current_running_mode = RunningMode.Undefined;
        internal static bool is_config_loaded = false;
        private static object config_load_lock = new object();
        internal const bool RWTimeout = false;
        private static string log_directory = "";

        static TrinityConfig()
        {
            TrinityC.Ping();
            InternalCalls.__init();

            LoggingLevel = c_DefaultLogLevel;
            StorageRoot  = DefaultStorageRoot;

            LoadTrinityConfig(false);
        }

        /// <summary>
        /// Gets or sets the running mode of current Trinity process.
        /// This property is obsolete and is kept for backward compatibility.
        /// Assigning a running mode to this property does not affect the system behavior.
        /// </summary>
        public static RunningMode CurrentRunningMode
        {
            get
            {
                return current_running_mode;
            }
            set
            {
                current_running_mode = value;
            }
        }

        private static string DefaultLogDirectory
        {
            get
            {
                return MyAssemblyPath + "trinity-log" + Path.DirectorySeparatorChar;
            }
        }

        /// <summary>
        /// Current system logging logLevel, default is LogLevel.Info.
        /// </summary>
        public static LogLevel LoggingLevel
        {
            get { return s_loglevel; }
            set { s_loglevel = value; CLogSetLogLevel(s_loglevel); }
        }

        /// <summary>
        /// Sets the directory for writing logs into.
        /// </summary>
        internal static string LogDirectory
        {
            get
            {
                if (log_directory == null || log_directory.Length == 0)
                    log_directory = DefaultLogDirectory;


                if (!Directory.Exists(log_directory))
                {
                    try
                    {
                        Directory.CreateDirectory(log_directory);
                    }
                    catch (Exception)
                    {
                        ThrowCreatingLogDirectoryException(log_directory);
                    }
                }

                if (log_directory[log_directory.Length - 1] != Path.DirectorySeparatorChar)
                {
                    log_directory = log_directory + Path.DirectorySeparatorChar;
                }

                try
                {
                    CLogInitializeLogger(log_directory);
                }
                catch (Exception) { }

                return log_directory;
            }
            set
            {
                log_directory = value;

                if (log_directory == null || log_directory.Length == 0)
                {
                    log_directory = DefaultLogDirectory;
                }

                if (log_directory[log_directory.Length - 1] != Path.DirectorySeparatorChar)
                {
                    log_directory += Path.DirectorySeparatorChar;
                }

                if (!Directory.Exists(log_directory))
                {
                    try
                    {
                        Directory.CreateDirectory(log_directory);
                    }
                    catch (Exception)
                    {
                        ThrowCreatingLogDirectoryException(log_directory);
                    }
                }

                try
                {
                    CLogInitializeLogger(log_directory);
                }
                catch (Exception) { }
            }
        }

        internal const  LogLevel c_DefaultLogLevel = LogLevel.Info;
        private  static LogLevel s_loglevel        = c_DefaultLogLevel;

        /// <summary>
        /// Provides a local testing configuration with one server (localhost).
        /// </summary>
        internal static bool LocalTest
        {
            set
            {
                if (value)
                {
                    Servers.Clear();
                    Proxies.Clear();

                    ServerInfo server = new ServerInfo("127.0.0.1",
                        5304,
                        MyAssemblyPath,
                        MyAssemblyPath + "storage\\",
                        LogLevel.Debug,
                        "0");

                    switch (CurrentClusterConfig.RunningMode)
                    {
                        case RunningMode.Server:
                            TrinityConfig.AddServer(server);
                            break;
                        case RunningMode.Proxy:
                            TrinityConfig.AddProxy(server);
                            break;
                    }
                }
            }
        }

        [DllImport(TrinityC.AssemblyName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        internal static extern unsafe void CLogInitializeLogger(string logDir);

        [DllImport(TrinityC.AssemblyName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        internal static extern void CLogSetLogLevel(LogLevel level);

        [DllImport(TrinityC.AssemblyName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        internal static extern void CLogSetEchoOnConsole(bool is_set);
    }
}
