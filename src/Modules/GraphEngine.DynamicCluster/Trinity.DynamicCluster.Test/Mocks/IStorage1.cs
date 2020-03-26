using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Trinity.Network;
using Trinity.Network.Messaging;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Test.Mocks
{
    class IStorage1 : IStorage
    {
        public int cnt = 0;

        public bool SendMessageCalledOnce => cnt == 1;

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
        {
            throw new NotImplementedException();
        }

        public TrinityErrorCode LoadCell(long cellId, out byte[] cellBuff, out ushort cellType)
        {
            throw new NotImplementedException();
        }

        public unsafe TrinityErrorCode LoadCell(long cellId, out byte* cellBuff, out int size, out ushort cellType)
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
            ++cnt;
        }

        public unsafe void SendMessage(byte* message, int size, out TrinityResponse response)
        {
            ++cnt;
            response = typeof(TrinityResponse).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(int) }, new ParameterModifier[] { }).Invoke(new object[] { 1 }) as TrinityResponse;
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
