using Trinity.Network;

namespace Trinity.ServiceFabric.Stateless
{
    public class StatelessServiceInfo : ServerInfo
    {
        public long InstanceId { get; private set; }

        public StatelessServiceInfo(long instanceId, string hostName, int port)
        {
            InstanceId = instanceId;
            HostName = hostName;
            Port = port;

            Id = instanceId.ToString();
        }
    }
}
