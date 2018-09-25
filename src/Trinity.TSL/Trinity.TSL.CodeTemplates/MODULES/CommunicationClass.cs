using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
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
                MessageRegistry.RegisterMessageHandler((ushort)(this.t_protocol_typeIdOffset + (ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name), _t_protocol_name_4Handler);
                MessageRegistry.RegisterMessageHandler((ushort)(this.t_protocol_typeIdOffset + (ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name__Response), _t_protocol_name_ResponseHandler);
                ELSE();
                MessageRegistry.RegisterMessageHandler((ushort)(ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name, _t_protocol_name_4Handler);
                MessageRegistry.RegisterMessageHandler((ushort)(ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name__Response, _t_protocol_name_ResponseHandler);
                END();
            }
            ELSE();
            {
                IF("node->type() == PGT_MODULE");
                MessageRegistry.RegisterMessageHandler((ushort)(this.t_protocol_typeIdOffset + (ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name), _t_protocol_name_4Handler);
                ELSE();
                MessageRegistry.RegisterMessageHandler((ushort)(ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name, _t_protocol_name_4Handler);
                END();
            }
            END();//IF
            END();//FOREACH
        }

        [MUTE]
        void _t_protocol_name_4Handler(SynReqArgs args) { }
        [MUTE_END]

        [FOREACH]
        [META("if($t_protocol->is_http_protocol()){continue;}")]

        [IF("!$t_protocol->has_response()")]
        //SYNC and ASYNC, no response;
        private unsafe void _t_protocol_nameHandler(t_protocol_typeArgs args)
        {
            IF("$t_protocol->has_request()");
            t_protocol_nameHandler(new t_protocol_requestReader(args.Buffer, args.Offset));
            ELSE();
            t_protocol_nameHandler();
            END();
        }

        [IF("$t_protocol->has_request()")]
        public abstract void t_protocol_nameHandler(t_protocol_requestReader request);
        [ELSE]
        public abstract void t_protocol_nameHandler();
        [END]

        //NO RESPONSE END
        [ELIF("$t_protocol->has_request() && $t_protocol->is_syn_req_rsp_protocol()")]
        //SYNC_WITH_RSP, request is not void
        private unsafe void _t_protocol_name_2Handler(t_protocol_typeArgs args)
        {
            var rsp = new t_protocol_responseWriter();
            t_protocol_nameHandler(new t_protocol_requestReader(args.Buffer, args.Offset), rsp);
            *(int*)(rsp.m_ptr - TrinityProtocol.MsgHeader) = rsp.Length + TrinityProtocol.TrinityMsgHeader;
            args.Response = new TrinityMessage(rsp.buffer, rsp.Length + TrinityProtocol.MsgHeader);
        }

        public abstract void t_protocol_nameHandler(t_protocol_requestReader request, t_protocol_responseWriter response);
        [ELIF("!$t_protocol->has_request() && $t_protocol->is_syn_req_rsp_protocol()")]
        private unsafe void _t_protocol_name_3Handler(t_protocol_typeArgs args)
        {
            var rsp = new t_protocol_responseWriter();
            t_protocol_nameHandler(rsp);
            *(int*)(rsp.m_ptr - TrinityProtocol.MsgHeader) = rsp.Length + TrinityProtocol.TrinityMsgHeader;
            args.Response = new TrinityMessage(rsp.buffer, rsp.Length + TrinityProtocol.MsgHeader);
        }

        public abstract void t_protocol_nameHandler(t_protocol_responseWriter response);
        [ELIF("$t_protocol->has_request() && $t_protocol->is_asyn_req_rsp_protocol()")]
        //ASYNC_WITH_RSP, request is not void
        private unsafe void _t_protocol_name_4Handler(t_protocol_typeArgs args)
        {
            using (var rsp = new t_protocol_responseWriter(asyncRspHeaderLength: TrinityProtocol.AsyncWithRspAdditionalHeaderLength))
            {
                Exception exception = null;
                var req = new t_protocol_requestReader(args.Buffer, args.Offset + TrinityProtocol.AsyncWithRspAdditionalHeaderLength);
                try { t_protocol_nameHandler(req, rsp); }
                catch (Exception ex) { exception = ex; }
                int token = *(int*)(args.Buffer + args.Offset);
                int from = *(int*)(args.Buffer + args.Offset + sizeof(int));
                _t_protocol_name_CheckError(exception, token, from);
                *(int*)(rsp.buffer) = TrinityProtocol.TrinityMsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength + rsp.Length;
                *(TrinityMessageType*)(rsp.buffer + TrinityProtocol.MsgTypeOffset) = TrinityMessageType.ASYNC_WITH_RSP;
                *(ushort*)(rsp.buffer + TrinityProtocol.MsgIdOffset) = (ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name__Response;
                *(int*)(rsp.m_ptr - TrinityProtocol.AsyncWithRspAdditionalHeaderLength) = token;
                *(int*)(rsp.m_ptr - TrinityProtocol.AsyncWithRspAdditionalHeaderLength + sizeof(int)) = 0;
                IF("node->type() == PGT_MODULE");
                this.SendMessage(m_memorycloud[from], rsp.buffer, rsp.Length + TrinityProtocol.MsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength);
                ELSE();
                Global.CloudStorage[from].SendMessage(rsp.buffer, rsp.Length + TrinityProtocol.MsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength);
                END();
            }
        }

        public abstract void t_protocol_name_2Handler(t_protocol_requestReader request, t_protocol_responseWriter response);
        [ELSE]
        //("!$t_protocol->has_request() && $t_protocol->is_asyn_req_rsp_protocol()")
        private unsafe void _t_protocol_name_5Handler(t_protocol_typeArgs args)
        {
            using (var rsp = new t_protocol_responseWriter(asyncRspHeaderLength: TrinityProtocol.AsyncWithRspAdditionalHeaderLength))
            {
                Exception exception = null;
                try { t_protocol_nameHandler(rsp); }
                catch (Exception ex) { exception = ex; }
                int token = *(int*)(args.Buffer + args.Offset);
                int from = *(int*)(args.Buffer + args.Offset + sizeof(int));
                _t_protocol_name_CheckError(exception, token, from);
                *(int*)(rsp.buffer) = TrinityProtocol.TrinityMsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength + rsp.Length;
                *(TrinityMessageType*)(rsp.buffer + TrinityProtocol.MsgTypeOffset) = TrinityMessageType.ASYNC_WITH_RSP;
                *(ushort*)(rsp.buffer + TrinityProtocol.MsgIdOffset) = (ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name__Response;
                *(int*)(rsp.m_ptr - TrinityProtocol.AsyncWithRspAdditionalHeaderLength) = token;
                *(int*)(rsp.m_ptr - TrinityProtocol.AsyncWithRspAdditionalHeaderLength + sizeof(int)) = 0;
                IF("node->type() == PGT_MODULE");
                this.SendMessage(m_memorycloud[from], rsp.buffer, rsp.Length + TrinityProtocol.MsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength);
                ELSE();
                Global.CloudStorage[from].SendMessage(rsp.buffer, rsp.Length + TrinityProtocol.MsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength);
                END();
            }
        }

        public abstract void t_protocol_name_2Handler(t_protocol_responseWriter response);
        [END]//METHOD HANDLER

        [IF("$t_protocol->is_asyn_req_rsp_protocol()")]
        #region AsyncWithRsp
        internal static int s_t_protocol_name_token_counter = 0;
        internal static ConcurrentDictionary<int, TaskCompletionSource<t_protocol_responseReader>> s_t_protocol_name_token_sources = new ConcurrentDictionary<int, TaskCompletionSource<t_protocol_responseReader>>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void _t_protocol_name_CheckError(Exception exception, int token, int from)
        {
            if (exception == null) return;
            byte[] rsp = new byte[TrinityProtocol.MsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength];
            fixed (byte* p = rsp)
            {
                *(int*)(p) = TrinityProtocol.TrinityMsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength;
                *(TrinityMessageType*)(p + TrinityProtocol.MsgTypeOffset) = TrinityMessageType.ASYNC_WITH_RSP;
                *(ushort*)(p + TrinityProtocol.MsgIdOffset) = (ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name__Response;
                *(int*)(p + TrinityProtocol.MsgHeader) = token;
                *(int*)(p + TrinityProtocol.MsgHeader + sizeof(int)) = -1;
                IF("node->type() == PGT_MODULE");
                this.SendMessage(m_memorycloud[from], p, rsp.Length);
                ELSE();
                Global.CloudStorage[from].SendMessage(p, rsp.Length);
                END();
            }
            ExceptionDispatchInfo.Capture(exception).Throw();
        }

        internal unsafe void _t_protocol_name_ResponseHandler(AsynReqRspArgs args)
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
            if (!s_t_protocol_name_token_sources.TryRemove(token, out var src))
            {
                throw new ArgumentException("Async task completion token not found while processing a AsyncWithResponse message.");
            }
            if (error != 0)
            {
                src.SetException(new Exception("AsyncWithResponse remote handler failed."));
                return;
            }

            byte* buffer_clone = (byte*)Memory.malloc((ulong)(args.Size));
            Memory.Copy(buffer, buffer_clone, args.Size);
            var reader = new t_protocol_responseReader(buffer_clone, TrinityProtocol.AsyncWithRspAdditionalHeaderLength);
            try { src.SetResult(reader); }
            catch { Memory.free(buffer_clone); throw; }
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
