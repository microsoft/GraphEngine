#include "common.h"
#include <string>
#include "SyntaxNode.h"

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        namespace Modules
        {
            string* 
CommunicationSchemaModule(
NProtocolGroup* node, ModuleContext* context)
            {
                string* source = new string();
                
source->append(R"::(
    public class )::");
source->append(Codegen::GetString(node->name));
source->append(R"::(CommunicationSchema : ICommunicationSchema
    {
        IEnumerable<IProtocolDescriptor> ICommunicationSchema.SynReqProtocolDescriptors
        {
            get
            {
                string request_sig;
                )::");
for (size_t iterator_1 = 0; iterator_1 < (node->protocolList)->size();++iterator_1)
{
if ((*(node->protocolList))[iterator_1]->referencedNProtocol->is_syn_req_protocol())
{
source->append(R"::(
                {
                    )::");
if ((*(node->protocolList))[iterator_1]->referencedNProtocol->pt_request == PT_VOID_REQUEST)
{
source->append(R"::(
                    request_sig = "void";
                    )::");
}
else
{
source->append(R"::(
                    request_sig = ")::");
source->append(Codegen::GetString(get_signature_string(tsl->find_struct_or_cell(tsl->find_protocol((*(node->protocolList))[iterator_1]->name)->request_message_struct))));
source->append(R"::(";
                    )::");
}
source->append(R"::(
                    yield return new ProtocolDescriptor()
                    {
                        Name = ")::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(",
                        RequestSignature = request_sig,
                        ResponseSignature = "void",
                        Type = Trinity.Network.Messaging.TrinityMessageType.SYNC
                    };
                }
                )::");
}
}
source->append(R"::(
                yield break;
            }
        }
        IEnumerable<IProtocolDescriptor> ICommunicationSchema.SynReqRspProtocolDescriptors
        {
            get
            {
                string request_sig, response_sig;
                )::");
for (size_t iterator_1 = 0; iterator_1 < (node->protocolList)->size();++iterator_1)
{
if ((*(node->protocolList))[iterator_1]->referencedNProtocol->is_syn_req_rsp_protocol())
{
source->append(R"::(
                {
                    )::");
if ((*(node->protocolList))[iterator_1]->referencedNProtocol->pt_request == PT_VOID_REQUEST)
{
source->append(R"::(
                    request_sig = "void";
                    )::");
}
else
{
source->append(R"::(
                    request_sig = ")::");
source->append(Codegen::GetString(get_signature_string(tsl->find_struct_or_cell(tsl->find_protocol((*(node->protocolList))[iterator_1]->name)->request_message_struct))));
source->append(R"::(";
                    )::");
}
source->append(R"::(
                    response_sig = ")::");
source->append(Codegen::GetString(get_signature_string(tsl->find_struct_or_cell(tsl->find_protocol((*(node->protocolList))[iterator_1]->name)->response_message_struct))));
source->append(R"::(";
                    yield return new ProtocolDescriptor()
                    {
                        Name = ")::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(",
                        RequestSignature = request_sig,
                        ResponseSignature = response_sig,
                        Type = Trinity.Network.Messaging.TrinityMessageType.SYNC_WITH_RSP
                    };
                }
                )::");
}
}
source->append(R"::(
                yield break;
            }
        }
        IEnumerable<IProtocolDescriptor> ICommunicationSchema.AsynReqProtocolDescriptors
        {
            get
            {
                string request_sig;
                )::");
for (size_t iterator_1 = 0; iterator_1 < (node->protocolList)->size();++iterator_1)
{
if ((*(node->protocolList))[iterator_1]->referencedNProtocol->is_asyn_req_protocol())
{
source->append(R"::(
                {
                    )::");
if ((*(node->protocolList))[iterator_1]->referencedNProtocol->pt_request == PT_VOID_REQUEST)
{
source->append(R"::(
                    request_sig = "void";
                    )::");
}
else
{
source->append(R"::(
                    request_sig = ")::");
source->append(Codegen::GetString(get_signature_string(tsl->find_struct_or_cell(tsl->find_protocol((*(node->protocolList))[iterator_1]->name)->request_message_struct))));
source->append(R"::(";
                    )::");
}
source->append(R"::(
                    yield return new ProtocolDescriptor()
                    {
                        Name = ")::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(",
                        RequestSignature = request_sig,
                        ResponseSignature = "void",
                        Type = Trinity.Network.Messaging.TrinityMessageType.ASYNC
                    };
                }
                )::");
}
}
source->append(R"::(
                yield break;
            }
        }
        IEnumerable<IProtocolDescriptor> ICommunicationSchema.AsynReqRspProtocolDescriptors
        {
            get
            {
                string request_sig;
                string response_sig;
                )::");
for (size_t iterator_1 = 0; iterator_1 < (node->protocolList)->size();++iterator_1)
{
if ((*(node->protocolList))[iterator_1]->referencedNProtocol->is_asyn_req_rsp_protocol())
{
source->append(R"::(
                {
                    )::");
if ((*(node->protocolList))[iterator_1]->referencedNProtocol->pt_request == PT_VOID_REQUEST)
{
source->append(R"::(
                    request_sig = "void";
                    )::");
}
else
{
source->append(R"::(
                    request_sig = ")::");
source->append(Codegen::GetString(get_signature_string(tsl->find_struct_or_cell(tsl->find_protocol((*(node->protocolList))[iterator_1]->name)->request_message_struct))));
source->append(R"::(";
                    )::");
}
source->append(R"::(
                    response_sig = ")::");
source->append(Codegen::GetString(get_signature_string(tsl->find_struct_or_cell(tsl->find_protocol((*(node->protocolList))[iterator_1]->name)->response_message_struct))));
source->append(R"::(";
                    yield return new ProtocolDescriptor()
                    {
                        Name = ")::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(",
                        RequestSignature = request_sig,
                        ResponseSignature = response_sig,
                        Type = Trinity.Network.Messaging.TrinityMessageType.ASYNC_WITH_RSP
                    };
                }
                )::");
}
}
source->append(R"::(
                yield break;
            }
        }
        string ICommunicationSchema.Name
        {
            get { return ")::");
source->append(Codegen::GetString(node->name));
source->append(R"::("; }
        }
        IEnumerable<string> ICommunicationSchema.HttpEndpointNames
        {
            get
            {
                )::");
for (size_t iterator_1 = 0; iterator_1 < (node->protocolList)->size();++iterator_1)
{
if ((*(node->protocolList))[iterator_1]->referencedNProtocol->is_http_protocol())
{
source->append(R"::(
                yield return ")::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(";
                )::");
}
}
source->append(R"::(
                yield break;
            }
        }
    }
    [CommunicationSchema(typeof()::");
source->append(Codegen::GetString(node->name));
source->append(R"::(CommunicationSchema))]
    public abstract partial class )::");
source->append(Codegen::GetString(node->name));
source->append(R"::(Base : )::");
source->append(Codegen::GetString(context->m_arguments[0]));
source->append(R"::( { }
    namespace TSL.)::");
source->append(Codegen::GetString(context->m_arguments[0]));
source->append(R"::(.)::");
source->append(Codegen::GetString(node->name));
source->append(R"::(
    {
        /// <summary>
        /// Specifies the type of a synchronous request (without response, that is, response type is void) message.
        /// </summary>
        public enum SynReqMessageType : ushort
        {
            )::");
for (size_t iterator_1 = 0; iterator_1 < (node->protocolList)->size();++iterator_1)
{
if ((*(node->protocolList))[iterator_1]->referencedNProtocol->is_syn_req_protocol())
{
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(,
            )::");
}
}
source->append(R"::(
        }
        /// <summary>
        /// Specifies the type of a synchronous request (with response) message.
        /// </summary>
        public enum SynReqRspMessageType : ushort
        {
            )::");
for (size_t iterator_1 = 0; iterator_1 < (node->protocolList)->size();++iterator_1)
{
if ((*(node->protocolList))[iterator_1]->referencedNProtocol->is_syn_req_rsp_protocol())
{
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(,
            )::");
}
}
source->append(R"::(
        }
        /// <summary>
        /// Specifies the type of an asynchronous request (without response) message.
        /// </summary>
        public enum AsynReqMessageType : ushort
        {
            )::");
for (size_t iterator_1 = 0; iterator_1 < (node->protocolList)->size();++iterator_1)
{
if ((*(node->protocolList))[iterator_1]->referencedNProtocol->is_asyn_req_protocol())
{
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(,
            )::");
}
}
source->append(R"::(
        }
        /// <summary>
        /// Specifies the type of an asynchronous request (with response) message.
        /// </summary>
        public enum AsynReqRspMessageType : ushort
        {
            )::");
for (size_t iterator_1 = 0; iterator_1 < (node->protocolList)->size();++iterator_1)
{
if ((*(node->protocolList))[iterator_1]->referencedNProtocol->is_asyn_req_rsp_protocol())
{
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(,
            )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(__Response,
            )::");
}
}
source->append(R"::(
        }
    }
    )::");

                return source;
            }
        }
    }
}
