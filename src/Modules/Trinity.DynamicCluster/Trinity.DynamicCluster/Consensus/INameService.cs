using System;
using System.Net;
using System.Text;
using System.Threading;
using Trinity.Network;

namespace Trinity.DynamicCluster.Consensus
{
    /// <summary>
    /// When a name service is started (rather than instantiated), it should publish the information of the local instance.
    /// </summary>
    public interface INameService: IService
    {
        /// <summary>
        /// Guarantees the delivery of informations regarding servers that
        /// the name service decides that this instance should connect to,
        /// regardless of whether these target instances have registered before
        /// or after <see cref="Start"/> is called. Also, a name service
        /// should report if its own info is published.
        /// </summary>
        event EventHandler<Tuple<Guid, ServerInfo>> NewServerInfoPublished;
        /// <summary>
        /// Obtains a unique identifier for the local instance.
        /// Note, on the platforms where multiple instances run
        /// on the same server, generating Ids with MAC address
        /// or other machine fingerprints will fail.
        /// </summary>
        Guid InstanceId { get; }
        /// <summary>
        /// Obtains the FQDN or IP address of the local instance.
        /// </summary>
        string Address { get; }
        /// <summary>
        /// Obtains the Tcp port for Trinity protocols on the local instance.
        /// </summary>
        int Port { get; }
        /// <summary>
        /// Obtains the Http port on the local instance.
        /// </summary>
        int HttpPort { get; }
    }
}
