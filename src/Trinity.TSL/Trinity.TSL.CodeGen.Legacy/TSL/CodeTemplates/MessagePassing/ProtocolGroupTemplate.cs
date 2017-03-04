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
            CodeWriter cw = new CodeWriter();
            SpecificationScript script = SpecificationScript.CurrentScript;
            foreach (var pgroup_desc in descs)
            {
                #region Generate abstract Trinity server/proxy base class

                cw += @"
    /// <summary>
    /// Represents the base class of " + pgroup_desc.Name + @" defined in TSL.
    /// Inherit from this class to implement the logic of the server/proxy.
    /// </summary>
    public abstract partial class " + script.NamingPrefix + pgroup_desc.Name + @"Base";

                cw += " : " + base_class;

                cw += @"
    {
        #region Fields
        ";
                cw += @"#endregion
        protected override void RegisterMessageHandler()
        {
        ";
                foreach (var protocol_desc in pgroup_desc.ProtocolDescriptors)
                {
                    cw += "\t\t\t" + protocol_desc.RegistrationLine(script, pgroup_desc.Name, base_class, protocol_desc);
                }

                cw += @"
        }";

                foreach (var protocol_desc in pgroup_desc.ProtocolDescriptors)
                {
                    cw += protocol_desc.InternalHandlerBlock;
                    cw += "\r\n";

                    cw += protocol_desc.PublicHandlerBlock;

                    cw += "\r\n";
                }

                cw += @"
    }
";
        #endregion

            }

            return cw;
        }

    }
}
