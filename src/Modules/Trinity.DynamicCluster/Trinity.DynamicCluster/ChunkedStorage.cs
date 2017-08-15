using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Network.Messaging;
using Trinity.Storage;

namespace Trinity.DynamicCluster
{
    unsafe class ChunkedStorage : Storage.Storage
    {
        // List<Chunk> chunks;
        // need a protocol to communicate within an AG: QueryYourChunkIds
        // we also need a protocol to "discuss" about chunk allocation. (P2)
        // route messages to a specific storage
        List<Storage.Storage> Storages; // <----
        MemoryCloud m_MemoryCloud; // get ChunkIds from m_MemoryCloud and decide routing.

        bool ContainsLocalStorage { get { throw new NotFiniteNumberException(); } }

        public override TrinityErrorCode AddCell(long cellId, byte[] buff)
        {
            throw new NotImplementedException();
        }

        public override TrinityErrorCode AddCell(long cellId, byte* buff, int size)
        {
            throw new NotImplementedException();
        }

        public override TrinityErrorCode AddCell(long cellId, byte[] buff, int offset, int size)
        {
            throw new NotImplementedException();
        }

        public override TrinityErrorCode AddCell(long cellId, byte* buff, int size, ushort cellType)
        {
            throw new NotImplementedException();
        }

        public override TrinityErrorCode AddCell(long cellId, byte[] buff, int offset, int size, ushort cellType)
        {
            throw new NotImplementedException();
        }

        public override bool Contains(long cellId)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override TrinityErrorCode GetCellType(long cellId, out ushort cellType)
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

        public override TrinityErrorCode RemoveCell(long cellId)
        {
            throw new NotImplementedException();
        }

        public override TrinityErrorCode SaveCell(long cell_id, byte[] buff)
        {
            throw new NotImplementedException();
        }

        public override TrinityErrorCode SaveCell(long cellId, byte* buff, int size)
        {
            throw new NotImplementedException();
        }

        public override TrinityErrorCode SaveCell(long cellId, byte* buff, int size, ushort cellType)
        {
            throw new NotImplementedException();
        }

        public override TrinityErrorCode SaveCell(long cell_id, byte[] buff, int offset, int size)
        {
            throw new NotImplementedException();
        }

        public override TrinityErrorCode SaveCell(long cellId, byte[] buff, int offset, int size, ushort cellType)
        {
            throw new NotImplementedException();
        }

        public override TrinityErrorCode UpdateCell(long cellId, byte[] buff)
        {
            throw new NotImplementedException();
        }

        public override TrinityErrorCode UpdateCell(long cellId, byte* buff, int size)
        {
            throw new NotImplementedException();
        }

        public override TrinityErrorCode UpdateCell(long cellId, byte[] buff, int offset, int size)
        {
            throw new NotImplementedException();
        }

        protected override void SendMessage(TrinityMessage message)
        {
            throw new NotImplementedException();
        }

        protected override void SendMessage(byte* message, int size)
        {
            throw new NotImplementedException();
        }

        protected override void SendMessage(TrinityMessage message, out TrinityResponse response)
        {
            throw new NotImplementedException();
        }

        protected override void SendMessage(byte* message, int size, out TrinityResponse response)
        {
            throw new NotImplementedException();
        }
    }
}
