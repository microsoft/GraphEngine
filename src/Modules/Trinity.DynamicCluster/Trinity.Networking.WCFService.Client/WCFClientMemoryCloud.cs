using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Network.Messaging;
using Trinity.Storage;

namespace Trinity.Networking.WCFService.Client
{
    class WCFClientMemoryCloud : MemoryCloud
    {
        private Storage.Storage[] m_storagetable;

        public override int MyPartitionId => throw new NotImplementedException();

        public override int MyProxyId => throw new NotImplementedException();

        public override IEnumerable<Chunk> MyChunks => throw new NotImplementedException();

        public override int PartitionCount => throw new NotImplementedException();

        public override int ProxyCount => throw new NotImplementedException();

        public override IList<RemoteStorage> ProxyList => throw new NotImplementedException();

        protected override IList<Storage.Storage> StorageTable => throw new NotImplementedException();

        public override int MyInstanceId => throw new NotImplementedException();

        public override unsafe TrinityErrorCode AddCell(long cellId, byte* buff, int size)
        {
            throw new NotImplementedException();
        }

        public override TrinityErrorCode AddCell(long cellId, byte[] buff)
        {
            throw new NotImplementedException();
        }

        public override TrinityErrorCode AddCell(long cellId, byte[] buff, int offset, int size)
        {
            throw new NotImplementedException();
        }

        public override bool Contains(long cellId)
        {
            throw new NotImplementedException();
        }

        public override TrinityErrorCode GetCellType(long cellId, out ushort cellType)
        {
            throw new NotImplementedException();
        }

        public override bool IsLocalCell(long cellId)
        {
            throw new NotImplementedException();
        }

        public override TrinityErrorCode LoadCell(long cellId, out byte[] cellBuff)
        {
            throw new NotImplementedException();
        }

        public override TrinityErrorCode LoadCell(long cellId, out byte[] cellBuff, out ushort cellType)
        {
            throw new NotImplementedException();
        }

        public override bool LoadStorage()
        {
            throw new NotImplementedException();
        }

        public override TrinityErrorCode RemoveCell(long cellId)
        {
            throw new NotImplementedException();
        }

        public override bool ResetStorage()
        {
            throw new NotImplementedException();
        }

        public override TrinityErrorCode SaveCell(long cellId, byte[] buff)
        {
            throw new NotImplementedException();
        }

        public override TrinityErrorCode SaveCell(long cellId, byte[] buff, int offset, int size)
        {
            throw new NotImplementedException();
        }

        public override unsafe TrinityErrorCode SaveCell(long cellId, byte* buff, int size)
        {
            throw new NotImplementedException();
        }

        public override TrinityErrorCode SaveCell(long cellId, byte[] buff, int offset, int size, ushort cellType)
        {
            throw new NotImplementedException();
        }

        public override TrinityErrorCode SaveCell(long cellId, byte[] buff, ushort cellType)
        {
            throw new NotImplementedException();
        }

        public override bool SaveStorage()
        {
            throw new NotImplementedException();
        }

        public override unsafe void SendMessageToProxy(int proxyId, byte* buffer, int size)
        {
            throw new NotImplementedException();
        }

        public override unsafe void SendMessageToProxy(int proxyId, byte* buffer, int size, out TrinityResponse response)
        {
            throw new NotImplementedException();
        }

        public override unsafe TrinityErrorCode UpdateCell(long cellId, byte* buff, int size)
        {
            throw new NotImplementedException();
        }

        public override TrinityErrorCode UpdateCell(long cellId, byte[] buff)
        {
            throw new NotImplementedException();
        }

        public override TrinityErrorCode UpdateCell(long cellId, byte[] buff, int offset, int size)
        {
            throw new NotImplementedException();
        }

        public override bool Open(ClusterConfig config, bool nonblocking)
        {
            this.m_storagetable = new Storage.Storage[]
            {
                new WCFRemoteStorage(this, 0, new TrinityWCFAdapterClient(
                    new WcfCommunicationClientFactory(),
                    //TODO url pull
                    null, 
            // [uniform: ]
            // [MinValue, -1]
            // [0, MaxValue]
                    partitionKey: new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(0)
                    ))
            };

            return true;
        }

        public override unsafe void SendMessageToProxy(int proxyId, byte** buffers, int* sizes, int count)
        {
            throw new NotImplementedException();
        }

        public override unsafe void SendMessageToProxy(int proxyId, byte** buffers, int* sizes, int count, out TrinityResponse response)
        {
            throw new NotImplementedException();
        }

        public override long GetTotalMemoryUsage()
        {
            throw new NotImplementedException();
        }
    }
}
