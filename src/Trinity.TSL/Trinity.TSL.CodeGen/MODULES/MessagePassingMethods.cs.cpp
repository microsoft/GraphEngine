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
MessagePassingMethods(
NProtocolGroup* node, ModuleContext* context)
            {
                string* source = new string();
                
std::string method_name_1;
std::string arg_extension_method_target_1;
std::string comm_id_1;
std::string send_message_method_1;
for (size_t iterator_1 = 0; iterator_1 < (node->protocolList)->size();++iterator_1)
{
source->append(R"::(
        #region prototype definition template variables
        )::");
if (node->type() == PGT_SERVER)
{
method_name_1 = *(*(node->protocolList))[iterator_1]->name + "To" + *node->name;
arg_extension_method_target_1 = "this Trinity.Storage.MemoryCloud storage, ";
comm_id_1 = "serverId";
send_message_method_1 = "storage.SendMessageToServer";
}
else if (node->type() == PGT_PROXY)
{
method_name_1 = *(*(node->protocolList))[iterator_1]->name + "To" + *node->name;
arg_extension_method_target_1 = "this Trinity.Storage.MemoryCloud storage, ";
comm_id_1 = "proxyId";
send_message_method_1 = "storage.SendMessageToProxy";
}
else
{
method_name_1 = *(*(node->protocolList))[iterator_1]->name;
arg_extension_method_target_1 = "";
comm_id_1 = "moduleId";
send_message_method_1 = "this.SendMessage";
}
source->append(R"::(
        #endregion
        )::");
if (!(*(node->protocolList))[iterator_1]->referencedNProtocol->has_request() && !(*(node->protocolList))[iterator_1]->referencedNProtocol->has_response())
{
source->append(R"::(
        public unsafe )::");
if (node->type() != PGT_MODULE)
{
source->append(R"::( static )::");
}
source->append(R"::( void )::");
source->append(Codegen::GetString(method_name_1));
source->append(R"::(()::");
source->append(Codegen::GetString(arg_extension_method_target_1));
source->append(R"::( int )::");
source->append(Codegen::GetString(comm_id_1));
source->append(R"::()
        {
            byte* bufferPtr = (byte*)Memory.malloc((ulong)TrinityProtocol.MsgHeader);
            try
            {
                *(int*)(bufferPtr) = TrinityProtocol.TrinityMsgHeader;
                *(bufferPtr + TrinityProtocol.MsgTypeOffset) = (byte))::");
source->append(Codegen::GetString(get_comm_protocol_trinitymessagetype((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::( ;
                *(ushort*)(bufferPtr + TrinityProtocol.MsgIdOffset) = (ushort)global::)::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.TSL.)::");
source->append(Codegen::GetString(get_comm_class_basename(node)));
source->append(R"::(.)::");
source->append(Codegen::GetString(node->name));
source->append(R"::(.)::");
source->append(Codegen::GetString(get_comm_protocol_type_string((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::(MessageType.)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(;
                )::");
source->append(Codegen::GetString(send_message_method_1));
source->append(R"::(()::");
source->append(Codegen::GetString(comm_id_1));
source->append(R"::(, bufferPtr, TrinityProtocol.MsgHeader);
            }
            finally { Memory.free(bufferPtr); }
        }
        )::");
}
else if ((*(node->protocolList))[iterator_1]->referencedNProtocol->has_request() && !(*(node->protocolList))[iterator_1]->referencedNProtocol->has_response())
{
source->append(R"::(
        public unsafe)::");
if (node->type() != PGT_MODULE)
{
source->append(R"::( static)::");
}
source->append(R"::( void )::");
source->append(Codegen::GetString(method_name_1));
source->append(R"::(()::");
source->append(Codegen::GetString(arg_extension_method_target_1));
source->append(R"::( int )::");
source->append(Codegen::GetString(comm_id_1));
source->append(R"::(, )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->request_message_struct));
source->append(R"::(Writer msg)
        {
            byte* bufferPtr = msg.buffer;
            try
            {
                *(int*)(bufferPtr) = msg.Length + TrinityProtocol.TrinityMsgHeader;
                *(bufferPtr + TrinityProtocol.MsgTypeOffset) = (byte))::");
source->append(Codegen::GetString(get_comm_protocol_trinitymessagetype((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::( ;
                *(ushort*)(bufferPtr + TrinityProtocol.MsgIdOffset) = (ushort)global::)::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.TSL.)::");
source->append(Codegen::GetString(get_comm_class_basename(node)));
source->append(R"::(.)::");
source->append(Codegen::GetString(node->name));
source->append(R"::(.)::");
source->append(Codegen::GetString(get_comm_protocol_type_string((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::(MessageType.)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(;
                )::");
source->append(Codegen::GetString(send_message_method_1));
source->append(R"::(()::");
source->append(Codegen::GetString(comm_id_1));
source->append(R"::(, bufferPtr, msg.Length + TrinityProtocol.MsgHeader);
            }
            finally { }
        }
        )::");
}
else if (!(*(node->protocolList))[iterator_1]->referencedNProtocol->has_request() && (*(node->protocolList))[iterator_1]->referencedNProtocol->has_response())
{
source->append(R"::(
        public unsafe)::");
if (node->type() != PGT_MODULE)
{
source->append(R"::( static)::");
}
source->append(R"::( )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->response_message_struct));
source->append(R"::(Reader )::");
source->append(Codegen::GetString(method_name_1));
source->append(R"::(()::");
source->append(Codegen::GetString(arg_extension_method_target_1));
source->append(R"::( int )::");
source->append(Codegen::GetString(comm_id_1));
source->append(R"::()
        {
            byte* bufferPtr = (byte*)Memory.malloc((ulong)TrinityProtocol.MsgHeader);
            try
            {
                *(int*)(bufferPtr) = TrinityProtocol.TrinityMsgHeader;
                *(bufferPtr + TrinityProtocol.MsgTypeOffset) = (byte))::");
source->append(Codegen::GetString(get_comm_protocol_trinitymessagetype((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::( ;
                *(ushort*)(bufferPtr + TrinityProtocol.MsgIdOffset) = (ushort)global::)::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.TSL.)::");
source->append(Codegen::GetString(get_comm_class_basename(node)));
source->append(R"::(.)::");
source->append(Codegen::GetString(node->name));
source->append(R"::(.)::");
source->append(Codegen::GetString(get_comm_protocol_type_string((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::(MessageType.)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(;
                TrinityResponse response;
                )::");
source->append(Codegen::GetString(send_message_method_1));
source->append(R"::(()::");
source->append(Codegen::GetString(comm_id_1));
source->append(R"::(, bufferPtr, TrinityProtocol.MsgHeader, out response);
                return new )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->response_message_struct));
source->append(R"::(Reader(response.Buffer, response.Offset);
            }
            finally { Memory.free(bufferPtr); }
        }
        )::");
}
else
{
source->append(R"::(
        public unsafe)::");
if (node->type() != PGT_MODULE)
{
source->append(R"::( static)::");
}
source->append(R"::( )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->response_message_struct));
source->append(R"::(Reader )::");
source->append(Codegen::GetString(method_name_1));
source->append(R"::(()::");
source->append(Codegen::GetString(arg_extension_method_target_1));
source->append(R"::( int )::");
source->append(Codegen::GetString(comm_id_1));
source->append(R"::(, )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->request_message_struct));
source->append(R"::(Writer msg)
        {
            byte* bufferPtr = msg.buffer;
            try
            {
                *(int*)(bufferPtr) = msg.Length + TrinityProtocol.TrinityMsgHeader;
                *(bufferPtr + TrinityProtocol.MsgTypeOffset) = (byte))::");
source->append(Codegen::GetString(get_comm_protocol_trinitymessagetype((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::( ;
                *(ushort*)(bufferPtr + TrinityProtocol.MsgIdOffset) = (ushort)global::)::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.TSL.)::");
source->append(Codegen::GetString(get_comm_class_basename(node)));
source->append(R"::(.)::");
source->append(Codegen::GetString(node->name));
source->append(R"::(.)::");
source->append(Codegen::GetString(get_comm_protocol_type_string((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::(MessageType.)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(;
                TrinityResponse response;
                )::");
source->append(Codegen::GetString(send_message_method_1));
source->append(R"::(()::");
source->append(Codegen::GetString(comm_id_1));
source->append(R"::(, bufferPtr, msg.Length + TrinityProtocol.MsgHeader, out response);
                return new )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->response_message_struct));
source->append(R"::(Reader(response.Buffer, response.Offset);
            }
            finally { }
        }
        )::");
}
}

                return source;
            }
        }
    }
}
