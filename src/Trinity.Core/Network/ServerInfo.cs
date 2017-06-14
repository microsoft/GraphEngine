// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Net;
using Trinity.Configuration;
using Trinity;
using Trinity.Utilities;
using Trinity.Diagnostics;
namespace Trinity.Network
{
    /// <summary>
    /// Represent the configuration information of a server or proxy.
    /// </summary>
    public class ServerInfo : ConfigurationSection
    {
        internal ServerInfo(XElement configSection)
            : base(configSection)
        {
            string endpoint = configSection.Attribute(ConfigurationConstants.Attrs.ENDPOINT).Value.Trim();
            string[] parts = endpoint.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            HostName = parts[0].Trim();
            Port     = int.Parse(parts[1].Trim());
            AssemblyPath = configSection.Attribute(ConfigurationConstants.Attrs.ASSEMBLY_PATH) == null ? null : configSection.Attribute(ConfigurationConstants.Attrs.ASSEMBLY_PATH).Value;
            Id = configSection.Attribute(ConfigurationConstants.Attrs.AVAILABILITY_GROUP) == null ? null : configSection.Attribute(ConfigurationConstants.Attrs.AVAILABILITY_GROUP).Value;
        }

        //  For legacy code compatibility
        private ServerInfo(string hostName, int port, string assemblyPath, string availabilityGroup, string storageRoot, string loggingLevel)
        {
            HostName = hostName;
            AssemblyPath = assemblyPath;
            Id = availabilityGroup;
            Port = port;

            this.Add(ConfigurationConstants.Tags.STORAGE,
                new ConfigurationEntry(ConfigurationConstants.Tags.STORAGE,
                new Dictionary<string, string> { { ConfigurationConstants.Attrs.STORAGE_ROOT, storageRoot } }));

            this.Add(ConfigurationConstants.Tags.LOGGING,
                new ConfigurationEntry(ConfigurationConstants.Tags.LOGGING,
                new Dictionary<string, string> { { ConfigurationConstants.Attrs.LOGGING_LEVEL, loggingLevel } }));
        }

        internal static ServerInfo _LegacyCreateServerInfo(string hostName, int port, string assemblyPath, string storageRoot, string loggingLevel, string availabilityGroup)
        {
            return new ServerInfo(hostName, port, assemblyPath, storageRoot, loggingLevel, availabilityGroup);
        }

        /// <summary>
        /// Constructs a ServerInfo instance.
        /// </summary>
        public ServerInfo(string hostName, int port, string assemblyPath, LogLevel logLevel)
        {
            HostName = hostName;
            AssemblyPath = assemblyPath;

            this.Add(ConfigurationConstants.Tags.LOGGING,
                new ConfigurationEntry(ConfigurationConstants.Tags.LOGGING,
                new Dictionary<string, string> { { ConfigurationConstants.Attrs.LOGGING_LEVEL, logLevel.ToString() } }));
        }
        /// <summary>
        /// Construct a ServerInfo instance.
        /// </summary>
        public ServerInfo()
        { }
        /// <summary>
        /// The host name of the server.
        /// </summary>
        public string HostName { get; private set; }
        /// <summary>
        /// The port of the server.
        /// </summary>
        public int Port { get; private set; }
        /// <summary>
        /// The assembly path of the server.
        /// </summary>
        public string AssemblyPath { get; private set; }
        /// <summary>
        /// The availability group id of the server.
        /// </summary>
        public string Id { get; private set; }
        /// <summary>
        /// The endpoint of the server.
        /// </summary>
        public IPEndPoint EndPoint
        {
            get
            {
                if (_ip_endpoint == null) _ip_endpoint = NetworkUtility.Hostname2IPEndPoint(HostName + ":" + Port.ToString());
                return _ip_endpoint;
            }
        }

        private IPEndPoint _ip_endpoint = null;
    }
}
