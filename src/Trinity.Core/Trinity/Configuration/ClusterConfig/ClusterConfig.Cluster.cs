// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Trinity.Network;
using Trinity.Utilities;

namespace Trinity
{
    public partial class ClusterConfig
    {
        /// <summary>
        /// Represents a list of Trinity servers.
        /// </summary>
        public List<AvailabilityGroup> Servers = new List<AvailabilityGroup>();

        /// <summary>
        /// Represents a list of Trinity proxies.
        /// </summary>
        public List<AvailabilityGroup> Proxies = new List<AvailabilityGroup>();

        /// <summary>
        /// Gets all the server instances.
        /// </summary>
        public List<ServerInfo> AllServerInstances
        {
            get
            {
                List<ServerInfo> list = new List<ServerInfo>();
                foreach (var server in Servers)
                {
                    foreach (var instance in server.ServerInstances)
                    {
                        list.Add(instance.Clone());
                    }
                }
                return list;
            }
        }

        /// <summary>
        /// GEts all the proxy instances.
        /// </summary>
        public List<ServerInfo> AllProxyInstances
        {
            get
            {
                List<ServerInfo> list = new List<ServerInfo>();
                foreach (var proxy in Proxies)
                {
                    foreach (var instance in proxy.ServerInstances)
                    {
                        list.Add(instance.Clone());
                    }
                }
                return list;
            }
        }

        /// <summary>
        /// Gets a list of IPEndPoints corresponding to all the server instances.
        /// </summary>
        public List<IPEndPoint> AllServerIPEndPoints
        {
            get
            {
                List<IPEndPoint> list = new List<IPEndPoint>();
                foreach (var server in Servers)
                {
                    foreach (var instance in server.ServerInstances)
                    {
                        list.Add(new IPEndPoint(instance.EndPoint.Address, instance.EndPoint.Port));
                    }
                }
                return list;
            }
        }

        /// <summary>
        /// Gets a list of IPEndPoints corresponding to all the proxy instances.
        /// </summary>
        public List<IPEndPoint> AllProxyIPEndPoints
        {
            get
            {
                List<IPEndPoint> list = new List<IPEndPoint>();
                foreach (var proxy in Proxies)
                {
                    foreach (var instance in proxy.ServerInstances)
                    {
                        list.Add(new IPEndPoint(instance.EndPoint.Address, instance.EndPoint.Port));
                    }
                }
                return list;
            }
        }

        internal IPAddress MyBoundIP
        {
            get
            {
                IPAddress my_ip_address;
                if (RunningMode == RunningMode.Server)
                {
                    var instance = GetMyServerInfo();
                    if (instance != null)
                    {
                        if (IPAddress.TryParse(instance.HostName, out my_ip_address))
                        {
                            return my_ip_address;
                        }
                    }
                }

                if (RunningMode == RunningMode.Proxy)
                {
                    var instance = GetMyProxyInfo();
                    if (instance != null)
                    {
                        if (IPAddress.TryParse(instance.HostName, out my_ip_address))
                        {
                            return my_ip_address;
                        }
                    }
                }
                return Global.MyIPAddress;
            }
        }

        public int ListeningPort
        {
            get
            {
                switch (RunningMode)
                {
                    case RunningMode.Server:
                        return ServerPort;
                    case RunningMode.Proxy:
                        return ProxyPort;
                    default:
                        return TrinityConfig.InvalidPort;
                }
            }
        }

        public int ServerPort
        {
            get
            {
                var instance = GetMyServerInfo();
                if (instance != null)
                {
                    Global.MyIPAddress = instance.EndPoint.Address;
                    return instance.Port;
                }
                return TrinityConfig.DefaultServerPort;
            }
        }

        internal ServerInfo GetMyServerInfo()
        {
            for (int i = 0; i < Servers.Count; i++)
            {
                foreach (var instance in Servers[i].ServerInstances)
                {
                    if (instance.AssemblyPath != null)
                    {
                        if (IPAddressComparer.CompareIPAddress(instance.EndPoint.Address, Global.MyIPAddress) == 0 &&
                    FileUtility.CompletePath(instance.AssemblyPath, false).ToLowerInvariant().Equals(Global.MyAssemblyPath.ToLowerInvariant()))
                        {
                            return instance;
                        }
                    }
                }
            }
            for (int i = 0; i < Servers.Count; i++)
            {
                foreach (var instance in Servers[i].ServerInstances)
                {
                    if (IPAddressComparer.CompareIPAddress(instance.EndPoint.Address, Global.MyIPAddress) == 0)
                        return instance;
                }
            }
            return null;
        }

        public int ProxyPort
        {
            get
            {
                var instance = GetMyProxyInfo();
                if (instance != null)
                {
                    Global.MyIPAddress = instance.EndPoint.Address;
                    return instance.Port;
                }
                return TrinityConfig.DefaultProxyPort;
            }
        }

        internal ServerInfo GetMyProxyInfo()
        {
            for (int i = 0; i < Proxies.Count; i++)
            {
                foreach (var instance in Proxies[i].ServerInstances)
                {
                    if (instance.AssemblyPath != null)
                    {
                        if (IPAddressComparer.CompareIPAddress(instance.EndPoint.Address, Global.MyIPAddress) == 0 &&
                            FileUtility.CompletePath(instance.AssemblyPath, false).ToLowerInvariant().Equals(Global.MyAssemblyPath.ToLowerInvariant()))
                        {
                            return instance;
                        }
                    }
                }

                foreach (var instance in Proxies[i].ServerInstances)
                {
                    if (IPAddressComparer.CompareIPAddress(instance.EndPoint.Address, Global.MyIPAddress) == 0)
                        return instance;
                }
            }
            return null;
        }

        private int my_server_id = -1;
        private int my_proxy_id = -1;

        public int MyInstanceId
        {
            get
            {
                if (RunningMode == RunningMode.Server)
                    return MyServerId;
                if (RunningMode == Trinity.RunningMode.Proxy)
                    return MyProxyId;
                return -1;
            }
        }

        /// <summary>
        /// Gets the ID of current server instance in the cluster.
        /// </summary>
        public int MyServerId
        {
            get
            {
                if (my_server_id != -1)
                    return my_server_id;

                if (RunningMode == RunningMode.Server)
                {
                    for (int i = 0; i < Servers.Count; i++)
                    {
                        if (Servers[i].Has(Global.MyIPAddresses, Global.MyIPEndPoint.Port) || Servers[i].HasLoopBackEndpoint(Global.MyIPEndPoint.Port))
                        {
                            my_server_id = i;
                            return i;
                        }
                    }
                }
                return -1;
            }
        }

        /// <summary>
        /// Gets the ID of current proxy instance in the cluster.
        /// </summary>
        public int MyProxyId
        {
            get
            {
                if (my_proxy_id != -1)
                    return my_proxy_id;

                if (RunningMode == RunningMode.Proxy)
                {
                    IPEndPoint myProxyIPE = new IPEndPoint(Global.MyIPAddress, ProxyPort);

                    for (int i = 0; i < Proxies.Count; i++)
                    {
                        if (Proxies[i].Has(Global.MyIPAddresses, ProxyPort) || Proxies[i].HasLoopBackEndpoint(myProxyIPE.Port))
                        {
                            my_proxy_id = i;
                            return i;
                        }
                    }
                }
                return -1;
            }
        }
    }
}
