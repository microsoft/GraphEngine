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
CommunicationClass(
NProtocolGroup* node, ModuleContext* context)
            {
                string* source = new string();
                
source->append(R"::(
    public abstract partial class )::");
source->append(Codegen::GetString(node->name));
source->append(R"::(Base : )::");
source->append(Codegen::GetString(get_comm_class_basename(node)));
source->append(R"::(
    {
        protected override void RegisterMessageHandler()
        {
            )::");
for (size_t iterator_1 = 0; iterator_1 < (node->protocolList)->size();++iterator_1)
{
source->append(R"::(
            {
                )::");
if((*(node->protocolList))[iterator_1]->referencedNProtocol->is_http_protocol()){continue;}
if (node->type() == PGT_MODULE)
{
source->append(R"::(
                MessageRegistry.RegisterMessageHandler((ushort)(this.)::");
source->append(Codegen::GetString(get_comm_protocol_type_string((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::(IdOffset + (ushort)global::)::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.TSL.)::");
source->append(Codegen::GetString(get_comm_class_basename(node)));
source->append(R"::(.)::");
source->append(Codegen::GetString(node->name));
source->append(R"::(.)::");
source->append(Codegen::GetString(get_comm_protocol_type_string((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::(MessageType.)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(), _)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler);
                )::");
}
else
{
source->append(R"::(
                MessageRegistry.RegisterMessageHandler((ushort)(ushort)global::)::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.TSL.)::");
source->append(Codegen::GetString(get_comm_class_basename(node)));
source->append(R"::(.)::");
source->append(Codegen::GetString(node->name));
source->append(R"::(.)::");
source->append(Codegen::GetString(get_comm_protocol_type_string((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::(MessageType.)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(, _)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler);
                )::");
}
source->append(R"::(
            }
            )::");
}
source->append(R"::(
        }
        )::");
for (size_t iterator_1 = 0; iterator_1 < (node->protocolList)->size();++iterator_1)
{
if((*(node->protocolList))[iterator_1]->referencedNProtocol->is_http_protocol()){continue;}
if (!(*(node->protocolList))[iterator_1]->referencedNProtocol->has_response())
{
source->append(R"::(
        private unsafe void _)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler()::");
source->append(Codegen::GetString(get_comm_protocol_type_string((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::(Args args)
        {
            )::");
if ((*(node->protocolList))[iterator_1]->referencedNProtocol->has_request())
{
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler(new )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->request_message_struct));
source->append(R"::(Reader(args.Buffer, args.Offset));
            )::");
}
else
{
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler();
            )::");
}
source->append(R"::(
        }
        )::");
if ((*(node->protocolList))[iterator_1]->referencedNProtocol->has_request())
{
source->append(R"::(
        public abstract void )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler()::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->request_message_struct));
source->append(R"::(Reader request);
        )::");
}
else
{
source->append(R"::(
        public abstract void )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler();
        )::");
}
}
else if ((*(node->protocolList))[iterator_1]->referencedNProtocol->has_request())
{
source->append(R"::(
        private unsafe void _)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler()::");
source->append(Codegen::GetString(get_comm_protocol_type_string((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::(Args args)
        {
            var rsp = new )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->response_message_struct));
source->append(R"::(Writer();
            )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler(new )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->request_message_struct));
source->append(R"::(Reader(args.Buffer, args.Offset), rsp);
            *(int*)(rsp.CellPtr - TrinityProtocol.MsgHeader) = rsp.Length + TrinityProtocol.TrinityMsgHeader;
            args.Response = new TrinityMessage(rsp.buffer, rsp.Length + TrinityProtocol.MsgHeader);
        }
        public abstract void )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler()::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->request_message_struct));
source->append(R"::(Reader request, )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->response_message_struct));
source->append(R"::(Writer response);
        )::");
}
else
{
source->append(R"::(
        private unsafe void _)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler()::");
source->append(Codegen::GetString(get_comm_protocol_type_string((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::(Args args)
        {
            var rsp = new )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->response_message_struct));
source->append(R"::(Writer();
            )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler(rsp);
            *(int*)(rsp.CellPtr - TrinityProtocol.MsgHeader) = rsp.Length + TrinityProtocol.TrinityMsgHeader;
            args.Response = new TrinityMessage(rsp.buffer, rsp.Length + TrinityProtocol.MsgHeader);
        }
        public abstract void )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler()::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->response_message_struct));
source->append(R"::(Writer response);
        )::");
}
}
source->append(R"::(
    }
    )::");

                return source;
            }
        }
    }
}
