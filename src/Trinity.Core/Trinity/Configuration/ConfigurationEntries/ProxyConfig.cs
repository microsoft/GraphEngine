using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Network;
namespace Trinity.Configuration
{
    internal sealed class ProxyConfig
    {
        static ProxyConfig proxyConfig = new ProxyConfig();
        private ProxyConfig() { Httpport = DefaultHttpPort; assemblyPath = DefaultAssemblyPath; }
        [ConfigInstance]
        internal static ProxyConfig Instance { get { return proxyConfig; } }
        [ConfigEntryName]
        internal static string ConfigEntry { get { return "Proxy"; } }

        private static string DefaultAssemblyPath { get { return Trinity.Utilities.AssemblyPath.MyAssemblyPath; } }
        private static int DefaultHttpPort { get { return 80; } }

        //internal static int DefaultServerPort = 5304;
        //internal static IPAddress ipAddress = Utilities.NetworkUtility.Hostname2IPv4Address("");

        private string Endpoint = "";
        private int Httpport;
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
        public string AssemblyPath
        {
            get { return assemblyPath; }
            set { assemblyPath = value; }
        }
    }
}
