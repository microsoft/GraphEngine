using Trinity.TSL;
using Trinity.Network.Messaging;
using System.Collections.Generic;
using System.Linq;
using Trinity.Utilities;

namespace Trinity.TSL
{
    internal static class AbstractProxyTemplate
    {
        internal static string GenerateCode(SpecificationScript script)
        {
            return ProtocolGroupTemplate.GenerateCode(ProtocolGroupType.TrinityProxy, script.ProxyDescriptors);
        }
    }
}

