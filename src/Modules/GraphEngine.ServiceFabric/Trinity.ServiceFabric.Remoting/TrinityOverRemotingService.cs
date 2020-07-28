using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Trinity.Core.Lib;
using Trinity.Network;
using Trinity.Network.Messaging;

namespace Trinity.ServiceFabric.Remoting
{
    class TrinityOverRemotingService : ITrinityOverRemotingService
    {
        private readonly MessageDispatchProc m_dispatcher;

        public TrinityOverRemotingService()
        {
            m_dispatcher =  Global.CommunicationInstance.MessageDispatcher;
        }

        public unsafe Task SendMessageAsync(byte[] message)
        {
            fixed (byte* p = message)
            {
                MessageBuff buff = new MessageBuff{ Buffer = p, Length = (uint)message.Length };
                m_dispatcher(&buff);
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

        public unsafe Task<byte[]> SendMessageWithResponseAsync(byte[] message)
        {
            MessageBuff buff = new MessageBuff();
            fixed (byte* p = message)
            {
                buff.Buffer = p;
                buff.Length = (uint)message.Length;
                m_dispatcher(&buff);
            }

            try
            {
                int len = *(int*)buff.Buffer;
                if (len < 0) return Task.FromException<byte[]>(new IOException("Error occured while processing the message"));
                byte[] buf = new byte[len];
                fixed(byte* p = buf)
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
