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
std::string send_message_method_1;
for (size_t iterator_1 = 0; iterator_1 < (node->protocolList)->size();++iterator_1)
{
if (!(*(node->protocolList))[iterator_1]->referencedNProtocol->is_http_protocol())
{
source->append(R"::(
        #region prototype definition template variables
        )::");
if (node->type() == PGT_SERVER)
{
method_name_1 = *(*(node->protocolList))[iterator_1]->name + "To" + *node->name;
arg_extension_method_target_1 = "this Trinity.Storage.MemoryCloud storage, ";
send_message_method_1 = "storage.SendMessageToServer";
}
else if (node->type() == PGT_PROXY)
{
method_name_1 = *(*(node->protocolList))[iterator_1]->name + "To" + *node->name;
arg_extension_method_target_1 = "this Trinity.Storage.MemoryCloud storage, ";
send_message_method_1 = "storage.SendMessageToProxy";
}
else
{
method_name_1 = *(*(node->protocolList))[iterator_1]->name;
arg_extension_method_target_1 = "";
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
source->append(R"::( int partitionId)
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
source->append(R"::((partitionId, bufferPtr, TrinityProtocol.MsgHeader);
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
source->append(R"::( int partitionId, )::");
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
source->append(R"::((partitionId, bufferPtr, msg.Length + TrinityProtocol.MsgHeader);
            }
            finally { }
        }
        )::");
}
else if (!(*(node->protocolList))[iterator_1]->referencedNProtocol->has_request() && (*(node->protocolList))[iterator_1]->referencedNProtocol->is_syn_req_rsp_protocol())
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
source->append(R"::( int partitionId)
        {
            byte* bufferPtr = stackalloc byte[TrinityProtocol.MsgHeader];
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
source->append(R"::((partitionId, bufferPtr, TrinityProtocol.MsgHeader, out response);
                return new )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->response_message_struct));
source->append(R"::(Reader(response.Buffer, response.Offset);
            }
            finally { Memory.free(bufferPtr); }
        }
        )::");
}
else if ((*(node->protocolList))[iterator_1]->referencedNProtocol->has_request() && (*(node->protocolList))[iterator_1]->referencedNProtocol->is_syn_req_rsp_protocol())
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
source->append(R"::( int partitionId, )::");
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
source->append(R"::((partitionId, bufferPtr, msg.Length + TrinityProtocol.MsgHeader, out response);
                return new )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->response_message_struct));
source->append(R"::(Reader(response.Buffer, response.Offset);
            }
            finally { }
        }
        )::");
}
else if (!(*(node->protocolList))[iterator_1]->referencedNProtocol->has_request() && (*(node->protocolList))[iterator_1]->referencedNProtocol->is_asyn_req_rsp_protocol())
{
source->append(R"::(
        public unsafe)::");
if (node->type() != PGT_MODULE)
{
source->append(R"::( static)::");
}
source->append(R"::( Task<)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->response_message_struct));
source->append(R"::(Reader> )::");
source->append(Codegen::GetString(method_name_1));
source->append(R"::(()::");
source->append(Codegen::GetString(arg_extension_method_target_1));
source->append(R"::( int partitionId)
        {
            byte* bufferPtr = stackalloc byte[TrinityProtocol.MsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength];
            try
            {
                int token = Interlocked.Increment(ref )::");
source->append(Codegen::GetString(node->name));
source->append(R"::(Base.s_)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(_token_counter);
                var task_source = new TaskCompletionSource<)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->response_message_struct));
source->append(R"::(Reader>();
                )::");
source->append(Codegen::GetString(node->name));
source->append(R"::(Base.s_)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(_token_sources[token] = task_source;
                *(int*)(bufferPtr + TrinityProtocol.MsgHeader) = token;
                *(int*)(bufferPtr + TrinityProtocol.MsgHeader + sizeof(int)) = Global.CloudStorage.MyInstanceId;
                *(int*)(bufferPtr) = TrinityProtocol.TrinityMsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength;
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
source->append(R"::((partitionId, bufferPtr, TrinityProtocol.MsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength);
                return task_source.Task;
            }
            finally { }
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
source->append(R"::( Task<)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->response_message_struct));
source->append(R"::(Reader> )::");
source->append(Codegen::GetString(method_name_1));
source->append(R"::(()::");
source->append(Codegen::GetString(arg_extension_method_target_1));
source->append(R"::( int partitionId, )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->request_message_struct));
source->append(R"::(Writer msg)
        {
            byte** bufferPtrs = stackalloc byte*[2];
            int*   size       = stackalloc int[2];
            byte*  bufferPtr  = stackalloc byte[TrinityProtocol.MsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength];
            bufferPtrs[0]     = bufferPtr;
            bufferPtrs[1]     = msg.buffer + TrinityProtocol.MsgHeader;
            size[0]           = TrinityProtocol.MsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength;
            size[1]           = msg.Length;
            try
            {
                int token = Interlocked.Increment(ref )::");
source->append(Codegen::GetString(node->name));
source->append(R"::(Base.s_)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(_token_counter);
                var task_source = new TaskCompletionSource<)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->response_message_struct));
source->append(R"::(Reader>();
                )::");
source->append(Codegen::GetString(node->name));
source->append(R"::(Base.s_)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(_token_sources[token] = task_source;
                *(int*)(bufferPtr) = TrinityProtocol.TrinityMsgHeader + msg.Length + TrinityProtocol.AsyncWithRspAdditionalHeaderLength;
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
                *(int*)(bufferPtr + TrinityProtocol.MsgHeader) = token;
                *(int*)(bufferPtr + TrinityProtocol.MsgHeader + sizeof(int)) = Global.CloudStorage.MyInstanceId;
                )::");
source->append(Codegen::GetString(send_message_method_1));
source->append(R"::((partitionId, bufferPtrs, size, 2);
                return task_source.Task;
            }
            finally { }
        }
        )::");
}
}
}

                return source;
            }
        }
    }
}
