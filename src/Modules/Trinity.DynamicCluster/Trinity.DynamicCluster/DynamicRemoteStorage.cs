using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Network;
using Trinity.Network.Messaging;
using Trinity.Storage;

namespace Trinity.DynamicCluster
{
    class DynamicRemoteStorage : RemoteStorage
    {
        private NameDescriptor m_name;

        public DynamicRemoteStorage(ServerInfo server_info, NameDescriptor name, int connPerServer) : base(server_info, connPerServer)
        {
            m_name = name;
        }

        public bool Iscalled(NameDescriptor nd)
        {
            return ((m_name.Nickname.Equals(nd.Nickname)) & (m_name.ServerId == nd.ServerId));
        }

        public bool Iscalled(String s)
        {
            return m_name.Nickname.Equals(s);
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
