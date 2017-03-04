using Trinity.TSL;
using Trinity.Network.Messaging;
using System.Collections.Generic;
using Trinity.Utilities;

namespace Trinity.TSL
{
    internal static class AbstractModuleTemplate
    {
        internal static string GenerateCode(SpecificationScript script)
        {
            return ProtocolGroupTemplate.GenerateCode(ProtocolGroupType.CommunicationModule, script.ModuleDescriptors);
        }
    }
}
