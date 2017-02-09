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
using Trinity.Utilities;

namespace Trinity.Network
{
    /// <summary>
    /// Represents an availability group which consists of a set of servers that are backups of each other.
    /// </summary>
    public struct AvailabilityGroup
    {
        /// <summary>
        /// The identifier of an availability group.
        /// </summary>
        public string Id;
        /// <summary>
        /// A list of <see cref="Trinity.Network.ServerInfo"/>, each of which represents a Trinity server.
        /// </summary>
        public List<ServerInfo> ServerInstances;

        /// <summary>
        /// Initializes a new instance of <see cref="Trinity.Network.AvailabilityGroup"/> class with one server.
        /// </summary>
        /// <param name="id">The id of the availability group.</param>
        /// <param name="serverInfo">A <see cref="Trinity.Network.ServerInfo"/> instance containing the information of the specified server.</param>
        public AvailabilityGroup(string id, ServerInfo serverInfo)
        {
            this.Id = id;
            ServerInstances = new List<ServerInfo>(3);
            ServerInstances.Add(serverInfo);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Trinity.Network.AvailabilityGroup"/> class with a set of servers.
        /// </summary>
        /// <param name="id">The id of the availability group.</param>
        /// <param name="replicas">A list of <see cref="Trinity.Network.ServerInfo"/> instances representing the replicas of the <see cref="Trinity.Network.AvailabilityGroup"/>.</param>
        public AvailabilityGroup(string id, List<ServerInfo> replicas)
        {
            this.Id = id;
            ServerInstances = new List<ServerInfo>(replicas.Count);
            for (int i = 0; i < replicas.Count; i++)
            {
                ServerInstances.Add(new ServerInfo(replicas[i]));
            }
        }

        /// <summary>
        /// Gets the storage root directory by the specified server <see cref="System.Net.IPEndPoint"/>.
        /// </summary>
        /// <param name="ipe">A <see cref="System.Net.IPEndPoint"/> of the specified server.</param>
        /// <returns>The storage root directory of the specified server.</returns>
        public string GetStorageRootByIPE(IPEndPoint ipe)
        {
            IPEndPointComparer comparer = new IPEndPointComparer();
            for (int i = 0; i < ServerInstances.Count; i++)
            {
                if (comparer.Compare(ServerInstances[i].EndPoint, ipe) == 0)
                    return ServerInstances[i].StorageRoot;
            }
            return null;
        }

        /// <summary>
        /// Determines whether the current availability group contains the specified server replica.
        /// </summary>
        /// <param name="ipe">A <see cref="System.Net.IPEndPoint"/> of the specified server.</param>
        /// <returns>If the availability group contains the specified server instance, returns true; otherwise, returns false.</returns>
        public bool Has(IPEndPoint ipe)
        {
            IPEndPointComparer comparer = new IPEndPointComparer();
            for (int i = 0; i < ServerInstances.Count; i++)
            {
                if (comparer.Compare(ServerInstances[i].EndPoint, ipe) == 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the current availability group contains the specified server replica.
        /// </summary>
        /// <param name="ips">A list of available <see cref="System.Net.IPAddress"/> instances for a given server.</param>
        /// <param name="port">The network port of the server.</param>
        /// <returns>If the availability group contains the specified server instance, returns true; otherwise, returns false.</returns>
        public bool Has(List<IPAddress> ips, int port)
        {
            IPEndPointComparer comparer = new IPEndPointComparer();
            for (int i = 0; i < ServerInstances.Count; i++)
            {
                foreach (var ip in ips)
                {
                    if (comparer.Compare(ServerInstances[i].EndPoint, new IPEndPoint(ip, port)) == 0)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the current availability group contains a server that binds to a loopback endpoint.
        /// </summary>
        /// <param name="port">The network port of the server.</param>
        /// <returns>If the availability group contains the specified server instance, returns true; otherwise, returns false.</returns>
        public bool HasLoopBackEndpoint(int port)
        {
            foreach(var instance in ServerInstances)
            {
                var ep = instance.EndPoint;
                if (IPAddress.IsLoopback(ep.Address) && ep.Port == port)
                    return true;
            }
            return false;
        }
    }
}
