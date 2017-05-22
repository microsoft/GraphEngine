// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Xml.Linq;
using System.Linq;
using Trinity.Configuration;
using Trinity.Network;

namespace Trinity
{
    /// <summary>
    /// Represents all the types of trinity server
    /// </summary>
    internal enum TrinityServerType : int
    {
        Server, Proxy
    }

    public static partial class TrinityConfig
    {
        #region Fields
        private static ClusterConfig s_current_cluster_config = new ClusterConfig();
        private static Dictionary<string, ClusterConfig> s_clusterConfigurations = new Dictionary<string, ClusterConfig>();
        
        internal static int BackgroundSendingInterval = ConfigurationConstants.DefaultValue.DEFAULT_BACKGROUND_SENDING_INTERVAL;
        internal static int HeartbeatInterval = ConfigurationConstants.DefaultValue.DEFAULT_HEARTBEAT_INTERVAL;
        internal static int MaxSocketReconnectNum = ConfigurationConstants.DefaultValue.DEFAULT_MAXSOCKET_RECONNECTNUM;

        internal const int InvalidPort = ConfigurationConstants.DefaultValue.DEFAULT_INVALID_VALUE;
        internal static int DefaultServerPort = ConfigurationConstants.DefaultValue.DEFAULT_SERVER_PORT;
        internal static int DefaultProxyPort = ConfigurationConstants.DefaultValue.DEFAULT_PROXY_PORT;
        #endregion

        /// <summary>
        /// Represents the configuration settings for Global.CloudStorage.
        /// </summary>
        public static ClusterConfig CurrentClusterConfig
        {
            get
            {
                return s_current_cluster_config;
            }
        }

        /// <summary>
        /// Represents the configuration settings for clusters defined in a configuration file.
        /// </summary>
        public static IReadOnlyDictionary<string, ClusterConfig> ClusterConfigurations
        {
            get { return s_clusterConfigurations; }
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
        /// Gets all the proxy instances.
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
        /// Indicates the number of network connections a Trinity client can connect to a Trinity server at most.
        /// </summary>
        public static int ClientMaxConn
        {
            get
            {
                return NetworkConfig.Instance.ClientMaxConn;
            }
            set
            {
                NetworkConfig.Instance.ClientMaxConn = value;
            }
        }

        /// <summary>
        /// Gets the listening Port of the current cluster configuration.
        /// </summary>
        internal static int ListeningPort
        {
            get
            {
                return CurrentClusterConfig.ListeningPort;
            }
        }

        /// <summary>
        /// Adds a Trinity server to the current cluster configuration.
        /// </summary>
        /// <param name="serverInfo">A <see cref="T:Trinity.Network.ServerInfo"/> instance.</param>
        public static void AddServer(ServerInfo serverInfo)
        {
            bool found = false;
            for (int i = 0; i < Servers.Count; i++)
            {
                if (Servers[i].Id == serverInfo.Id)
                {
                    Servers[i].Instances.Add(serverInfo);
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
        /// Adds a Trinity proxy to the current cluster configuration.
        /// </summary>
        /// <param name="proxy">A <see cref="Trinity.Network.ServerInfo"/> instance that represent a proxy.</param>
        public static void AddProxy(ServerInfo proxy)
        {
            bool found = false;
            for (int i = 0; i < Servers.Count; i++)
            {
                if (Proxies[i].Id == proxy.Id)
                {
                    Proxies[i].Instances.Add(proxy);
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
        [Obsolete]
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
            get
            {
                return NetworkConfig.Instance.HttpPort;
            }
            set
            {
                NetworkConfig.Instance.HttpPort = value;
            }
        }

        /// <summary>
        /// The network port of the current Trinity proxy.
        /// </summary>
        [Obsolete]
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
