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
    public class ServerInfo : ConfigurationSection
    {
        internal ServerInfo(XElement configSection)
            : base(configSection)
        {
            string endpoint = configSection.Attribute(ConfigurationConstants.Attrs.ENDPOINT).Value.Trim();
            string[] parts = endpoint.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            HostName = parts[0].Trim();
            EndPoint = NetworkUtility.Hostname2IPEndPoint(endpoint);
            AssemblyPath = configSection.Attribute(ConfigurationConstants.Attrs.ASSEMBLY_PATH) == null ? null : configSection.Attribute(ConfigurationConstants.Attrs.ASSEMBLY_PATH).Value;
            Id = configSection.Attribute(ConfigurationConstants.Attrs.AVAILABILITY_GROUP) == null ? null : configSection.Attribute(ConfigurationConstants.Attrs.AVAILABILITY_GROUP).Value;
        }

        //  For legacy code compatibility
        private ServerInfo(string hostName, string assemblyPath, string availabilityGroup, IPEndPoint endpoint, string storageRoot, string loggingLevel)
        {
            HostName = hostName;
            AssemblyPath = assemblyPath;
            Id = availabilityGroup;
            EndPoint = endpoint;

            this.Add(ConfigurationConstants.Tags.STORAGE,
                new ConfigurationEntry(ConfigurationConstants.Tags.STORAGE,
                new Dictionary<string, string> { { ConfigurationConstants.Attrs.STORAGE_ROOT, storageRoot } }));

            this.Add(ConfigurationConstants.Tags.LOGGING,
                new ConfigurationEntry(ConfigurationConstants.Tags.LOGGING,
                new Dictionary<string, string> { { ConfigurationConstants.Attrs.LOGGING_LEVEL, loggingLevel } }));
        }

        private ServerInfo(string hostName, int port, string assemblyPath, string storageRoot, string loggingLevel, string availabilityGroup)
            : this(hostName,  assemblyPath, availabilityGroup, NetworkUtility.Hostname2IPEndPoint(hostName + ":" + port), storageRoot, loggingLevel) { }

        internal static ServerInfo _LegacyCreateServerInfo(string hostName, int port, string assemblyPath, string storageRoot, string loggingLevel, string availabilityGroup)
        {
            return new ServerInfo(hostName, port, assemblyPath, storageRoot, loggingLevel, availabilityGroup);
        }

        internal static ServerInfo _LegacyCreateServerInfo(string hostName, string assemblyPath, string availabilityGroup, IPEndPoint endpoint, string storageRoot, string loggingLevel)
        {
            return new ServerInfo(hostName,  assemblyPath, availabilityGroup, endpoint, storageRoot, loggingLevel);
        }
        /// <summary>
        /// for legacy
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        /// <param name="assemblyPath"></param>
        /// <param name="logLevel"></param>
        public ServerInfo(string hostName, int port, string assemblyPath, LogLevel logLevel)
        {
            HostName = hostName;
            AssemblyPath = assemblyPath;

            this.Add(ConfigurationConstants.Tags.LOGGING,
                new ConfigurationEntry(ConfigurationConstants.Tags.LOGGING,
                new Dictionary<string, string> { { ConfigurationConstants.Attrs.LOGGING_LEVEL, logLevel.ToString() } }));
        }
        /// <summary>
        /// for legacy
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        /// <param name="assemblyPath"></param>
        /// <param name="logLevel"></param>
        public ServerInfo(string hostName, int port, LogLevel logLevel, string Id, string assemblyPath)
            : this(hostName, assemblyPath, Id, NetworkUtility.Hostname2IPEndPoint(hostName + ":" + port), "", logLevel.ToString()) { }
        /// <summary>
        /// for legacy
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        /// <param name="assemblyPath"></param>
        /// <param name="logLevel"></param>
        public ServerInfo()
        { }
        /// <summary>
        /// The host name of the server.
        /// </summary>
        public string HostName { get; private set; }
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
        public IPEndPoint EndPoint { get; private set; }
    }
}
