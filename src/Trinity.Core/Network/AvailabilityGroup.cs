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
using Trinity.Configuration;
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
        /// A list of <see cref="Trinity.Network.ServerConfigurationSection"/>, each of which represents a Trinity server.
        /// </summary>
        public List<ServerInfo> Instances;

        /// <summary>
        /// Initializes a new instance of <see cref="Trinity.Network.AvailabilityGroup"/> class with one server.
        /// </summary>
        /// <param name="id">The id of the availability group.</param>
        /// <param name="serverInfo">A <see cref="Trinity.Network.ServerConfigurationSection"/> instance containing the information of the specified server.</param>
        public AvailabilityGroup(string id, ServerInfo serverInfo)
        {
            this.Id = id;
            Instances = new List<ServerInfo>();
            Instances.Add(serverInfo);
        }
        /// <summary>
        /// Initializes a new instance of <see cref="Trinity.Network.AvailabilityGroup"/> class with a set of servers.
        /// </summary>
        /// <param name="id">The id of the availability group.</param>
        /// <param name="replicas">A list of <see cref="Trinity.Network.ServerConfigurationSection"/> instances representing the replicas of the <see cref="Trinity.Network.AvailabilityGroup"/>.</param>
        public AvailabilityGroup(string id, IEnumerable<ServerInfo> replicas)
        {
            this.Id = id;
            Instances = new List<ServerInfo>(replicas);
        }

        /// <summary>
        /// Determines whether the current availability group contains the specified server replica.
        /// </summary>
        /// <param name="ipe">A <see cref="System.Net.IPEndPoint"/> of the specified server.</param>
        /// <returns>If the availability group contains the specified server instance, returns true; otherwise, returns false.</returns>
        public bool Has(IPEndPoint ipe)
        {
            IPEndPointComparer comparer = new IPEndPointComparer();
            for (int i = 0; i < Instances.Count; i++)
            {
                if (comparer.Compare(Instances[i].EndPoint, ipe) == 0)
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
            for (int i = 0; i < Instances.Count; i++)
            {
                foreach (var ip in ips)
                {
                    if (comparer.Compare(Instances[i].EndPoint, new IPEndPoint(ip, port)) == 0)
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
            foreach(var instance in Instances)
            {
                var ep = instance.EndPoint;
                if (IPAddress.IsLoopback(ep.Address) && ep.Port == port)
                    return true;
            }
            return false;
        }
    }
}
