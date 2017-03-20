using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity.Network.Messaging;

namespace Trinity.TSL
{
    internal class ProtocolDescriptor : AbstractStruct
    {
        public TrinityMessageType ProtocolType;
        public StructDescriptor Request;
        public StructDescriptor Response;
    }
}
