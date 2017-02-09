// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Trinity.Network;

namespace Trinity
{
    internal enum TrinityServerType : int
    {
        Server, Proxy
    }

    public static partial class TrinityConfig
    {
        static ClusterConfig current_cluster_config;

        internal static ClusterConfig CurrentClusterConfig
        {
            get
            {
                if (current_cluster_config == null)
                {
                    lock (config_load_lock)
                    {
                        if (current_cluster_config == null)
                        {
                            if (!File.Exists(DefaultConfigFile))
                                current_cluster_config = new ClusterConfig(DefaultConfigFile);
                            else
                                current_cluster_config = new ClusterConfig();
                        }
                    }
                }
                return current_cluster_config;
            }
        }

        /// <summary>
        /// Represents a list of Trinity servers.
        /// </summary>
        public static List<AvailabilityGroup> Servers
        {
            get
            {
                return CurrentClusterConfig.Servers;
            }
        }

        /// <summary>
        /// Represents a list of Trinity proxies.
        /// </summary>
        public static List<AvailabilityGroup> Proxies
        {
            get
            {
                return CurrentClusterConfig.Proxies;
            }
        }

        /// <summary>
        /// Gets all the server instances.
        /// </summary>
        public static List<ServerInfo> AllServerInstances
        {
            get
            {
                return CurrentClusterConfig.AllServerInstances;
            }
        }

        /// <summary>
        /// GEts all the proxy instances.
        /// </summary>
        public static List<ServerInfo> AllProxyInstances
        {
            get
            {
                return CurrentClusterConfig.AllProxyInstances;
            }
        }

        /// <summary>
        /// Gets a list of IPEndPoints corresponding to all the server instances.
        /// </summary>
        public static List<IPEndPoint> AllServerIPEndPoints
        {
            get
            {
                return CurrentClusterConfig.AllServerIPEndPoints;
            }
        }

        /// <summary>
        /// Gets a list of IPEndPoints corresponding to all the proxy instances.
        /// </summary>
        public static List<IPEndPoint> AllProxyIPEndPoints
        {
            get
            {
                return CurrentClusterConfig.AllProxyIPEndPoints;
            }
        }

        /// <summary>
        /// Default Value = 10 ms
        /// </summary>
        internal static int BackgroundSendingInterval = 10; //ms
        internal static int HeartbeatInterval = 1000; //ms
        internal static int MaxSocketReconnectNum = 8;
        internal static int client_max_conn = 2;

        /// <summary>
        /// Indicates the number of network connections a Trinity client can connect to a Trinity server at most.
        /// </summary>
        public static int ClientMaxConn
        {
            get
            {
                return client_max_conn;
            }
            set
            {
                client_max_conn = value;
            }
        }

        //Protocol Settings
        internal const int InvalidPort = -1;
        internal static int DefaultServerPort = 5304;
        internal static int DefaultProxyPort = 7304;
        internal static int s_HttpPort = 80;

        internal static int ListeningPort
        {
            get
            {
                return CurrentClusterConfig.ListeningPort;
            }
        }

        /// <summary>
        /// Adds a Trinity server.
        /// </summary>
        /// <param name="serverInfo">A <see cref="T:Trinity.Network.ServerInfo"/> instance.</param>
        public static void AddServer(ServerInfo serverInfo)
        {
            bool found = false;
            for (int i = 0; i < Servers.Count; i++)
            {
                if (Servers[i].Id == serverInfo.Id)
                {
                    Servers[i].ServerInstances.Add(serverInfo);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Servers.Add(new AvailabilityGroup(serverInfo.Id, serverInfo));
            }
        }

        /// <summary>
        /// Adds a Trinity proxy.
        /// </summary>
        /// <param name="proxy">A <see cref="Trinity.Network.ServerInfo"/> instance that represent a proxy.</param>
        public static void AddProxy(ServerInfo proxy)
        {
            bool found = false;
            for (int i = 0; i < Servers.Count; i++)
            {
                if (Proxies[i].Id == proxy.Id)
                {
                    Proxies[i].ServerInstances.Add(proxy);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Proxies.Add(new AvailabilityGroup(proxy.Id, proxy));
            }
        }

        /// <summary>
        /// The network port of the current Trinity server.
        /// </summary>
        public static int ServerPort
        {
            get
            {
                return CurrentClusterConfig.ServerPort;
            }

            set
            {
                DefaultServerPort = value;
            }
        }

        /// <summary>
        /// The HTTP port of the current Trinity server/proxy
        /// </summary>
        public static int HttpPort
        {
            //TODO not accessible in config file
            get
            {
                return s_HttpPort;
            }
            set
            {
                s_HttpPort = value;
            }
        }

        /// <summary>
        /// The network port of the current Trinity proxy.
        /// </summary>
        public static int ProxyPort
        {
            get
            {
                return CurrentClusterConfig.ProxyPort;
            }
            set
            {
                DefaultProxyPort = value;
            }
        }
    }
}
