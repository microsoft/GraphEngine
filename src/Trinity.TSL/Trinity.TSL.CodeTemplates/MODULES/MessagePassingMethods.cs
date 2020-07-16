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
    public static class MessagePassingMethodsModule
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
        [META_VAR("std::string", "send_message_method")]
        [META_VAR("std::string", "send_recv_message_method")]
        [MAP_VAR("t_method_name", "%method_name")]
        [MAP_VAR("t_method_name_2", "%method_name")]
        [MAP_VAR("t_method_name_3", "%method_name")]
        [MAP_VAR("t_send_message", "%send_message_method")]
        [MAP_VAR("t_send_recv_message", "%send_recv_message_method")]

        [FOREACH]
        [USE_LIST("t_protocol")]
        [IF("!$t_protocol->is_http_protocol()")]

        #region prototype definition template variables
        [META("%method_name = *$t_protocol_name;")]
        [IF("node->type() == PGT_SERVER || node->type() == PGT_PROXY")]

        [META("%send_message_method = \"storage.SendMessageAsync\";")]
        [META("%send_recv_message_method = \"storage.SendRecvMessageAsync\";")]
        [ELSE]//PGT_MODULE

        [META("%send_message_method = \"storage.SendMessageAsync<\" + *node->name + \"Base>\";")]
        [META("%send_recv_message_method = \"storage.SendRecvMessageAsync<\" + *node->name + \"Base>\";")]

        [END]
        #endregion

        [IF("!$t_protocol->has_request() && !$t_protocol->has_response()")]
        public unsafe static Task t_method_nameAsync(this Trinity.Storage.IMessagePassingEndpoint storage)
        {
            byte* bufferPtr = stackalloc byte[TrinityProtocol.MsgHeader];
            *(int*)(bufferPtr) = TrinityProtocol.TrinityMsgHeader;
            *(TrinityMessageType*)(bufferPtr + TrinityProtocol.MsgTypeOffset) = __meta.META_OUTPUT("get_comm_protocol_trinitymessagetype($t_protocol)"); ;
            *(ushort*)(bufferPtr + TrinityProtocol.MsgIdOffset) = (ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name;
            return t_send_message(bufferPtr, TrinityProtocol.MsgHeader);
        }
        [ELIF("$t_protocol->has_request() && !$t_protocol->has_response()")]
        public unsafe static Task t_method_nameAsync(this Trinity.Storage.IMessagePassingEndpoint storage, t_protocol_requestWriter msg)
        {
            byte* bufferPtr = msg.buffer;
            *(int*)(bufferPtr) = msg.Length + TrinityProtocol.TrinityMsgHeader;
            *(TrinityMessageType*)(bufferPtr + TrinityProtocol.MsgTypeOffset) = __meta.META_OUTPUT("get_comm_protocol_trinitymessagetype($t_protocol)"); ;
            *(ushort*)(bufferPtr + TrinityProtocol.MsgIdOffset) = (ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name;
            return t_send_message(bufferPtr, msg.Length + TrinityProtocol.MsgHeader);
        }
        [ELIF("!$t_protocol->has_request() && $t_protocol->is_syn_req_rsp_protocol()")]
        public unsafe static Task<t_protocol_responseReader> t_method_name_2Async(this Trinity.Storage.IMessagePassingEndpoint storage)
        {
            byte* bufferPtr = stackalloc byte[TrinityProtocol.MsgHeader];
            *(int*)(bufferPtr) = TrinityProtocol.TrinityMsgHeader;
            *(TrinityMessageType*)(bufferPtr + TrinityProtocol.MsgTypeOffset) = __meta.META_OUTPUT("get_comm_protocol_trinitymessagetype($t_protocol)"); ;
            *(ushort*)(bufferPtr + TrinityProtocol.MsgIdOffset) = (ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name;
            return t_send_recv_message(bufferPtr, TrinityProtocol.MsgHeader)
                          .ContinueWith(
                            t =>
                            {
                                TrinityResponse response = t.Result;
                                return new t_protocol_responseReader(response.Buffer, response.Offset);
                            },
                            TaskContinuationOptions.ExecuteSynchronously);
        }
        [ELIF("$t_protocol->has_request() && $t_protocol->is_syn_req_rsp_protocol()")]
        public unsafe static Task<t_protocol_responseReader> t_method_name_2Async(this Trinity.Storage.IMessagePassingEndpoint storage, t_protocol_requestWriter msg)
        {
            byte* bufferPtr = msg.buffer;
            *(int*)(bufferPtr) = msg.Length + TrinityProtocol.TrinityMsgHeader;
            *(TrinityMessageType*)(bufferPtr + TrinityProtocol.MsgTypeOffset) = __meta.META_OUTPUT("get_comm_protocol_trinitymessagetype($t_protocol)"); ;
            *(ushort*)(bufferPtr + TrinityProtocol.MsgIdOffset) = (ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name;
            return t_send_recv_message(bufferPtr, msg.Length + TrinityProtocol.MsgHeader)
                          .ContinueWith(
                            t =>
                            {
                                TrinityResponse response = t.Result;
                                return new t_protocol_responseReader(response.Buffer, response.Offset);
                            },
                            TaskContinuationOptions.ExecuteSynchronously);
        }
        [ELIF("!$t_protocol->has_request() && $t_protocol->is_asyn_req_rsp_protocol()")]
        public unsafe static Task<t_protocol_responseReader> t_method_name_3Async(this Trinity.Storage.IMessagePassingEndpoint storage)
        {
            byte* bufferPtr = stackalloc byte[TrinityProtocol.MsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength];
            int token = Interlocked.Increment(ref t_comm_nameBase.s_t_protocol_name_token_counter);
            *(int*)(bufferPtr + TrinityProtocol.MsgHeader) = token;
            *(int*)(bufferPtr + TrinityProtocol.MsgHeader + sizeof(int)) = Global.CloudStorage.MyInstanceId;
            *(int*)(bufferPtr) = TrinityProtocol.TrinityMsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength;
            *(TrinityMessageType*)(bufferPtr + TrinityProtocol.MsgTypeOffset) = __meta.META_OUTPUT("get_comm_protocol_trinitymessagetype($t_protocol)"); ;
            *(ushort*)(bufferPtr + TrinityProtocol.MsgIdOffset) = (ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name;
            return t_send_recv_message(bufferPtr, TrinityProtocol.MsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength)
                          .ContinueWith(
                            t =>
                            {
                                TrinityResponse response = t.Result;
                                return new t_protocol_responseReader(response.Buffer, response.Offset);
                            },
                            TaskContinuationOptions.ExecuteSynchronously);
        }
        [ELSE]
        //("$t_protocol->has_request() && $t_protocol->is_asyn_req_rsp_protocol()")
        public unsafe static Task<t_protocol_responseReader> t_method_name_3Async(this Trinity.Storage.IMessagePassingEndpoint storage, t_protocol_requestWriter msg)
        {
            byte** bufferPtrs = stackalloc byte*[2];
            int*   size       = stackalloc int[2];
            byte*  bufferPtr  = stackalloc byte[TrinityProtocol.MsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength];
            bufferPtrs[0]     = bufferPtr;
            bufferPtrs[1]     = msg.buffer + TrinityProtocol.MsgHeader;
            size[0]           = TrinityProtocol.MsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength;
            size[1]           = msg.Length;

            int token = Interlocked.Increment(ref t_comm_nameBase.s_t_protocol_name_token_counter);
            *(int*)(bufferPtr) = TrinityProtocol.TrinityMsgHeader + msg.Length + TrinityProtocol.AsyncWithRspAdditionalHeaderLength;
            *(TrinityMessageType*)(bufferPtr + TrinityProtocol.MsgTypeOffset) = __meta.META_OUTPUT("get_comm_protocol_trinitymessagetype($t_protocol)"); ;
            *(ushort*)(bufferPtr + TrinityProtocol.MsgIdOffset) = (ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name;
            *(int*)(bufferPtr + TrinityProtocol.MsgHeader) = token;
            *(int*)(bufferPtr + TrinityProtocol.MsgHeader + sizeof(int)) = Global.CloudStorage.MyInstanceId;
            return t_send_recv_message(bufferPtrs, size, 2)
                          .ContinueWith(
                            t =>
                            {
                                TrinityResponse response = t.Result;
                                return new t_protocol_responseReader(response.Buffer, response.Offset);
                            },
                            TaskContinuationOptions.ExecuteSynchronously);
        }


        [END]

        [END]//IF not HTTP
        [END]//FOREACH

        [MODULE_END]
        private static unsafe Task t_send_message(byte** bufferPtrs, int* size, int v)
        {
            throw new NotImplementedException();
        }
        private static unsafe Task t_send_message(byte* bufferPtr, int msgHeader)
        {
            throw new NotImplementedException();
        }
        private static unsafe Task<TrinityResponse> t_send_recv_message(byte** bufferPtrs, int* size, int v)
        {
            throw new NotImplementedException();
        }
        private static unsafe Task<TrinityResponse> t_send_recv_message(byte* bufferPtr, int msgHeader)
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
