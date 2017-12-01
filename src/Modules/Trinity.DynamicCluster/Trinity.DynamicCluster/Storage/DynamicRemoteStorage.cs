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

namespace Trinity.DynamicCluster
{
    class DynamicRemoteStorage : RemoteStorage
    {

        public DynamicRemoteStorage(ReplicaInformation server_info, int connPerServer, MemoryCloud mc)
            : base(new[] { new ServerInfo(server_info.Address, server_info.Port, null, LogLevel.Info) }, connPerServer, mc, server_info.PartitionId, nonblocking: true)
        {
            Id = server_info.Id;
            NickName = Utils.GenerateNickName(Id);
        }

        public string NickName { get; }
        public Guid Id { get; }

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
