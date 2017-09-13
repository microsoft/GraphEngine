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
        public DynamicRemoteStorage(ServerInfo server_info, int connPerServer) : base(server_info, connPerServer)
        {
        }

        internal unsafe void SendMessage(byte* buffer, int size)
        {
            base.SendMessage(buffer, size);
        }

        internal unsafe void SendMessage(byte* buffer, int size, out TrinityResponse response)
        {
            base.SendMessage(buffer, size, out response);
        }
    }
}
