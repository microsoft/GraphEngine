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
    /// <summary>
    /// Simple pass-through IStorage, used to conduct module offset probing etc.
    /// during client startup sequence.
    /// When the client is started and registered, the PassThroughIStorage is discarded,
    /// and replaced by a number of RedirectedIStorage instances.
    /// </summary>
    internal class PassThroughIStorage : IStorage
    {
        private IMessagePassingEndpoint m_ep;
        #region Unsupported
        public unsafe TrinityErrorCode AddCell(long cellId, byte* buff, int size, ushort cellType)
        {
            throw new NotImplementedException();
        }

        public bool Contains(long cellId)
        {
            throw new NotImplementedException();
        }

        public void Dispose() { }

        public TrinityErrorCode GetCellType(long cellId, out ushort cellType)
        {
            throw new NotImplementedException();
        }

        public T GetCommunicationModule<T>() where T : CommunicationModule
        {
            throw new NotImplementedException();
        }

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

        public unsafe TrinityErrorCode UpdateCell(long cellId, byte* buff, int size)
        {
            throw new NotImplementedException();
        }
        #endregion

        public PassThroughIStorage(IMessagePassingEndpoint ep)
        {
            this.m_ep = ep;
        }

        public unsafe void SendMessage(byte* message, int size)
            => m_ep.SendMessage(message, size);

        public unsafe void SendMessage(byte* message, int size, out TrinityResponse response)
            => m_ep.SendMessage(message, size, out response);

        public unsafe void SendMessage(byte** message, int* sizes, int count)
            => m_ep.SendMessage(message, sizes, count);

        public unsafe void SendMessage(byte** message, int* sizes, int count, out TrinityResponse response)
            => m_ep.SendMessage(message, sizes, count, out response);
    }
}
