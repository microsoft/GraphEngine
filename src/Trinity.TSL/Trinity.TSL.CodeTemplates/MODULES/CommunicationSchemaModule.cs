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
using Trinity.TSL;
using Trinity.TSL.Lib;
/*MUTE*/
using t_Namespace.MODULES;
/*MUTE_END*/

namespace t_Namespace
{
    [MODULE_BEGIN]
    [TARGET("NProtocolGroup")]
    [MAP_VAR("t_base_class_name", "context->m_arguments[0]")]
    [MAP_VAR("t_server_name", "node->name")]
    [MAP_LIST("t_protocol", "node->protocolList")]
    [MAP_VAR("t_protocol", "referencedNProtocol")]
    [MAP_VAR("t_protocol_name", "name", MemberOf="t_protocol")]
    [MAP_VAR("t_request_signature", "get_signature_string(tsl->find_struct_or_cell(tsl->find_protocol($$->name)->request_message_struct))", MemberOf="t_protocol")]
    [MAP_VAR("t_response_signature", "get_signature_string(tsl->find_struct_or_cell(tsl->find_protocol($$->name)->response_message_struct))", MemberOf="t_protocol")]
    public class t_server_nameCommunicationSchema : __meta, ICommunicationSchema
    {
        IEnumerable<IProtocolDescriptor> ICommunicationSchema.SynReqProtocolDescriptors
        {
            get
            {
                string request_sig;
                FOREACH();
                IF("$t_protocol->is_syn_req_protocol()");
                {
                    IF("$t_protocol->pt_request == PT_VOID_REQUEST");
                    request_sig = "void";
                    ELSE();
                    request_sig = "t_request_signature";
                    END();

                    yield return new ProtocolDescriptor()
                    {
                        Name = "t_protocol_name",
                        RequestSignature = request_sig,
                        ResponseSignature = "void",
                        Type = Trinity.Network.Messaging.TrinityMessageType.SYNC
                    };
                }
                END();
                END();
                yield break;
            }
        }

        IEnumerable<IProtocolDescriptor> ICommunicationSchema.SynReqRspProtocolDescriptors
        {
            get
            {
                string request_sig, response_sig;
                FOREACH();
                IF("$t_protocol->is_syn_req_rsp_protocol()");
                {
                    IF("$t_protocol->pt_request == PT_VOID_REQUEST");
                    request_sig = "void";
                    ELSE();
                    request_sig = "t_request_signature";
                    END();

                    response_sig = "t_response_signature";

                    yield return new ProtocolDescriptor()
                    {
                        Name = "t_protocol_name",
                        RequestSignature = request_sig,
                        ResponseSignature = response_sig,
                        Type = Trinity.Network.Messaging.TrinityMessageType.SYNC_WITH_RSP
                    };
                }
                END();
                END();
                yield break;
            }
        }

        IEnumerable<IProtocolDescriptor> ICommunicationSchema.AsynReqProtocolDescriptors
        {
            get
            {
                string request_sig;
                FOREACH();
                IF("$t_protocol->is_asyn_req_protocol()");
                {
                    IF("$t_protocol->pt_request == PT_VOID_REQUEST");
                    request_sig = "void";
                    ELSE();
                    request_sig = "t_request_signature";
                    END();

                    yield return new ProtocolDescriptor()
                    {
                        Name = "t_protocol_name",
                        RequestSignature = request_sig,
                        ResponseSignature = "void",
                        Type = Trinity.Network.Messaging.TrinityMessageType.ASYNC
                    };
                }
                END();
                END();
                yield break;
            }
        }

        string ICommunicationSchema.Name
        {
            get { return "t_server_name"; }
        }

        IEnumerable<string> ICommunicationSchema.HttpEndpointNames
        {
            get
            {
                FOREACH();
                IF("$t_protocol->is_http_protocol()");
                yield return "t_protocol_name";
                END();
                END();
                yield break;
            }
        }
    }

    [CommunicationSchema(typeof(t_server_nameCommunicationSchema))]
    public abstract partial class t_server_nameBase : t_base_class_name { }

    namespace TSL.t_base_class_name.t_server_name
    {
        /// <summary>
        /// Specifies the type of a synchronous request (without response, that is, response type is void) message.
        /// </summary>
        public enum SynReqMessageType : ushort
        {
            [FOREACH]
            [IF("$t_protocol->is_syn_req_protocol()")]
            t_protocol_name,
            /*END*/
            /*END*/
        }

        /// <summary>
        /// Specifies the type of a synchronous request (with response) message.
        /// </summary>
        public enum SynReqRspMessageType : ushort
        {
            [FOREACH]
            [IF("$t_protocol->is_syn_req_rsp_protocol()")]
            t_protocol_name,
            /*END*/
            /*END*/
        }

        /// <summary>
        /// Specifies the type of an asynchronous request (without response) message.
        /// </summary>
        /// <remarks>Note that asynchronous message with response is not supported.</remarks>
        public enum AsynReqMessageType : ushort
        {
            [FOREACH]
            [IF("$t_protocol->is_asyn_req_protocol()")]
            t_protocol_name,
            /*END*/
            /*END*/
        }
    }
    /*MODULE_END*/
}
