using System.Collections.Generic;
using Trinity.Network;

namespace Trinity
{
    public interface IClusterConfig
    {
        RunningMode RunningMode { get; set; }

        List<AvailabilityGroup> Servers { get; }

        List<AvailabilityGroup> Proxies { get; }

        int MyServerId { get; }

        int MyProxyId { get; }

        int MyInstanceId { get; }

        ServerInfo GetMyServerInfo();

        ServerInfo GetMyProxyInfo();

        int ServerPort { get; }

        int ProxyPort { get; }

        int ListeningPort { get; }

        string OutputCurrentConfig();
    }
}
