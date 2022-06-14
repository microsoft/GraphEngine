using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Network;
using Trinity.Network.Messaging;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Storage
{
    public partial class Partition : IStorage
    {
        public T GetCommunicationModule<T>() where T : CommunicationModule
            => Global.CommunicationInstance.GetCommunicationModule<T>();

        public unsafe void SendMessage(byte* message, int size)
        {
            var msg_type = *(TrinityMessageType*)(message + TrinityProtocol.MsgTypeOffset);
            ushort msg_id = *(ushort*)(message + TrinityProtocol.MsgIdOffset);
            int ms = ProtocolSemanticRegistry.s_protocolSemantics[(int)msg_type, msg_id];
            m_smfuncs[ms](message, size);
        }

        public unsafe void SendMessage(byte* message, int size, out TrinityResponse response)
        {
            var msg_type = *(TrinityMessageType*)(message + TrinityProtocol.MsgTypeOffset);
            ushort msg_id = *(ushort*)(message + TrinityProtocol.MsgIdOffset);
            int ms = ProtocolSemanticRegistry.s_protocolSemantics[(int)msg_type, msg_id];
            response = m_smrfuncs[ms](message, size);
        }

        public unsafe void SendMessage(byte** message, int* sizes, int count)
        {
            var msg_type = PointerHelper.GetUshort(message, sizes, TrinityProtocol.MsgTypeOffset);
            ushort msg_id = PointerHelper.GetUshort(message, sizes, TrinityProtocol.MsgIdOffset);
            int ms = ProtocolSemanticRegistry.s_protocolSemantics[msg_type, msg_id];
            m_smmfuncs[ms](message, sizes, count);
        }

        public unsafe void SendMessage(byte** message, int* sizes, int count, out TrinityResponse response)
        {
            var msg_type = PointerHelper.GetUshort(message, sizes, TrinityProtocol.MsgTypeOffset);
            ushort msg_id = PointerHelper.GetUshort(message, sizes, TrinityProtocol.MsgIdOffset);
            int ms = ProtocolSemanticRegistry.s_protocolSemantics[msg_type, msg_id];
            response = m_smrmfuncs[ms](message, sizes, count);
        }
    }
}
