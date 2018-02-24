#pragma warning disable 0162 // disable the "unreachable code" warning
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using t_Namespace.MODULES;
using Trinity;
using Trinity.Network;
using Trinity.Core.Lib;
using Trinity.Network.Messaging;
using Trinity.TSL;
using Trinity.TSL.Lib;
using Trinity.Storage;

namespace t_Namespace
{
    /// <summary>
    /// This module generates message passing methods.
    /// </summary>
    public static class LegacyMessagePassingExtensions
    {
        [MODULE_BEGIN]
        [TARGET("NProtocolGroup")]
        [MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")]
        [MAP_VAR("t_comm_name", "node->name")]
        [MAP_LIST("t_protocol", "node->protocolList")]
        [MAP_VAR("t_protocol", "referencedNProtocol")]
        [MAP_VAR("t_protocol_name", "name")]
        [MAP_VAR("t_protocol_name_2", "name")]
        [MAP_VAR("t_protocol_name_3", "name")]
        [MAP_VAR("t_protocol_request", "referencedNProtocol->request_message_struct")]
        [MAP_VAR("t_protocol_response", "referencedNProtocol->response_message_struct")]

        [META_VAR("std::string", "method_name")]
        [META_VAR("std::string", "storage_name")]
        [META_VAR("std::string", "arg_extension_method_target")]
        [MAP_VAR("t_storage", "%storage_name")]
        [MAP_VAR("t_method_name", "%method_name")]
        [MAP_VAR("t_method_name_2", "%method_name")]
        [MAP_VAR("t_method_name_3", "%method_name")]

        [FOREACH]
        [USE_LIST("t_protocol")]
        [IF("!$t_protocol->is_http_protocol()")]

        #region prototype definition template variables
        [IF("node->type() == PGT_SERVER")]

        [META("%method_name = *$t_protocol_name + \"To\" + *$t_comm_name;")]
        [META("%arg_extension_method_target = \"this Trinity.Storage.MemoryCloud storage, \";")]
        [META("%storage_name = \"storage\";")]

        [ELIF("node->type() == PGT_PROXY")]

        [META("%method_name = *$t_protocol_name + \"To\" + *$t_comm_name;")]
        [META("%arg_extension_method_target = \"this Trinity.Storage.MemoryCloud storage, \";")]
        [META("%storage_name = \"storage\";")]

        [ELSE]//PGT_MODULE

        [META("%method_name = *$t_protocol_name;")]
        [META("%arg_extension_method_target = \"\";")]
        [META("%storage_name = \"m_memorycloud\";")]

        [END]
        #endregion

        [IF("!$t_protocol->has_request() && !$t_protocol->has_response()")]
        public unsafe /*IF("node->type() != PGT_MODULE")*/ static /*END*/ void t_method_name([META_OUTPUT("%arg_extension_method_target")] int partitionId)
        {
            t_comm_name.MessagePassingExtension.t_protocol_name(t_storage[partitionId]);
        }
        [ELIF("$t_protocol->has_request() && !$t_protocol->has_response()")]
        public unsafe/*IF("node->type() != PGT_MODULE")*/ static/*END*/ void t_method_name([META_OUTPUT("%arg_extension_method_target")] int partitionId, t_protocol_requestWriter msg)
        {
            t_comm_name.MessagePassingExtension.t_protocol_name(t_storage[partitionId], msg);
        }
        [ELIF("!$t_protocol->has_request() && $t_protocol->is_syn_req_rsp_protocol()")]
        public unsafe/*IF("node->type() != PGT_MODULE")*/ static/*END*/ t_protocol_responseReader t_method_name_2([META_OUTPUT("%arg_extension_method_target")] int partitionId)
        {
            return t_comm_name.MessagePassingExtension.t_protocol_name_2(t_storage[partitionId]);
        }
        [ELIF("$t_protocol->has_request() && $t_protocol->is_syn_req_rsp_protocol()")]
        public unsafe/*IF("node->type() != PGT_MODULE")*/ static/*END*/ t_protocol_responseReader t_method_name_2([META_OUTPUT("%arg_extension_method_target")] int partitionId, t_protocol_requestWriter msg)
        {
            return t_comm_name.MessagePassingExtension.t_protocol_name_2(t_storage[partitionId], msg);
        }
        [ELIF("!$t_protocol->has_request() && $t_protocol->is_asyn_req_rsp_protocol()")]
        public unsafe/*IF("node->type() != PGT_MODULE")*/ static/*END*/ Task<t_protocol_responseReader> t_method_name_3([META_OUTPUT("%arg_extension_method_target")] int partitionId)
        {
            return t_comm_name.MessagePassingExtension.t_protocol_name_3(t_storage[partitionId]);
        }
        [ELSE]
        //("$t_protocol->has_request() && $t_protocol->is_asyn_req_rsp_protocol()")
        public unsafe/*IF("node->type() != PGT_MODULE")*/ static/*END*/ Task<t_protocol_responseReader> t_method_name_3([META_OUTPUT("%arg_extension_method_target")] int partitionId, t_protocol_requestWriter msg)
        {
            return t_comm_name.MessagePassingExtension.t_protocol_name_3(t_storage[partitionId], msg);
        }
        [END]

        [END]//IF not HTTP
        [END]//FOREACH

        [MODULE_END]
        private static MemoryCloud t_storage = null;
    }
}
