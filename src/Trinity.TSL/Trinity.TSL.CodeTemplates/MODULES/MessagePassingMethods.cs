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

namespace t_Namespace
{
    /// <summary>
    /// This module generates message passing methods.
    /// </summary>
    public static class MessagePassingExtensions
    {
        [MODULE_BEGIN]
        [TARGET("NProtocolGroup")]
        [MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")]
        [MAP_VAR("t_base_class_name", "get_comm_class_basename(node)")]
        [MAP_VAR("t_comm_name", "node->name")]
        [MAP_LIST("t_protocol", "node->protocolList")]
        [MAP_VAR("t_protocol", "referencedNProtocol")]
        [MAP_VAR("t_protocol_name", "name")]
        [MAP_VAR("t_protocol_type", "get_comm_protocol_type_string($$->referencedNProtocol)")]
        [MAP_VAR("t_protocol_request", "referencedNProtocol->request_message_struct")]
        [MAP_VAR("t_protocol_response", "referencedNProtocol->response_message_struct")]

        [META_VAR("std::string", "method_name")]
        [META_VAR("std::string", "arg_extension_method_target")]
        [META_VAR("std::string", "comm_id")]
        [META_VAR("std::string", "send_message_method")]
        [MAP_VAR("t_method_name", "%method_name")]
        [MAP_VAR("t_method_name_2", "%method_name")]
        [MAP_VAR("t_send_message", "%send_message_method")]
        [MAP_VAR("t_comm_id", "%comm_id")]

        [FOREACH]
        [USE_LIST("t_protocol")]

        #region prototype definition template variables
        [IF("node->type() == PGT_SERVER")]

        [META("%method_name = *$t_protocol_name + \"To\" + *$t_comm_name;")]
        [META("%arg_extension_method_target = \"this Trinity.Storage.MemoryCloud storage, \";")]
        [META("%comm_id = \"serverId\";")]
        [META("%send_message_method = \"storage.SendMessageToServer\";")]

        [ELIF("node->type() == PGT_PROXY")]

        [META("%method_name = *$t_protocol_name + \"To\" + *$t_comm_name;")]
        [META("%arg_extension_method_target = \"this Trinity.Storage.MemoryCloud storage, \";")]
        [META("%comm_id = \"proxyId\";")]
        [META("%send_message_method = \"storage.SendMessageToProxy\";")]

        [ELSE]//PGT_MODULE

        [META("%method_name = *$t_protocol_name;")]
        [META("%arg_extension_method_target = \"\";")]
        [META("%comm_id = \"moduleId\";")]
        [META("%send_message_method = \"this.SendMessage\";")]

        [END]
        #endregion

        [IF("!$t_protocol->has_request() && !$t_protocol->has_response()")]
        public unsafe /*IF("node->type() != PGT_MODULE")*/ static /*END*/ void t_method_name([META_OUTPUT("%arg_extension_method_target")] int t_comm_id)
        {
            byte* bufferPtr = (byte*)Memory.malloc((ulong)TrinityProtocol.MsgHeader);
            try
            {
                *(int*)(bufferPtr) = TrinityProtocol.TrinityMsgHeader;
                *(bufferPtr + TrinityProtocol.MsgTypeOffset) = (byte)__meta.META_OUTPUT("get_comm_protocol_trinitymessagetype($t_protocol)"); ;
                *(ushort*)(bufferPtr + TrinityProtocol.MsgIdOffset) = (ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name;
                t_send_message(t_comm_id, bufferPtr, TrinityProtocol.MsgHeader);
            }
            finally { Memory.free(bufferPtr); }
        }
        [ELIF("$t_protocol->has_request() && !$t_protocol->has_response()")]
        public unsafe/*IF("node->type() != PGT_MODULE")*/ static/*END*/ void t_method_name([META_OUTPUT("%arg_extension_method_target")] int t_comm_id, t_protocol_requestWriter msg)
        {
            byte* bufferPtr = msg.buffer;
            try
            {
                *(int*)(bufferPtr) = msg.Length + TrinityProtocol.TrinityMsgHeader;
                *(bufferPtr + TrinityProtocol.MsgTypeOffset) = (byte)__meta.META_OUTPUT("get_comm_protocol_trinitymessagetype($t_protocol)"); ;
                *(ushort*)(bufferPtr + TrinityProtocol.MsgIdOffset) = (ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name;
                t_send_message(t_comm_id, bufferPtr, msg.Length + TrinityProtocol.MsgHeader);
            }
            finally { }
        }
        [ELIF("!$t_protocol->has_request() && $t_protocol->has_response()")]
        public unsafe/*IF("node->type() != PGT_MODULE")*/ static/*END*/ t_protocol_responseReader t_method_name_2([META_OUTPUT("%arg_extension_method_target")] int t_comm_id)
        {
            byte* bufferPtr = (byte*)Memory.malloc((ulong)TrinityProtocol.MsgHeader);
            try
            {
                *(int*)(bufferPtr) = TrinityProtocol.TrinityMsgHeader;
                *(bufferPtr + TrinityProtocol.MsgTypeOffset) = (byte)__meta.META_OUTPUT("get_comm_protocol_trinitymessagetype($t_protocol)"); ;
                *(ushort*)(bufferPtr + TrinityProtocol.MsgIdOffset) = (ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name;
                TrinityResponse response;
                t_send_message(t_comm_id, bufferPtr, TrinityProtocol.MsgHeader, out response);
                return new t_protocol_responseReader(response.Buffer, response.Offset);
            }
            finally { Memory.free(bufferPtr); }
        }
        [ELSE]//$t_protocol->has_request() && $t_protocol->has_response()
        public unsafe/*IF("node->type() != PGT_MODULE")*/ static/*END*/ t_protocol_responseReader t_method_name_2([META_OUTPUT("%arg_extension_method_target")] int t_comm_id, t_protocol_requestWriter msg)
        {
            byte* bufferPtr = msg.buffer;
            try
            {
                *(int*)(bufferPtr) = msg.Length + TrinityProtocol.TrinityMsgHeader;
                *(bufferPtr + TrinityProtocol.MsgTypeOffset) = (byte)__meta.META_OUTPUT("get_comm_protocol_trinitymessagetype($t_protocol)"); ;
                *(ushort*)(bufferPtr + TrinityProtocol.MsgIdOffset) = (ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name;
                TrinityResponse response;
                t_send_message(t_comm_id, bufferPtr, msg.Length + TrinityProtocol.MsgHeader, out response);
                return new t_protocol_responseReader(response.Buffer, response.Offset);
            }
            finally { }
        }
        [END]

        [END]//FOREACH

        [MODULE_END]
        private static unsafe void t_send_message(int t_comm_id, byte* bufferPtr, int msgHeader)
        {
            throw new NotImplementedException();
        }
        private static unsafe void t_send_message(int t_comm_id, byte* bufferPtr, int msgHeader, out TrinityResponse response)
        {
            throw new NotImplementedException();
        }
    }

    public unsafe class t_protocol_requestWriter
    {
        internal byte* buffer;
        internal int offset;
        internal int Length;
    }

    public unsafe class t_protocol_responseReader
    {
        private byte* buffer;
        private int offset;

        public t_protocol_responseReader(byte* buffer, int offset)
        {
            this.buffer=buffer;
            this.offset=offset;
        }
    }
}
