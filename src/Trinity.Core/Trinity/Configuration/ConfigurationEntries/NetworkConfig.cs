using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Configuration
{
    public sealed class NetworkConfig
    {
        #region Fields
        static NetworkConfig s_NetworkConfig = new NetworkConfig();

        private const int c_DefaultHttpPort             = 80;
        private const int c_DefaultClientMaxConn        = 2;
        private const int c_DefaultClientSendRetry      = 1;
        private const int c_DefaultClientReconnectRetry = 1;

        #endregion
        private NetworkConfig() 
        {
            HttpPort             = c_DefaultHttpPort;
            ClientMaxConn        = c_DefaultClientMaxConn;
            ClientSendRetry      = c_DefaultClientSendRetry;
            ClientReconnectRetry = c_DefaultClientReconnectRetry;
            Handshake            = true;
        }

        [ConfigInstance]
        internal static NetworkConfig Instance { get { return s_NetworkConfig; } }
        [ConfigEntryName]
        internal static string ConfigEntry { get { return ConfigurationConstants.Tags.NETWORK; } }

        [ConfigSetting(Optional: true)]
        public int HttpPort { get; set; }
        [ConfigSetting(Optional: true)]
        public int ClientMaxConn { get; set; }
        [ConfigSetting(Optional: true)]
        public int ClientSendRetry { get; set; }
        [ConfigSetting(Optional: true)]
        public int ClientReconnectRetry { get; set; }
        [ConfigSetting(Optional: true)]
        public bool Handshake { get { return CTrinityConfig.CHandshake(); } set { CTrinityConfig.CSetHandshake(value); } }
        [ConfigSetting(Optional: true)]
        public bool ClientDisableSendBuffer{ get { return CTrinityConfig.CClientDisableSendBuffer(); } set { CTrinityConfig.CSetClientDisableSendBuffer(value); } }
    }
}
