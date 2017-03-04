using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL;

/*MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")*/
namespace t_Namespace
{
    [TARGET("NTSL")]
    [MAP_LIST("t_module", "node->moduleList")]
    [MAP_LIST("t_protocol", "protocolList", MemberOf = "t_module")]
    [FOREACH]
    public abstract partial class t_module_name : Trinity.Network.CommunicationModule
    {
        protected override void RegisterMessageHandler()
        {
        }
    }
    /*END*/
}
