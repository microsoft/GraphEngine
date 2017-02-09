// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.Network;
using Trinity.Utilities;

namespace Trinity
{
    /// <summary>
    /// A class for configuring a Trinity cluster.
    /// </summary>
    public partial class ClusterConfig
    {
        XMLConfig xml_config;
        private string configFile;
        private RunningMode running_mode = RunningMode.Undefined;

        public ClusterConfig()
        {
        }

        public ClusterConfig(string xmlConfig)
        {
            configFile = xmlConfig;
            if (!File.Exists(xmlConfig))
                return;
            xml_config = new XMLConfig(configFile);
            LoadConfig();
        }

        internal string OutputCurrentConfig()
        {
            CodeWriter cw = new CodeWriter();
            cw.WL();
            cw.WL("*****************************************************");
            cw.WL();

            #region cw.WL("Protocol.Servers: ");
            cw.WL("ServerCount: {0}", Servers.Count);

            foreach (var server in Servers)
            {
                foreach (var instance in server.ServerInstances)
                {
                    cw.WL("    {0}", instance.EndPoint);
                }
            }
            #endregion

            #region cw.WL("Protocol.Proxies: ");
            cw.WL("ProxyCount: {0}", Proxies.Count);
            foreach (var proxy in Proxies)
            {
                foreach (var instance in proxy.ServerInstances)
                {
                    cw.WL("    {0}", instance.EndPoint);
                }
            }
            #endregion

            cw.WL();
            cw.WL("*****************************************************");
            cw.WL();
            return cw.ToString();
        }

        /// <summary>
        /// Gets or sets the running mode of current Trinity process.
        /// </summary>
        public RunningMode RunningMode
        {
            get
            {
                return running_mode;
            }
            set
            {
                running_mode = value;
            }
        }

        public void SaveConfig()
        {
            if (File.Exists(configFile))
            {
                try
                {
                    File.Delete(configFile);
                }
                catch (Exception) { }
            }

            FileUtility.CompletePath(Path.GetDirectoryName(configFile), true);

            XMLConfig xml_config = new XMLConfig(configFile);

            #region xml_config.SetEntryValues( "Servers", "Server", ipStringList );
            xml_config.ClearEntryValues("Servers", "Server");

            for (int i = 0; i < Servers.Count; i++)
            {
                var server = Servers[i];

                foreach (var instance in server.ServerInstances)
                {
                    Dictionary<string, object> pvs = new Dictionary<string, object>();

                    pvs.Add("ServerId", server.Id);
                    pvs.Add(c_AssemblyPathAttributeKey, instance.AssemblyPath);
                    pvs.Add(c_LoggingLevelAttributeKey, instance.LoggingLevel);

                    xml_config.SetEntryProperties("Servers", "Server", string.Format(CultureInfo.InvariantCulture, "{0}:{1}", instance.HostName, instance.Port), pvs);
                }
            }
            #endregion

            #region xml_config.SetEntryValues( "Proxies", "Proxy", ipStringList );
            xml_config.ClearEntryValues("Proxies", "Proxy");

            for (int i = 0; i < Proxies.Count; i++)
            {
                var proxy = Proxies[i];
                foreach (var instance in proxy.ServerInstances)
                {
                    Dictionary<string, object> pvs = new Dictionary<string, object>();

                    pvs.Add("ProxyId", proxy.Id);
                    pvs.Add("WorkingDirectory", instance.AssemblyPath);
                    pvs.Add("LoggingLevel", instance.LoggingLevel);

                    xml_config.SetEntryProperties("Proxies", "Proxy", string.Format(CultureInfo.InvariantCulture, "{0}:{1}", instance.HostName, instance.Port), pvs);
                }

            }
            #endregion

            xml_config.Save();
        }

        const string c_AssemblyPathAttributeKey = "AssemblyPath";
        const string c_StorageRootAttributeKey  = "StorageRoot";
        const string c_LoggingLevelAttributeKey = "LoggingLevel";
        private void LoadConfig()
        {
            try
            {
                Servers.Clear();
                Servers.AddRange(GetAvailabilityGroupList(xml_config, "Servers", "Server", "ServerId"));

                Proxies.Clear();
                Proxies.AddRange(GetAvailabilityGroupList(xml_config, "Proxies", "Proxy", "ProxyId"));
            }
            catch (Exception e)
            {
                Log.WriteLine(LogLevel.Error, "There are errors in your configuration file.");
                Log.WriteLine(e.Message);
                Global.Exit();
            }
        }

        /// <summary>
        /// Obtain a list of AvailabilityGroup from XML config.
        /// </summary>
        private IEnumerable<AvailabilityGroup> GetAvailabilityGroupList(XMLConfig config, string xml_section_name, string xml_server_info_entry_name, string xml_agroup_id_attribute_name)
        {
            var server_entry_list = config.GetEntries(xml_section_name, xml_server_info_entry_name);
            Dictionary<string, List<ServerInfo>> id_infolist_dict = new Dictionary<string, List<ServerInfo>>();
            for (int i = 0; i < server_entry_list.Count; i++)
            {
                var        str_ip_endpoint = server_entry_list[i].Value;
                string[]   parts           = str_ip_endpoint.Split(new char[] { ':' });
                IPEndPoint ep              = Utilities.NetworkUtility.Hostname2IPEndPoint(str_ip_endpoint.Trim());

                if (ep != null)
                {
                    ServerInfo si = new ServerInfo();
                    si.HostName   = parts[0].Trim();
                    si.Port       = ep.Port;

                    Dictionary<string, string> pvs = server_entry_list[i].Attributes().ToDictionary(attr => attr.Name.ToString(), attr => attr.Value);

                    if (pvs.ContainsKey(c_AssemblyPathAttributeKey))
                        si.AssemblyPath = FileUtility.CompletePath(pvs[c_AssemblyPathAttributeKey], false);
                    else
                        si.AssemblyPath = null;

                    string storageRoot = null;
                    if (pvs.TryGetValue(c_StorageRootAttributeKey, out storageRoot))
                        si.StorageRoot = storageRoot.Trim();
                    else
                        si.StorageRoot = null;

                    if (pvs.ContainsKey(c_LoggingLevelAttributeKey))
                        si.LoggingLevel = (LogLevel)Enum.Parse(typeof(LogLevel), pvs[c_LoggingLevelAttributeKey], true);
                    else
                        si.LoggingLevel = TrinityConfig.c_DefaultLogLevel;

                    string serverId = null;
                    if (pvs.TryGetValue(xml_agroup_id_attribute_name, out serverId))
                        si.Id = serverId.Trim();
                    else
                        si.Id = i.ToString(CultureInfo.InvariantCulture);

                    si.CalculateIPEndpoint();

                    List<ServerInfo> list = null;
                    if (!id_infolist_dict.TryGetValue(si.Id, out list))
                    {
                        list = new List<ServerInfo>();
                        id_infolist_dict.Add(si.Id, list);
                    }
                    list.Add(si);
                }
                else
                {
                    Log.WriteLine(LogLevel.Info, "Cannot resolve {0}, ignoring.", str_ip_endpoint);
                }
            }
            return id_infolist_dict.Select(_ => new AvailabilityGroup(_.Key, _.Value));
        }
    }
}
