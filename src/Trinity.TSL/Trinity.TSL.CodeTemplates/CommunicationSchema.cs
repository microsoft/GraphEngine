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
using Trinity.Network.Messaging;
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

    internal class ProtocolDescriptor : IProtocolDescriptor
    {
        public string Name
        {
            get;
            set;
        }

        public string RequestSignature
        {
            get;
            set;
        }

        public string ResponseSignature
        {
            get;
            set;
        }

        public TrinityMessageType Type
        {
            get;
            set;
        }
    }

    #region Server
    [FOREACH]
    [USE_LIST("t_server")]
    [MODULE_CALL("CommunicationSchemaModule", "$t_server", "\"TrinityServer\"")]
    [END]
    #endregion

    #region Proxy
    [FOREACH]
    [USE_LIST("t_proxy")]
    [MODULE_CALL("CommunicationSchemaModule", "$t_proxy", "\"TrinityProxy\"")]
    [END]
    #endregion

    #region Module
    [FOREACH]
    [USE_LIST("t_module")]
    [MODULE_CALL("CommunicationSchemaModule", "$t_module", "\"CommunicationModule\"")]
    [END]
    #endregion

    [MUTE]
    class comm_schema_place_holder { }
    /*MUTE_END*/
}
