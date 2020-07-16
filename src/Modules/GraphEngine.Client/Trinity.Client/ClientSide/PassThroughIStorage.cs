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
        public unsafe Task<TrinityErrorCode> AddCellAsync(long cellId, byte* buff, int size, ushort cellType)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ContainsAsync(long cellId)
        {
            throw new NotImplementedException();
        }

        public void Dispose() { }

        public Task<(TrinityErrorCode, ushort)> GetCellTypeAsync(long cellId)
        {
            throw new NotImplementedException();
        }

        public T GetCommunicationModule<T>() where T : CommunicationModule
        {
            throw new NotImplementedException();
        }

        public Task<LoadCellResponse> LoadCellAsync(long cellId)
        {
            throw new NotImplementedException();
        }

        public Task<LoadCellUnsafeResponse> LoadCellUnsafeAsync(long cellId)
        {
            throw new NotImplementedException();
        }

        public Task<TrinityErrorCode> RemoveCellAsync(long cellId)
        {
            throw new NotImplementedException();
        }

        public unsafe Task<TrinityErrorCode> SaveCellAsync(long cellId, byte* buff, int size, ushort cellType)
        {
            throw new NotImplementedException();
        }

        public unsafe Task<TrinityErrorCode> UpdateCellAsync(long cellId, byte* buff, int size)
        {
            throw new NotImplementedException();
        }
        #endregion

        public PassThroughIStorage(IMessagePassingEndpoint ep)
        {
            this.m_ep = ep;
        }

        public unsafe Task SendMessageAsync(byte* message, int size)
            => m_ep.SendMessageAsync(message, size);

        public unsafe Task<TrinityResponse> SendRecvMessageAsync(byte* message, int size)
            => m_ep.SendRecvMessageAsync(message, size);

        public unsafe Task SendMessageAsync(byte** message, int* sizes, int count)
            => m_ep.SendMessageAsync(message, sizes, count);

        public unsafe Task<TrinityResponse> SendRecvMessageAsync(byte** message, int* sizes, int count)
            => m_ep.SendRecvMessageAsync(message, sizes, count);
    }
}
