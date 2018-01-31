using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Network;
using Trinity.Network.Messaging;
using Trinity.Storage;

namespace Trinity.Client
{
    internal class RedirectedIStorage : IStorage
    {
        private IMessagePassingEndpoint m_ep;
        private CommunicationInstance m_comminst;
        private int partitionId;

        public RedirectedIStorage(IMessagePassingEndpoint ep, TrinityClient tc, int p)
        {
            m_ep = ep;
            m_comminst = tc;
            partitionId = p;
        }

        public unsafe TrinityErrorCode AddCell(long cellId, byte* buff, int size, ushort cellType)
        {
            throw new NotImplementedException();
        }

        public bool Contains(long cellId)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public TrinityErrorCode GetCellType(long cellId, out ushort cellType)
        {
            throw new NotImplementedException();
        }

        public T GetCommunicationModule<T>() where T : CommunicationModule
            => m_comminst.GetCommunicationModule<T>();

        public TrinityErrorCode LoadCell(long cellId, out byte[] cellBuff, out ushort cellType)
        {
            throw new NotImplementedException();
        }

        public TrinityErrorCode RemoveCell(long cellId)
        {
            throw new NotImplementedException();
        }

        public unsafe TrinityErrorCode SaveCell(long cellId, byte* buff, int size, ushort cellType)
        {
            throw new NotImplementedException();
        }

        public unsafe void SendMessage(byte* message, int size)
        {
            throw new NotImplementedException();
        }

        public unsafe void SendMessage(byte* message, int size, out TrinityResponse response)
        {
            throw new NotImplementedException();
        }

        public unsafe void SendMessage(byte** message, int* sizes, int count)
        {
            throw new NotImplementedException();
        }

        public unsafe void SendMessage(byte** message, int* sizes, int count, out TrinityResponse response)
        {
            throw new NotImplementedException();
        }

        public unsafe TrinityErrorCode UpdateCell(long cellId, byte* buff, int size)
        {
            throw new NotImplementedException();
        }
    }
}
