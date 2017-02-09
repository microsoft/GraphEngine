// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Trinity.Diagnostics;
using Trinity.Utilities;

namespace Trinity.Network
{
    /// <summary>
    /// Contains the information for a server or proxy.
    /// </summary>
    public class ServerInfo
    {
        /// <summary>
        /// The host name of the server or proxy.
        /// </summary>
        public string HostName;
        /// <summary>
        /// The network port for the server or proxy.
        /// </summary>
        public int Port;
        /// <summary>
        /// The assembly path of the server or proxy.
        /// </summary>
        public string AssemblyPath;

        /// <summary>
        /// The storage root directory of the server or proxy.
        /// </summary>
        public string StorageRoot;

        /// <summary>
        /// The logging level for the server or proxy.
        /// </summary>
        public LogLevel LoggingLevel;

        /// <summary>
        /// The id of the server group that the server belongs to.
        /// </summary>
        public string Id;

        private IPEndPoint ipe = null;

        ///// <summary>
        ///// Indicates whether an IP (e.g., 192.168.1.1) is specified for the host name.
        ///// </summary>
        //public bool IPSpecified = false;

        /// <summary>
        /// Initializes an empty ServerInfo instance.
        /// </summary>
        public ServerInfo()
        {

        }

        /// <summary>
        /// Returns the IPEndPoint of the current Trinity server.
        /// </summary>
        public IPEndPoint EndPoint
        {
            get
            {
                return ipe;
            }
        }

        /// <summary>
        /// Initializes an ServerInfo instance with the specified hostname, port, assembly path, and logging level.
        /// </summary>
        /// <param name="hostname">The host name of the server or proxy.</param>
        /// <param name="port">The network port for the server or proxy.</param>
        /// <param name="assemblyPath">The assembly path of the server or proxy.</param>
        /// <param name="logLevel">The logging level for the server or proxy.</param>
        public ServerInfo(string hostname, int port, string assemblyPath, LogLevel logLevel)
        {
            this.HostName     = hostname;
            this.Port         = port;
            this.AssemblyPath = assemblyPath;
            this.StorageRoot  = FileUtility.CompletePath(assemblyPath, false) + "storage\\";
            this.LoggingLevel = logLevel;
            this.ipe          = new IPEndPoint(NetworkUtility.Hostname2IPv4Address(HostName), Port);
        }

        /// <summary>
        /// Initializes an ServerInfo instance with the specified hostname, port, assembly path, and logging level.
        /// </summary>
        /// <param name="hostname">The host name of the server or proxy.</param>
        /// <param name="port">The network port for the server or proxy.</param>
        /// <param name="assemblyPath">The assembly path of the server or proxy.</param>
        /// <param name="storageRoot">The storage root directory of the server or proxy.</param>
        /// <param name="logLevel">The logging level for the server or proxy.</param>
        /// <param name="id">The id of the current server.</param>
        public ServerInfo(string hostname, int port, string assemblyPath, string storageRoot, LogLevel logLevel, string id)
        {
            this.HostName     = hostname;
            this.Port         = port;
            this.AssemblyPath = assemblyPath;
            this.StorageRoot  = storageRoot;
            this.LoggingLevel = logLevel;
            this.Id           = id;
            this.ipe          = new IPEndPoint(NetworkUtility.Hostname2IPv4Address(HostName), Port);
        }

        internal ServerInfo(ServerInfo si)
        {
            this.HostName     = si.HostName;
            this.Port         = si.Port;
            this.AssemblyPath = si.AssemblyPath;
            this.StorageRoot  = si.StorageRoot;
            this.LoggingLevel = si.LoggingLevel;
            this.Id           = si.Id;
            this.ipe          = new IPEndPoint(NetworkUtility.Hostname2IPv4Address(HostName), Port);
        }


        internal void CalculateIPEndpoint()
        {
            IPAddress ip = null;
            if (IPAddress.TryParse(HostName, out ip))
            {
                //IPSpecified = true;
                this.ipe = new IPEndPoint(ip, Port);
            }
            else
            {
                //IPSpecified = false;
                this.ipe = new IPEndPoint(NetworkUtility.Hostname2IPv4Address(HostName), Port);
            }
        }
        internal ServerInfo Clone()
        {
            return new ServerInfo(this);
        }
    }
}
