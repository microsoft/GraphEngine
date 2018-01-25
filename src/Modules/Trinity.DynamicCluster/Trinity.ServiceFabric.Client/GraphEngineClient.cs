using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Network;
using Trinity.Network.Messaging;
using Trinity.Storage;

namespace Trinity.ServiceFabric.Client
{
    public partial class GraphEngineClient : MemoryCloud, IMessagePassingEndpoint
    {
        public T GetModule<T>() where T : CommunicationModule => GetCommunicationModule<T>();


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
    }
}
