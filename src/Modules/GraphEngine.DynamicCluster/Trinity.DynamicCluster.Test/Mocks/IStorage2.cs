using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Network;
using Trinity.Network.Messaging;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Test.Mocks
{
    class IStorage2 : IStorage
    {
        public unsafe Task<TrinityErrorCode> AddCellAsync(long cellId, byte* buff, int size, ushort cellType)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ContainsAsync(long cellId)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public Task<(TrinityErrorCode ErrorCode, ushort CellType)> GetCellTypeAsync(long cellId)
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

        public unsafe Task SendMessageAsync(byte* message, int size)
        {
            throw new IOException();
        }

        public unsafe Task SendMessageAsync(byte** message, int* sizes, int count)
        {
            throw new NotImplementedException();
        }

        public unsafe Task<TrinityResponse> SendRecvMessageAsync(byte* message, int size)
        {
            throw new IOException();
        }

        public unsafe Task<TrinityResponse> SendRecvMessageAsync(byte** message, int* sizes, int count)
        {
            throw new NotImplementedException();
        }

        public unsafe Task<TrinityErrorCode> UpdateCellAsync(long cellId, byte* buff, int size)
        {
            throw new NotImplementedException();
        }
    }
}
