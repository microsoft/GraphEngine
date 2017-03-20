using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL;
using Trinity.Network;
using Trinity.Network.Messaging;

/*MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")*/
namespace t_Namespace
{
    [TARGET("NTSL")]
    [MAP_LIST("t_server", "node->serverList")]
    [MAP_LIST("t_proxy", "node->proxyList")]
    [MAP_LIST("t_module", "node->moduleList")]
    [MAP_VAR("t_server", "")]
    [MAP_VAR("t_proxy", "")]
    [MAP_VAR("t_module", "")]

    [FOREACH]
    [USE_LIST("t_server")]
    [MODULE_CALL("CommunicationClass", "$t_server")]
    [END]

    [FOREACH]
    [USE_LIST("t_proxy")]
    [MODULE_CALL("CommunicationClass", "$t_proxy")]
    [END]

    [FOREACH]
    [USE_LIST("t_module")]
    [MODULE_CALL("CommunicationClass", "$t_module")]
    [END]

    [MUTE]
    class protocol_place_holder { }
    /*MUTE_END*/
}
