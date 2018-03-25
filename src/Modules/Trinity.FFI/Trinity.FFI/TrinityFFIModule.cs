using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Trinity.Extension;
using Trinity.Network;

namespace Trinity.FFI
{
    [AutoRegisteredCommunicationModule]
    public class TrinityFFIModule : FFIModuleBase
    {
        public override void AsynProtocolHandler(MessageRequestReader request)
        {
        }

        public override void SynProtocolHandler(MessageRequestReader request, MessageResponseWriter response)
        {
        }

        public override string GetModuleName()
        {
            return "Trinity.FFI";
        }
    }
}
