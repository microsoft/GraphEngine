#pragma warning disable 0162 // disable the "unreachable code" warning
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity;
using Trinity.Network;
using Trinity.Network.Http;
using Trinity.TSL;
using Trinity.TSL.Lib;

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
    #region Server
    [FOREACH]
    [USE_LIST("t_server")]
    [MODULE_CALL("HTTPModule", "$t_server", "\"TrinityServer\"")]
    [END]
    #endregion

    #region Proxy
    [FOREACH]
    [USE_LIST("t_proxy")]
    [MODULE_CALL("HTTPModule", "$t_proxy", "\"TrinityProxy\"")]
    [END]
    #endregion

    #region Module
    [FOREACH]
    [USE_LIST("t_module")]
    [MODULE_CALL("HTTPModule", "$t_module", "\"CommunicationModule\"")]
    [END]
    #endregion

    [MUTE]
    class http_place_holder { }
    /*MUTE_END*/
}
