using System.Fabric;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Trinity.Core.Lib;
using Trinity.Network;
using Trinity.Network.Messaging;
using Trinity.ServiceFabric.NativeClient.Remoting.Interfaces;

namespace Trinity.ServiceFabric.NativeClient.Remoting
{
    public class TrinityOverNativeTCPCommunicationService : ITrinityOverNativeTCPCommunicationService
    {
        private MessageDispatchProc TrinityMessageDispatcher { get; }
        private StatefulServiceContext GraphEngineServiceContext { get; }

        private FabricTransportServiceRemotingListener m_fabListener = null;
        //ivate TrinityOverRemotingService m_trinityProxy = null;

        public TrinityOverNativeTCPCommunicationService(StatefulServiceContext graphEngineStatefulContext)
        {
            GraphEngineServiceContext = graphEngineStatefulContext;
            TrinityMessageDispatcher  = Global.CommunicationInstance.MessageDispatcher;

            //m_fabListener = new FabricTransportServiceRemotingListener(graphEngineStatefulContext,
            //    this,
            //    new FabricTransportRemotingListenerSettings
            //    {
            //        EndpointResourceName = Constants.c_TrinityNativeEndpointName,
            //        //TODO security stuff
            //    });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public unsafe Task SendMessageAsync(byte[] message)
        {
            fixed (byte* p = message)
            {
                MessageBuff buff = new MessageBuff { Buffer = p, Length = (uint)message.Length };
                TrinityMessageDispatcher(&buff);
                try
                {
                    int errorCode = *(int*)buff.Buffer;
                    if (errorCode < 0) return Task.FromException(new IOException("Error occured while processing the message"));
                    else return Task.FromResult(0);
                }
                finally
                {
                    Memory.free(buff.Buffer);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public unsafe Task<byte[]> SendMessageWithResponseAsync(byte[] message)
        {
            MessageBuff buff = new MessageBuff();
            fixed (byte* p = message)
            {
                buff.Buffer = p;
                buff.Length = (uint)message.Length;
                TrinityMessageDispatcher(&buff);
            }

            try
            {
                int len = *(int*)buff.Buffer;
                if (len < 0) return Task.FromException<byte[]>(new IOException("Error occured while processing the message"));
                byte[] buf = new byte[len];
                fixed (byte* p = buf)
                {
                    Memory.memcpy(p, buff.Buffer + TrinityProtocol.SocketMsgHeader, (uint)len);
                }
                return Task.FromResult(buf);
            }
            finally
            {
                Memory.free(buff.Buffer);
            }
        }
    }
}
