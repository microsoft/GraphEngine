using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using t_Namespace.MODULES;
using Trinity.Network;
using Trinity.Network.Messaging;
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
    [MAP_VAR("t_protocol_request", "referencedNProtocol->request_message_struct")]
    [MAP_VAR("t_protocol_response", "referencedNProtocol->response_message_struct")]
    [MAP_VAR("t_protocol_type", "get_comm_protocol_type_string($$->referencedNProtocol)")]
    public abstract partial class t_comm_name : t_base_class_name
    {
        protected override void RegisterMessageHandler()
        {
            FOREACH();
            {
                META("if($t_protocol->is_http_protocol()){continue;}");

                IF("node->type() == PGT_MODULE");
                MessageRegistry.RegisterMessageHandler((ushort)(this.t_protocol_typeIdOffset + (ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name), _t_protocol_name_4Handler);
                ELSE();
                MessageRegistry.RegisterMessageHandler((ushort)(ushort)global::t_Namespace.TSL.t_base_class_name.t_comm_name.t_protocol_typeMessageType.t_protocol_name, _t_protocol_name_4Handler);
                END();
            }
            END();
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


        [ELIF("$t_protocol->has_request()")]
        //SYNC_WITH_RSP, request is not void
        private unsafe void _t_protocol_name_2Handler(t_protocol_typeArgs args)
        {
            var rsp = new t_protocol_responseWriter();
            t_protocol_nameHandler(new t_protocol_requestReader(args.Buffer, args.Offset), rsp);
            *(int*)(rsp.CellPtr - TrinityProtocol.MsgHeader) = rsp.Length + TrinityProtocol.TrinityMsgHeader;
            args.Response = new TrinityMessage(rsp.buffer, rsp.Length + TrinityProtocol.MsgHeader);
        }

        public abstract void t_protocol_nameHandler(t_protocol_requestReader request, t_protocol_responseWriter response);
        [ELSE]
        //SYNC_WITH_RSP, request is void
        private unsafe void _t_protocol_name_3Handler(t_protocol_typeArgs args)
        {
            var rsp = new t_protocol_responseWriter();
            t_protocol_nameHandler(rsp);
            *(int*)(rsp.CellPtr - TrinityProtocol.MsgHeader) = rsp.Length + TrinityProtocol.TrinityMsgHeader;
            args.Response = new TrinityMessage(rsp.buffer, rsp.Length + TrinityProtocol.MsgHeader);
        }

        public abstract void t_protocol_nameHandler(t_protocol_responseWriter response);
        [END]
        [END]//FOREACH

        [MUTE]
        void commclass__placeholder() { }
        /*MUTE_END*/
    }

    [MODULE_END]
    public unsafe class t_protocol_responseWriter
    {
        internal byte* buffer;

        public t_protocol_responseWriter()
        {
        }

        public int CellPtr { get; internal set; }
        public int Length { get; internal set; }
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
            t_protocol_name
        }
    }
}
