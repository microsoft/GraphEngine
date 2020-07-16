using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using t_Namespace.MODULES;
using Trinity;
using Trinity.Core.Lib;
using Trinity.Network.Messaging;
using Trinity.Storage;
using Trinity.TSL;

namespace t_Namespace
{
    [MODULE_BEGIN]
    [TARGET("NProtocolGroup")]
    [MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")]
    [MAP_VAR("t_base_class_name", "get_comm_class_basename(node)")]
    [MAP_VAR("t_comm_name", "node->name")]
    [MAP_LIST("t_protocol", "node->protocolList")]
    [MAP_VAR("t_protocol", "referencedNProtocol")]
    [MAP_VAR("t_protocol_name", "name")]
    [MAP_VAR("t_protocol_name_2", "name")]
    [MAP_VAR("t_protocol_name_3", "name")]
    [MAP_VAR("t_protocol_name_4", "name")]
    [MAP_VAR("t_protocol_name_5", "name")]
    [MAP_VAR("t_protocol_request", "referencedNProtocol->request_message_struct")]
    [MAP_VAR("t_protocol_response", "referencedNProtocol->response_message_struct")]
    [MAP_VAR("t_protocol_type", "get_comm_protocol_type_string($$->referencedNProtocol)")]
    public abstract partial class t_comm_nameBase : t_base_class_name
    {
        protected override void RegisterMessageHandler()
        {
            FOREACH();
            META("if($t_protocol->is_http_protocol()){continue;}");
            IF("$t_protocol->is_asyn_req_rsp_protocol()");
            {
                IF("node->type() == PGT_MODULE");
                MessageRegistry.RegisterMessageHandler((ushort)(this.t_protocol_typeIdOffset + (ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name), _t_protocol_name_4HandlerAsync);
                MessageRegistry.RegisterMessageHandler((ushort)(this.t_protocol_typeIdOffset + (ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name__Response), _t_protocol_name_ResponseHandlerAsync);
                ELSE();
                MessageRegistry.RegisterMessageHandler((ushort)(ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name, _t_protocol_name_4HandlerAsync);
                MessageRegistry.RegisterMessageHandler((ushort)(ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name__Response, _t_protocol_name_ResponseHandlerAsync);
                END();
            }
            ELSE();
            {
                IF("node->type() == PGT_MODULE");
                MessageRegistry.RegisterMessageHandler((ushort)(this.t_protocol_typeIdOffset + (ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name), _t_protocol_name_4HandlerAsync);
                ELSE();
                MessageRegistry.RegisterMessageHandler((ushort)(ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name, _t_protocol_name_4HandlerAsync);
                END();
            }
            END();//IF
            END();//FOREACH
        }

        [MUTE]
        Task _t_protocol_name_4HandlerAsync(SynReqArgs args) { return Task.CompletedTask; }
        [MUTE_END]

        [FOREACH]
        [META("if($t_protocol->is_http_protocol()){continue;}")]

        [IF("!$t_protocol->has_response()")]
        //SYNC and ASYNC, no response;
        private unsafe Task _t_protocol_nameHandlerAsync(t_protocol_typeArgs args)
        {
            IF("$t_protocol->has_request()");
            return t_protocol_nameHandlerAsync(new t_protocol_requestReader(args.Buffer, args.Offset));
            ELSE();
            return t_protocol_nameHandlerAsync();
            END();
        }

        [IF("$t_protocol->has_request()")]
        public abstract Task t_protocol_nameHandlerAsync(t_protocol_requestReader request);
        [ELSE]
        public abstract Task t_protocol_nameHandlerAsync();
        [END]

        //NO RESPONSE END
        [ELIF("$t_protocol->has_request() && $t_protocol->is_syn_req_rsp_protocol()")]
        //SYNC_WITH_RSP, request is not void
        private unsafe Task _t_protocol_name_2HandlerAsync(t_protocol_typeArgs args)
        {
            var rsp = new t_protocol_responseWriter();
            return t_protocol_nameHandlerAsync(new t_protocol_requestReader(args.Buffer, args.Offset), rsp).ContinueWith(
                t =>
                {
                    *(int*)(rsp.m_ptr - TrinityProtocol.MsgHeader) = rsp.Length + TrinityProtocol.TrinityMsgHeader;
                    args.Response = new TrinityMessage(rsp.buffer, rsp.Length + TrinityProtocol.MsgHeader);
                },
                TaskContinuationOptions.ExecuteSynchronously);
        }

        public abstract Task t_protocol_nameHandlerAsync(t_protocol_requestReader request, t_protocol_responseWriter response);
        [ELIF("!$t_protocol->has_request() && $t_protocol->is_syn_req_rsp_protocol()")]
        private unsafe Task _t_protocol_name_3HandlerAsync(t_protocol_typeArgs args)
        {
            var rsp = new t_protocol_responseWriter();
            return t_protocol_nameHandlerAsync(rsp).ContinueWith(
                t =>
                {
                    *(int*)(rsp.m_ptr - TrinityProtocol.MsgHeader) = rsp.Length + TrinityProtocol.TrinityMsgHeader;
                    args.Response = new TrinityMessage(rsp.buffer, rsp.Length + TrinityProtocol.MsgHeader);
                },
                TaskContinuationOptions.ExecuteSynchronously);
        }

        public abstract Task t_protocol_nameHandlerAsync(t_protocol_responseWriter response);
        [ELIF("$t_protocol->has_request() && $t_protocol->is_asyn_req_rsp_protocol()")]
        //ASYNC_WITH_RSP, request is not void
        private unsafe Task _t_protocol_name_4HandlerAsync(t_protocol_typeArgs args)
        {
            var rsp = new t_protocol_responseWriter(asyncRspHeaderLength: TrinityProtocol.AsyncWithRspAdditionalHeaderLength);
            var req = new t_protocol_requestReader(args.Buffer, args.Offset + TrinityProtocol.AsyncWithRspAdditionalHeaderLength);
            return t_protocol_nameHandlerAsync(req, rsp)
                .ContinueWith(
                    t =>
                    {
                        Exception exception = t.Exception;
                        int token = *(int*)(args.Buffer + args.Offset);
                        int from = *(int*)(args.Buffer + args.Offset + sizeof(int));
                        if (exception != null) return _t_protocol_name_CheckErrorAsync(exception, token, from);
                        *(int*)(rsp.buffer) = TrinityProtocol.TrinityMsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength + rsp.Length;
                        *(TrinityMessageType*)(rsp.buffer + TrinityProtocol.MsgTypeOffset) = TrinityMessageType.ASYNC_WITH_RSP;
                        *(ushort*)(rsp.buffer + TrinityProtocol.MsgIdOffset) = (ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name__Response;
                        *(int*)(rsp.m_ptr - TrinityProtocol.AsyncWithRspAdditionalHeaderLength) = token;
                        *(int*)(rsp.m_ptr - TrinityProtocol.AsyncWithRspAdditionalHeaderLength + sizeof(int)) = 0;
                        IF("node->type() == PGT_MODULE");
                        return this.SendMessageAsync(m_memorycloud[from], rsp.buffer, rsp.Length + TrinityProtocol.MsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength);
                        ELSE();
                        return Global.CloudStorage[from].SendMessageAsync(rsp.buffer, rsp.Length + TrinityProtocol.MsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength);
                        END();
                    })
                .Unwrap()
                .ContinueWith(t => rsp?.Dispose(), TaskContinuationOptions.ExecuteSynchronously);
        }

        public abstract Task t_protocol_name_2HandlerAsync(t_protocol_requestReader request, t_protocol_responseWriter response);
        [ELSE]
        //("!$t_protocol->has_request() && $t_protocol->is_asyn_req_rsp_protocol()")
        private unsafe Task _t_protocol_name_5HandlerAsync(t_protocol_typeArgs args)
        {
            var rsp = new t_protocol_responseWriter(asyncRspHeaderLength: TrinityProtocol.AsyncWithRspAdditionalHeaderLength);
            return t_protocol_nameHandlerAsync(rsp)
                .ContinueWith(
                    t =>
                    {
                        Exception exception = t.Exception;
                        int token = *(int*)(args.Buffer + args.Offset);
                        int from = *(int*)(args.Buffer + args.Offset + sizeof(int));
                        if (exception != null) return _t_protocol_name_CheckErrorAsync(exception, token, from);
                        *(int*)(rsp.buffer) = TrinityProtocol.TrinityMsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength + rsp.Length;
                        *(TrinityMessageType*)(rsp.buffer + TrinityProtocol.MsgTypeOffset) = TrinityMessageType.ASYNC_WITH_RSP;
                        *(ushort*)(rsp.buffer + TrinityProtocol.MsgIdOffset) = (ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name__Response;
                        *(int*)(rsp.m_ptr - TrinityProtocol.AsyncWithRspAdditionalHeaderLength) = token;
                        *(int*)(rsp.m_ptr - TrinityProtocol.AsyncWithRspAdditionalHeaderLength + sizeof(int)) = 0;
                        IF("node->type() == PGT_MODULE");
                        return this.SendMessageAsync(m_memorycloud[from], rsp.buffer, rsp.Length + TrinityProtocol.MsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength);
                        ELSE();
                        return Global.CloudStorage[from].SendMessageAsync(rsp.buffer, rsp.Length + TrinityProtocol.MsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength);
                        END();
                    })
                .Unwrap()
                .ContinueWith(t => rsp?.Dispose(), TaskContinuationOptions.ExecuteSynchronously);
        }

        public abstract Task t_protocol_name_2HandlerAsync(t_protocol_responseWriter response);
        [END]//METHOD HANDLER

        [IF("$t_protocol->is_asyn_req_rsp_protocol()")]
        #region AsyncWithRsp
        internal static int s_t_protocol_name_token_counter = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe Task _t_protocol_name_CheckErrorAsync(Exception exception, int token, int from)
        {
            if (exception == null) return Task.CompletedTask;
            byte[] rsp = new byte[TrinityProtocol.MsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength];
            GCHandle gch = GCHandle.Alloc(rsp, GCHandleType.Pinned);
            byte* p = (byte*)gch.AddrOfPinnedObject();
            *(int*)(p) = TrinityProtocol.TrinityMsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength;
            *(TrinityMessageType*)(p + TrinityProtocol.MsgTypeOffset) = TrinityMessageType.ASYNC_WITH_RSP;
            *(ushort*)(p + TrinityProtocol.MsgIdOffset) = (ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name__Response;
            *(int*)(p + TrinityProtocol.MsgHeader) = token;
            *(int*)(p + TrinityProtocol.MsgHeader + sizeof(int)) = -1;
            Task task;
            IF("node->type() == PGT_MODULE");
            task = this.SendMessageAsync(m_memorycloud[from], p, rsp.Length);
            ELSE();
            task = Global.CloudStorage[from].SendMessageAsync(p, rsp.Length);
            END();
            return task.ContinueWith(
                t =>
                {
                    gch.Free();
                    ExceptionDispatchInfo.Capture(exception).Throw();
                },
                TaskContinuationOptions.ExecuteSynchronously);
        }

        internal unsafe Task<t_protocol_responseReader> _t_protocol_name_ResponseHandlerAsync(AsynReqRspArgs args)
        {
            byte* buffer = args.Buffer + args.Offset;
            int size = args.Size - TrinityProtocol.AsyncWithRspAdditionalHeaderLength;
            if (size < 0)
            {
                throw new ArgumentException("Async task completion handler encountered negative message size.");
            }
            // the token should be at the frontmost position
            int token = *(int*)buffer;
            int error = *(int*)(buffer + sizeof(int));
            if (error != 0)
            {
                throw new Exception("AsyncWithResponse remote handler failed.");
            }

            byte* buffer_clone = (byte*)Memory.malloc((ulong)(args.Size));
            Memory.Copy(buffer, buffer_clone, args.Size);
            try
            {
                var reader = new t_protocol_responseReader(buffer_clone, TrinityProtocol.AsyncWithRspAdditionalHeaderLength);
                return Task.FromResult(reader);
            }
            catch
            {
                Memory.free(buffer_clone);
                throw;
            }
        }
        #endregion
        [END]

        [END]//FOREACH METHOD

        [MUTE]

        private static MemoryCloud m_memorycloud = null;
        /*MUTE_END*/
    }

    [MODULE_END]
    public unsafe class t_protocol_responseWriter : IDisposable
    {
        internal byte* buffer;
        public t_protocol_responseWriter()
        {
        }

        public t_protocol_responseWriter(int asyncRspHeaderLength)
        {
        }

        public int m_ptr { get; internal set; }
        public int Length { get; internal set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }


    public unsafe class t_protocol_requestReader
    {
        private byte* buffer;
        private int offset;

        public t_protocol_requestReader(byte* buffer, int offset)
        {
            this.buffer=buffer;
            this.offset=offset;
        }
    }


    public unsafe class t_protocol_typeArgs
    {
        public byte* Buffer;
        public int Offset;

        public TrinityMessage Response { get; internal set; }
    }

    /// <summary>
    /// Matches with CommunicationSchemaModule namespace namespace TSL.t_base_class_name.t_server_name
    /// </summary>
    namespace TSL.t_base_class_name.t_comm_name
    {
        public enum t_protocol_typeMessageType : ushort
        {
            t_protocol_name,
            t_protocol_name__Response
        }
    }
}
