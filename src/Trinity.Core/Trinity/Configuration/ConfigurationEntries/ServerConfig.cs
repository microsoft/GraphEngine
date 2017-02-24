using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Trinity.Network;

namespace Trinity.Configuration
{
    internal sealed class ServerConfig
    {
        static ServerConfig serverConfig = new ServerConfig();
        private ServerConfig() { Httpport = DefaultHttpPort; assemblyPath = DefaultAssemblyPath; }
        [ConfigInstance]
        internal static ServerConfig Instance { get { return serverConfig; } }
        [ConfigEntryName]
        internal static string ConfigEntry { get { return "Server"; } }

        private static string DefaultAssemblyPath { get { return Trinity.Utilities.AssemblyPath.MyAssemblyPath; } }
        private static int DefaultHttpPort { get { return 80; } }

        //internal static int DefaultServerPort = 5304;
        //internal static IPAddress ipAddress = Utilities.NetworkUtility.Hostname2IPv4Address("");

        private string Endpoint = "";
        private int Httpport;
        private AvailabilityGroup availabilityGroup = new AvailabilityGroup();
        private string assemblyPath = "";

        [ConfigSetting(Optional: false)]
        public string EndPoint
        {
            get { return Endpoint; }
            set { Endpoint = value; }
        }
        [ConfigSetting(Optional: true)]
        public int HttpPort
        {
            get { return Httpport; }
            set { Httpport = value; }
        }
        [ConfigSetting(Optional: true)]
        public AvailabilityGroup AvailabilityGroup
        {
            get { return availabilityGroup; }
            set { availabilityGroup = value; }
        }
        [ConfigSetting(Optional: true)]
        public string AssemblyPath
        {
            get { return assemblyPath; }
            set { assemblyPath = value; }
        }
    }
}
