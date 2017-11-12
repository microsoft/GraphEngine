using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.Network;
using Trinity.Network.Messaging;
using Trinity.Storage;

namespace Trinity.DynamicCluster
{
    class DynamicRemoteStorage : RemoteStorage
    {
        private NameDescriptor m_name;

        public DynamicRemoteStorage(ServerInfo server_info, NameDescriptor name, int connPerServer, MemoryCloud mc, int pid, bool nonblocking) : base(new[] { server_info }, connPerServer, mc, pid, nonblocking)
        {
            Log.WriteLine($"DynamicCluster: connecting to {name.Nickname} at {server_info.HostName}:{server_info.Port}");
            m_name = name;
        }

        public NameDescriptor Name => m_name;

        public bool Iscalled(NameDescriptor nd)
        {
            return ((m_name.Nickname.Equals(nd.Nickname)) && (m_name.ServerId == nd.ServerId));
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
