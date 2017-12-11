using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Trinity.Networking.WCFService
{
    [ServiceContract]
    public interface ITrinityWCFAdapter
    {
        [OperationContract(IsTerminating =false)]
        byte[] SendMessageWithResponse(byte[] request);
        void SendMessageWithoutResponse(byte[] request);
        //TODO async
        //Task<byte[]> AsynchronousProtocolWithResponse(int partId, ushort messageType, byte[] payload);
    }
}
