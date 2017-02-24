using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Trinity.Configuration;
using Trinity.Diagnostics;
using Trinity.Network;
using Trinity.Utilities;
namespace Trinity
{
    public class ClusterConfig
    {
        #region Fields
        private RunningMode running_mode = RunningMode.Undefined;
        private string configFile;
        private int my_server_id = -1;
        private int my_proxy_id = -1;
        XMLConfig xml_config;
        #endregion

        #region Constructors
        // for compatibility only
        private ClusterConfig(string xmlConfig)
        {
            Servers = new List<AvailabilityGroup>();
            Proxies = new List<AvailabilityGroup>();
            configFile = xmlConfig;
            if (!File.Exists(xmlConfig))
                return;
            xml_config = new XMLConfig(configFile);
            LoadConfig();
        }
        internal static ClusterConfig _LegacyLoadClusterConfig(string xmlConfig)
        {
            return new ClusterConfig(xmlConfig);
        }

        internal ClusterConfig() : this(new XElement(ConfigurationConstants.Tags.CLUSTER)) { }

        internal ClusterConfig(XElement config)
        {
            var IdAttr = config.Attribute(ConfigurationConstants.Attrs.ID);
            Id = IdAttr != null ? IdAttr.Value : null;

            var serverEntries = config.Elements().Where(_ => _.Name == ConfigurationConstants.Tags.SERVER);
            var proxyEntries = config.Elements().Where(_ => _.Name == ConfigurationConstants.Tags.PROXY);

            int server_count = serverEntries.Count();
            int proxy_count = proxyEntries.Count();
            int server_with_agid_count = serverEntries.Where(_ => _.Attribute(ConfigurationConstants.Attrs.AVAILABILITY_GROUP) != null).Count();
            int proxy_with_agid_count = proxyEntries.Where(_ => _.Attribute(ConfigurationConstants.Attrs.AVAILABILITY_GROUP) != null).Count();

            if (server_count != server_with_agid_count && server_with_agid_count != 0)
            {
                throw new TrinityConfigException("Not all servers have " + ConfigurationConstants.Attrs.AVAILABILITY_GROUP + " attributes.");
            }

            if (proxy_count != proxy_with_agid_count && proxy_with_agid_count != 0)
            {
                throw new TrinityConfigException("Not all proxies have " + ConfigurationConstants.Attrs.AVAILABILITY_GROUP + " attributes.");
            }

            int server_ag_id = 0;
            int proxies_ag_id = 0;

            Servers = serverEntries
                .GroupBy(_ =>
                {
                    var attr = _.Attribute(ConfigurationConstants.Attrs.AVAILABILITY_GROUP);
                    if (attr != null) return attr.Value;
                    else return (server_ag_id++).ToString();
                })
                .Select(_ => new AvailabilityGroup(_.Key, _.Select(xelem => new ServerInfo(xelem))))
                .ToList();

            Proxies = proxyEntries
                .Where(_ => _.Name == ConfigurationConstants.Tags.PROXY)
                .GroupBy(_ =>
                {
                    var attr = _.Attribute(ConfigurationConstants.Attrs.AVAILABILITY_GROUP);
                    if (attr != null) return attr.Value;
                    else return (proxies_ag_id++).ToString();
                })
                .Select(_ => new AvailabilityGroup(_.Key, _.Select(xelem => new ServerInfo(xelem))))
                .ToList();
        }
        #endregion

        public string Id { get; private set; }
        public List<AvailabilityGroup> Servers { get; private set; }
        public List<AvailabilityGroup> Proxies { get; private set; }
        /// <summary>
        /// Get all Server instatnce
        /// </summary>
        public List<ServerInfo> AllServerInstances
        {
            get { return Servers.SelectMany(_ => _.Instances).ToList(); }
        }
        /// <summary>
        /// Get all Proxy instance
        /// </summary>
        public List<ServerInfo> AllProxyInstances
        {
            get { return Proxies.SelectMany(_ => _.Instances).ToList(); }
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
        /// <summary>
        /// Gets a list of IPEndPoints corresponding to all the server instances.
        /// </summary>
        public List<IPEndPoint> AllServerIPEndPoints
        {
            get
            {
                return Servers
                    .SelectMany(_ => _.Instances
                        .Select(instance =>
                            new IPEndPoint(
                                instance.EndPoint.Address,
                                instance.EndPoint.Port))).ToList();
            }
        }

        /// <summary>
        /// Gets a list of IPEndPoints corresponding to all the proxy instances.
        /// </summary>
        public List<IPEndPoint> AllProxyIPEndPoints
        {
            get
            {
                return Proxies
                    .SelectMany(_ => _.Instances
                        .Select(instance =>
                            new IPEndPoint(
                                instance.EndPoint.Address,
                                instance.EndPoint.Port))).ToList();
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
                    return instance.EndPoint.Port;
                }
                return TrinityConfig.DefaultServerPort;
            }
        }

        internal ServerInfo GetMyServerInfo()
        {
            for (int i = 0; i < Servers.Count; i++)
            {
                foreach (var instance in Servers[i].Instances)
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
                foreach (var instance in Servers[i].Instances)
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
                    return instance.EndPoint.Port;
                }
                return TrinityConfig.DefaultProxyPort;
            }
        }

        internal ServerInfo GetMyProxyInfo()
        {
            for (int i = 0; i < Proxies.Count; i++)
            {
                foreach (var instance in Proxies[i].Instances)
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

                foreach (var instance in Proxies[i].Instances)
                {
                    if (IPAddressComparer.CompareIPAddress(instance.EndPoint.Address, Global.MyIPAddress) == 0)
                        return instance;
                }
            }
            return null;
        }



        public int MyInstanceId
        {
            get
            {
                if (RunningMode == RunningMode.Server)
                    return MyServerId;
                if (RunningMode == RunningMode.Proxy)
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
                foreach (var instance in server.Instances)
                {
                    cw.WL("    {0}", instance.EndPoint);
                }
            }
            #endregion

            #region cw.WL("Protocol.Proxies: ");
            cw.WL("ProxyCount: {0}", Proxies.Count);
            foreach (var proxy in Proxies)
            {
                foreach (var instance in proxy.Instances)
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
                throw;
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
                var str_ip_endpoint = server_entry_list[i].Value;
                string[] parts = str_ip_endpoint.Split(new char[] { ':' });
                IPEndPoint ep = Utilities.NetworkUtility.Hostname2IPEndPoint(str_ip_endpoint.Trim());

                if (ep != null)
                {

                    Dictionary<string, string> pvs = server_entry_list[i].Attributes().ToDictionary(attr => attr.Name.ToString(), attr => attr.Value);

                    string assemblyPath = null;
                    string Id = null;
                    LogLevel loggingLevel = LoggingConfig.c_DefaultLogLevel;
                    string storageRoot = null;

                    if (pvs.ContainsKey(ConfigurationConstants.Attrs.ASSEMBLY_PATH))
                        assemblyPath = FileUtility.CompletePath(pvs[ConfigurationConstants.Attrs.ASSEMBLY_PATH], false);

                    if (pvs.TryGetValue(ConfigurationConstants.Attrs.STORAGE_ROOT, out storageRoot))
                        storageRoot = storageRoot.Trim();

                    if (pvs.ContainsKey(ConfigurationConstants.Attrs.LOGGING_LEVEL))
                        loggingLevel = (LogLevel)Enum.Parse(typeof(LogLevel), pvs[ConfigurationConstants.Attrs.LOGGING_LEVEL], true);

                    if (pvs.TryGetValue(xml_agroup_id_attribute_name, out Id))
                        Id = Id.Trim();
                    else
                        Id = i.ToString(CultureInfo.InvariantCulture);

                    ServerInfo si = ServerInfo._LegacyCreateServerInfo(
                        hostName: parts[0].Trim(),
                        //httpPort: TrinityConfig.HttpPort,
                        assemblyPath: assemblyPath,
                        availabilityGroup: Id,
                        storageRoot: storageRoot,
                        endpoint: ep,
                        loggingLevel: loggingLevel.ToString());


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
            return id_infolist_dict.Select(_ => new AvailabilityGroup(_.Key, _.Value.Select(si => (ServerInfo)si)));
        }
    }

}
