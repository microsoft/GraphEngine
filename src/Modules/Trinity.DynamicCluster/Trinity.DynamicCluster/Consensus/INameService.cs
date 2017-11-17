using System;
using System.Text;
using Trinity.Network;

namespace Trinity.DynamicCluster.Consensus
{
    public interface INameService
    {
        TrinityErrorCode Start();
        TrinityErrorCode PublishServerInfo(NameDescriptor name, ServerInfo serverInfo);
        event EventHandler<Tuple<NameDescriptor, ServerInfo>> NewServerInfoPublished;
        TrinityErrorCode Stop();
    }
}
