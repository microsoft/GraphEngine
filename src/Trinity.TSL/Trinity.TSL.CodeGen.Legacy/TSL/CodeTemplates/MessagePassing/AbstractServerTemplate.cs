using Trinity.TSL;
using Trinity.Network.Messaging;
using System.Collections.Generic;
using Trinity.Utilities;

namespace Trinity.TSL
{
    internal static class AbstractServerTemplate
    {
        internal static string GenerateCode(SpecificationScript script)
        {
            return ProtocolGroupTemplate.GenerateCode(ProtocolGroupType.TrinityServer, script.ServerDescriptors);
        }
    }
}
