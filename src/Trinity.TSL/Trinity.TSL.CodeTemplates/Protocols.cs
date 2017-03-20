using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL;
using Trinity.Core.Lib;
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
    [MAP_VAR("t_module_name", "name")]

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

    public static class MessagePassingExtension
    {
        #region Server
        [FOREACH]
        [USE_LIST("t_server")]
        [MODULE_CALL("MessagePassingMethods", "$t_server")]
        [END]
        #endregion

        #region Proxy
        [FOREACH]
        [USE_LIST("t_proxy")]
        [MODULE_CALL("MessagePassingMethods", "$t_proxy")]
        [END]
        #endregion

        [MUTE]
        class mpi_place_holder { }
        /*MUTE_END*/
    }

    #region Module
    [FOREACH]
    [USE_LIST("t_module")]
    public abstract partial class t_module_nameBase : CommunicationModule
    {
        [MODULE_CALL("MessagePassingMethods", "$t_module")]
        [MUTE]
        class mpi_place_holder { }
        /*MUTE_END*/
    }
    /*END*/
    #endregion


    [MUTE]
    class protocol_place_holder { }
    /*MUTE_END*/
}
