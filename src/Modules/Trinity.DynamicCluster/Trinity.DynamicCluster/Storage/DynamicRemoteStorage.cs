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

        public DynamicRemoteStorage(ServerInfo server_info, int connPerServer, MemoryCloud mc, int pid, bool nonblocking) 
            : base(new[] { server_info }, connPerServer, mc, pid, nonblocking) { }

        public string NickName { get; private set; }
        public Guid Id { get; private set; }

        internal void SetId(Guid id)
        {
            Id = id;
            NickName = Utils.GenerateNickName(id);
        }

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
