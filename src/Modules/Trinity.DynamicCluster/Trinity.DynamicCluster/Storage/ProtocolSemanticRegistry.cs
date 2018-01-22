using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Network.Messaging;

namespace Trinity.DynamicCluster.Storage
{
    public static class ProtocolSemanticRegistry
    {
        internal static int[,] s_protocolSemantics;
        static ProtocolSemanticRegistry()
        {
            s_protocolSemantics = new int[6/*Nr. types of messages*/, ushort.MaxValue + 1];
            s_protocolSemantics.Initialize();
            //TODO configure default handler semantics
        }

        public static void RegisterProtocolSemantic(TrinityMessageType messageType, ushort messageId, ProtocolSemantic semantic)
        {
            int mt = (int)messageType;
            int ms = (int)semantic;
            if (mt < 0 || mt >= 6 || ms < 0 || ms >= (int)ProtocolSemantic.ProtocolSemanticEND)
            { throw new ArgumentOutOfRangeException(nameof(messageType)); }
            if (semantic == ProtocolSemantic.Broadcast            &&
                (messageType == TrinityMessageType.ASYNC_WITH_RSP ||
                 messageType == TrinityMessageType.SYNC_WITH_RSP  ||
                 messageType == TrinityMessageType.PRESERVED_SYNC_WITH_RSP))
            { throw new ArgumentException($"Cannot register {ProtocolSemantic.Broadcast} semantic for a protocol with response. Use {nameof(Partition.Broadcast)} method instead."); }
            s_protocolSemantics[mt, messageId] = ms;
        }
    }
}
