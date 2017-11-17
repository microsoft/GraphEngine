using System;
using System.Text;
using Trinity.Network;

namespace Trinity.DynamicCluster.Consensus
{
    public interface INameService
    {
        TrinityErrorCode Start();
        /// <summary>
        /// Publishes the information of the local instance.
        /// </summary>
        TrinityErrorCode PublishServerInfo(NameDescriptor name, ServerInfo serverInfo);
        /// <summary>
        /// Guarantees the delivery of informations regarding servers that
        /// the name service decides that this instance should connect to,
        /// regardless of whether these target instances have registered before
        /// or after <see cref="Start"/> is called.
        /// </summary>
        event EventHandler<Tuple<NameDescriptor, ServerInfo>> NewServerInfoPublished;
        TrinityErrorCode Stop();
    }
}
