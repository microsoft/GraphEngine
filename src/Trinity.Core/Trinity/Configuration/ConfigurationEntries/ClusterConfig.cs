// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Configuration
{
    internal sealed class ClusterConfig
    {
        static ClusterConfig clusterConfig = new ClusterConfig();
        private ClusterConfig() { }
        [ConfigInstance]
        internal static ClusterConfig Instance { get { return clusterConfig; } }
        [ConfigEntryName]
        internal static string ConfigEntry { get { return "Cluster"; } }

        [ConfigSetting(Optional: true)]
        public int Id { get; set; }
        [ConfigSetting(Optional: false)]
        public ServerConfig Server
        {
            get { return ServerConfig.Instance; }
        }
        [ConfigSetting(Optional: false)]
        public ProxyConfig Proxy
        {
            get { return ProxyConfig.Instance; }
        }
    }
}
