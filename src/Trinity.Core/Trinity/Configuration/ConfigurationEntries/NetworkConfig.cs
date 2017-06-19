using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Configuration
{
    /// <summary>
    /// Contains settings for the configuration section "Network".
    /// </summary>
    public sealed class NetworkConfig
    {
        #region Singleton
        static NetworkConfig s_NetworkConfig = new NetworkConfig();

        private NetworkConfig()
        {
            HttpPort = c_DefaultHttpPort;
            ClientMaxConn = c_DefaultClientMaxConn;
            ClientSendRetry = c_DefaultClientSendRetry;
            ClientReconnectRetry = c_DefaultClientReconnectRetry;
            Handshake = true;
        }

        [ConfigInstance]
        internal static NetworkConfig Instance { get { return s_NetworkConfig; } }
        [ConfigEntryName]
        internal static string ConfigEntry { get { return ConfigurationConstants.Tags.NETWORK; } }
        #endregion

        #region Fields
        private const int c_DefaultHttpPort = ConfigurationConstants.DefaultValue.DEFAULT_HTTP_PORT;
        private const int c_DefaultClientMaxConn = ConfigurationConstants.DefaultValue.DEFAULT_CLIENT_MAX_CONN;
        private const int c_DefaultClientSendRetry = ConfigurationConstants.DefaultValue.DEFAULT_CLIENT_SEND_RETRY;
        private const int c_DefaultClientReconnectRetry = ConfigurationConstants.DefaultValue.DEFAULT_CLIENT_RECONNECT_RETRY;
        #endregion

        #region Properties
        /// <summary>
        /// Represents the HttpPort value, if the server/proxy has Http endpoints, 
        /// it will listen on the specified port for Http requests after it is started.
        /// </summary>
        [ConfigSetting(Optional: true)]
        public int HttpPort { get; set; }

        /// <summary>
        /// Represents value to specify how many client connections are established 
        /// between a client and a communication instance.
        /// </summary>
        [ConfigSetting(Optional: true)]
        public int ClientMaxConn { get; set; }

        /// <summary>
        /// Represents value to specify how many retries will be attempted when a message cannot be sent.
        /// </summary>
        [ConfigSetting(Optional: true)]
        public int ClientSendRetry { get; set; }

        /// <summary>
        /// Represents value to specify how many reconnect attempts are made
        /// </summary>
        [ConfigSetting(Optional: true)]
        public int ClientReconnectRetry { get; set; }

        /// <summary>
        /// Represents value to specify whether a Trinity server/proxy/client preforms
        /// handshaking for a connection, if the value is true, it will preforms handshaking for a connection.
        /// </summary>
        [ConfigSetting(Optional: true)]
        public bool Handshake { get { return CTrinityConfig.CHandshake(); } set { CTrinityConfig.CSetHandshake(value); } }
        
     
        [ConfigSetting(Optional: true)]
        public bool ClientDisableSendBuffer { get { return CTrinityConfig.CClientDisableSendBuffer(); } set { CTrinityConfig.CSetClientDisableSendBuffer(value); } }
        #endregion
    }
}
