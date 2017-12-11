using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Core.Lib;
using Trinity.Network;
using Trinity.Network.Messaging;
using Trinity.Storage;

namespace Trinity.Networking.WCFService.Client
{
    class WCFRemoteStorage : RemoteStorage
    {
        private WCFClientMemoryCloud m_memorycloud;
        private TrinityWCFAdapterClient m_wcfclient;

        protected internal WCFRemoteStorage(WCFClientMemoryCloud mc, int partitionId, TrinityWCFAdapterClient trinityWCFAdapterClient)
            : base(Enumerable.Empty<ServerInfo>(), connPerServer: 0, mc: null, partitionId: partitionId, nonblocking: true)
        {
            m_memorycloud = mc;
            m_wcfclient = trinityWCFAdapterClient;
        }

        public override unsafe TrinityErrorCode AddCell(long cellId, byte* cellBytes, int cellSize)
        {
            //XXX
            return base.AddCell(cellId, cellBytes, cellSize);
        }

        public override unsafe void SendMessage(byte* message, int size)
        {
            byte[] buf = new byte[size];
            Memory.Copy(message, 0, buf, 0, size);
            m_wcfclient.SendMessageWithoutResponse(buf);
        }

        public override unsafe void SendMessage(byte* message, int size, out TrinityResponse response)
        {
            byte[] buf = new byte[size];
            Memory.Copy(message, 0, buf, 0, size);
            var rsp = m_wcfclient.SendMessageWithResponse(buf);
            //TODO
            response = null;
            //response = new TrinityResponse()
        }
    }
}
