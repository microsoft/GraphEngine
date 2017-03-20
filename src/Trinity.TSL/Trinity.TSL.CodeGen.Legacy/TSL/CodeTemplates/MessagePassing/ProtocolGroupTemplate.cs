using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Network.Messaging;
using Trinity.Utilities;

namespace Trinity.TSL
{
    enum ProtocolGroupType
    {
        TrinityServer,
        TrinityProxy,
        CommunicationModule
    }
    /// <summary>
    /// Shared class to generate code for Proxy/server.
    /// </summary>
    class ProtocolGroupTemplate
    {
        internal static string GenerateCode(ProtocolGroupType base_class, List<ProtocolGroupDescriptor> descs)
        {
            return "";
        }

    }
}
