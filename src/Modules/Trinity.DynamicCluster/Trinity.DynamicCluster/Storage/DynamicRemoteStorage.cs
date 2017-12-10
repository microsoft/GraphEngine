using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.DynamicCluster.Consensus;
using Trinity.Network;
using Trinity.Network.Messaging;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Storage
{
    class DynamicRemoteStorage : RemoteStorage
    {

        public DynamicRemoteStorage(ReplicaInformation server_info, int connPerServer, MemoryCloud mc)
            : base(new[] { new ServerInfo(server_info.Address, server_info.Port, null, LogLevel.Info) }, connPerServer, mc, server_info.PartitionId, nonblocking: true)
        {
            ReplicaInformation = server_info;
            NickName = Utils.GenerateNickName(ReplicaInformation.Id);
        }

        public string NickName { get; }
        public ReplicaInformation ReplicaInformation { get; }

        public override unsafe void SendMessage(byte* buffer, int size)
        {
            base.SendMessage(buffer, size);
        }

        public override unsafe void SendMessage(byte* buffer, int size, out TrinityResponse response)
        {
            base.SendMessage(buffer, size, out response);
        }
    }
}
