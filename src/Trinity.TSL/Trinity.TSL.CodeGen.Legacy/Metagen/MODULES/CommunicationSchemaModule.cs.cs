using System.Text;
using System.Collections.Generic;

namespace Trinity.TSL.CodeTemplates
{
    internal partial class Modules
    {
        internal static string 
CommunicationSchemaModule(
NProtocolGroup node, ModuleContext context)
        {
            StringBuilder source = new StringBuilder();
            
source.Append(@"
    public class ");
source.Append(Codegen.GetString(node->name));
source.Append(@"CommunicationSchema : ICommunicationSchema
    {
        IEnumerable<IProtocolDescriptor> ICommunicationSchema.SynReqProtocolDescriptors
        {
            get
            {
                string request_sig;
                ");
for (int iterator_1 = 0; iterator_1 < (node->protocolList).Count;++iterator_1)
{
if (tsl->find_protocol((node->protocolList)[iterator_1]->name)->is_syn_req_protocol())
{
source.Append(@"
                {
                    ");
if (tsl->find_protocol((node->protocolList)[iterator_1]->name)->pt_request == PT_VOID_REQUEST)
{
source.Append(@"
                    request_sig = ""void"";
                    ");
}
else
{
source.Append(@"
                    request_sig = """);
source.Append(Codegen.GetString(get_signature_string(tsl->find_struct_or_cell(tsl->find_protocol((node->protocolList)[iterator_1]->name)->request_message_struct))));
source.Append(@""";
                    ");
}
source.Append(@"
                    yield return new ProtocolDescriptor()
                    {
                        Name = """);
source.Append(Codegen.GetString((node->protocolList)[iterator_1].name));
source.Append(@""",
                        RequestSignature = request_sig,
                        ResponseSignature = ""void"",
                        Type = Trinity.Network.Messaging.TrinityMessageType.SYNC
                    };
                }
                ");
}
}
source.Append(@"
                yield break;
            }
        }
        IEnumerable<IProtocolDescriptor> ICommunicationSchema.SynReqRspProtocolDescriptors
        {
            get
            {
                string request_sig, response_sig;
                ");
for (int iterator_1 = 0; iterator_1 < (node->protocolList).Count;++iterator_1)
{
if (tsl->find_protocol((node->protocolList)[iterator_1]->name)->is_syn_req_rsp_protocol())
{
source.Append(@"
                {
                    ");
if (tsl->find_protocol((node->protocolList)[iterator_1]->name)->pt_request == PT_VOID_REQUEST)
{
source.Append(@"
                    request_sig = ""void"";
                    ");
}
else
{
source.Append(@"
                    request_sig = """);
source.Append(Codegen.GetString(get_signature_string(tsl->find_struct_or_cell(tsl->find_protocol((node->protocolList)[iterator_1]->name)->request_message_struct))));
source.Append(@""";
                    ");
}
source.Append(@"
                    response_sig = """);
source.Append(Codegen.GetString(get_signature_string(tsl->find_struct_or_cell(tsl->find_protocol((node->protocolList)[iterator_1]->name)->response_message_struct))));
source.Append(@""";
                    yield return new ProtocolDescriptor()
                    {
                        Name = """);
source.Append(Codegen.GetString((node->protocolList)[iterator_1].name));
source.Append(@""",
                        RequestSignature = request_sig,
                        ResponseSignature = response_sig,
                        Type = Trinity.Network.Messaging.TrinityMessageType.SYNC_WITH_RSP
                    };
                }
                ");
}
}
source.Append(@"
                yield break;
            }
        }
        IEnumerable<IProtocolDescriptor> ICommunicationSchema.AsynReqProtocolDescriptors
        {
            get
            {
                string request_sig;
                ");
for (int iterator_1 = 0; iterator_1 < (node->protocolList).Count;++iterator_1)
{
if (tsl->find_protocol((node->protocolList)[iterator_1]->name)->is_asyn_req_protocol())
{
source.Append(@"
                {
                    ");
if (tsl->find_protocol((node->protocolList)[iterator_1]->name)->pt_request == PT_VOID_REQUEST)
{
source.Append(@"
                    request_sig = ""void"";
                    ");
}
else
{
source.Append(@"
                    request_sig = """);
source.Append(Codegen.GetString(get_signature_string(tsl->find_struct_or_cell(tsl->find_protocol((node->protocolList)[iterator_1]->name)->request_message_struct))));
source.Append(@""";
                    ");
}
source.Append(@"
                    yield return new ProtocolDescriptor()
                    {
                        Name = """);
source.Append(Codegen.GetString((node->protocolList)[iterator_1].name));
source.Append(@""",
                        RequestSignature = request_sig,
                        ResponseSignature = ""void"",
                        Type = Trinity.Network.Messaging.TrinityMessageType.ASYNC
                    };
                }
                ");
}
}
source.Append(@"
                yield break;
            }
        }
        string ICommunicationSchema.Name
        {
            get { return """);
source.Append(Codegen.GetString(node->name));
source.Append(@"""; }
        }
        IEnumerable<string> ICommunicationSchema.HttpEndpointNames
        {
            get
            {
                ");
for (int iterator_1 = 0; iterator_1 < (node->protocolList).Count;++iterator_1)
{
if (tsl->find_protocol((node->protocolList)[iterator_1]->name)->is_http_protocol())
{
source.Append(@"
                yield return """);
source.Append(Codegen.GetString((node->protocolList)[iterator_1].name));
source.Append(@""";
                ");
}
}
source.Append(@"
                yield break;
            }
        }
    }
    [CommunicationSchema(typeof(");
source.Append(Codegen.GetString(node->name));
source.Append(@"CommunicationSchema))]
    public abstract partial class ");
source.Append(Codegen.GetString(node->name));
source.Append(@"Base : ");
source.Append(Codegen.GetString(context->m_arguments[0]));
source.Append(@" { }
    namespace TSL.");
source.Append(Codegen.GetString(context->m_arguments[0]));
source.Append(@".");
source.Append(Codegen.GetString(node->name));
source.Append(@"
    {
        /// <summary>
        /// Specifies the type of a synchronous request (without response, that is, response type is void) message.
        /// </summary>
        public enum SynReqMessageType : ushort
        {
            ");
for (int iterator_1 = 0; iterator_1 < (node->protocolList).Count;++iterator_1)
{
if (tsl->find_protocol((node->protocolList)[iterator_1]->name)->is_syn_req_protocol())
{
source.Append(Codegen.GetString((node->protocolList)[iterator_1].name));
source.Append(@",
            ");
}
}
source.Append(@"
        }
        /// <summary>
        /// Specifies the type of a synchronous request (with response) message.
        /// </summary>
        public enum SynReqRspMessageType : ushort
        {
            ");
for (int iterator_1 = 0; iterator_1 < (node->protocolList).Count;++iterator_1)
{
if (tsl->find_protocol((node->protocolList)[iterator_1]->name)->is_syn_req_rsp_protocol())
{
source.Append(Codegen.GetString((node->protocolList)[iterator_1].name));
source.Append(@",
            ");
}
}
source.Append(@"
        }
        /// <summary>
        /// Specifies the type of an asynchronous request (without response) message.
        /// </summary>
        /// <remarks>Note that asynchronous message with response is not supported.</remarks>
        public enum AsynReqMessageType : ushort
        {
            ");
for (int iterator_1 = 0; iterator_1 < (node->protocolList).Count;++iterator_1)
{
if (tsl->find_protocol((node->protocolList)[iterator_1]->name)->is_asyn_req_protocol())
{
source.Append(Codegen.GetString((node->protocolList)[iterator_1].name));
source.Append(@",
            ");
}
}
source.Append(@"
        }
    }
    ");

            return source.ToString();
        }
    }
}
