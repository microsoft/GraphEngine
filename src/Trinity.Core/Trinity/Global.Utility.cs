// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using Trinity.Network;
using System.Reflection;
using Trinity;
using Trinity.Core.Lib;
using Trinity.Utilities;
using Trinity.Diagnostics;

namespace Trinity
{
    public static partial class Global
    {
        private static IPAddress my_ip_address = null;
        private static List<IPAddress> my_ip_addresses = null;

        private static IPEndPoint my_ip_endpoint = null;

        private static long my_aggregation_ipe_value = -1;

        private static string my_assembly_path = "";

        static string tmp_dir = null;
        static string app_dir = null;

        /// <summary>
        /// Safely exit current Trinity instance. Logs will be flushed before exiting.
        /// </summary>
        public static void Exit()
        {
            Global.Exit(0);
        }

        /// <summary>
        /// Safely exit current Trinity instance. Logs will be flushed before exiting.
        /// </summary>
        /// <param name="exitCode">Exit code to be given to the operating system.</param>
        public static void Exit(int exitCode)
        {
            Win32.NativeAPI.timeEndPeriod(1);
            Diagnostics.Log.Flush();
            Environment.Exit(exitCode);
        }

        internal static string TempDirectory
        {
            get
            {
                if (tmp_dir == null)
                {
                    tmp_dir = FileUtility.CompletePath(FileUtility.CompletePath(Environment.GetEnvironmentVariable("TEMP")) + @"Trinity\", true);
                }
                return tmp_dir;
            }
        }

        internal static string AppDataDirectory
        {
            get
            {
                if (app_dir == null)
                {
                    app_dir = FileUtility.CompletePath(FileUtility.CompletePath(Environment.GetEnvironmentVariable("AppData")) + @"Trinity\", true);
                }
                return app_dir;
            }
        }

        /// <summary>
        /// Gets the path or UNC location of the running Trinity instance.
        /// </summary>
        public static string MyAssemblyPath
        {
            get
            {
                if (my_assembly_path.Length == 0)
                {
                    my_assembly_path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar;
                }
                return my_assembly_path;
            }
        }

        /// <summary>
        /// Gets the IPAddress bound to current Trinity server.
        /// </summary>
        public static IPAddress MyIPAddress
        {
            get
            {
                if (my_ip_address == null)
                {
                    IPAddress[] addressList = Dns.GetHostEntry(Environment.MachineName).AddressList;
                    foreach (IPAddress ip in addressList)
                    {
                        if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            my_ip_address = ip;
                            break;
                        }
                    }
                }
                return my_ip_address;
            }
            internal set
            {
                my_ip_address = value;
                my_ip_endpoint = null;
            }
        }

        /// <summary>
        /// Gets all the IPAddresses bound to current Trinity server.
        /// </summary>
        public static List<IPAddress> MyIPAddresses
        {
            get
            {
                if (my_ip_addresses == null)
                {
                    my_ip_addresses = new List<IPAddress>();
                    IPAddress[] addressList = Dns.GetHostEntry(Environment.MachineName).AddressList;
                    foreach (IPAddress ip in addressList)
                    {
                        if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            my_ip_addresses.Add(ip);
                        }
                    }
                }
                return my_ip_addresses;
            }
        }

        /// <summary>
        /// Gets the IPEndPoint bound to current Trinity instance.
        /// </summary>
        public static IPEndPoint MyIPEndPoint
        {
            get
            {
                if (my_ip_endpoint == null)
                {
                    my_ip_endpoint = new IPEndPoint(MyIPAddress, TrinityConfig.ListeningPort);
                }

                return my_ip_endpoint;
            }
        }

        /// <summary>
        /// Gets the total memory usage of all Trinity servers.
        /// </summary>
        /// <returns>Working set size in bytes.</returns>
        public static long GetTotalMemoryUsage()
        {
            return CloudStorage.GetTotalMemoryUsage();
        }

        /// <summary>
        /// Gets the IPEndPoint bound to current Trinity proxy.
        /// </summary>
        public unsafe static long MyProxyIPEndPoint
        {
            get
            {
                if (my_aggregation_ipe_value == -1)
                {
                    byte[] bytes = new byte[8];

                    fixed (byte* byte_p = bytes)
                    {
                        byte* p = byte_p;

                        *(int*)p = BitHelper.ToInt32(MyIPAddress.GetAddressBytes(), 0);
                        p += 4;

                        *(int*)p = TrinityConfig.ProxyPort;

                        my_aggregation_ipe_value = *(long*)byte_p;
                    }
                }
                return my_aggregation_ipe_value;
            }
        }

        internal unsafe static DateTime GetBuildTime()
        {
            byte[] PEBuffer = new byte[2048];
            using (FileStream fs = new FileStream(Assembly.GetCallingAssembly().Location, FileMode.Open, FileAccess.Read))
            {
                fs.Read(PEBuffer, 0, 2048);
            }
            fixed (byte* p = PEBuffer)
            {
                DateTime t = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(*(int*)(p + *(int*)(p + 60) + 8));
                return t.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(t).Hours);
            }
        }
    }
}
