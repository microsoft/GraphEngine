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
LegacyMessagePassingMethods(
NProtocolGroup* node, ModuleContext* context)
            {
                string* source = new string();
                
std::string method_name_1;
std::string storage_name_1;
std::string arg_extension_method_target_1;
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
storage_name_1 = "storage";
}
else if (node->type() == PGT_PROXY)
{
method_name_1 = *(*(node->protocolList))[iterator_1]->name + "To" + *node->name;
arg_extension_method_target_1 = "this Trinity.Storage.MemoryCloud storage, ";
storage_name_1 = "storage";
}
else
{
method_name_1 = *(*(node->protocolList))[iterator_1]->name;
arg_extension_method_target_1 = "";
storage_name_1 = "m_memorycloud";
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
            )::");
source->append(Codegen::GetString(node->name));
source->append(R"::(.MessagePassingExtension.)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(()::");
source->append(Codegen::GetString(storage_name_1));
source->append(R"::([partitionId]);
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
            )::");
source->append(Codegen::GetString(node->name));
source->append(R"::(.MessagePassingExtension.)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(()::");
source->append(Codegen::GetString(storage_name_1));
source->append(R"::([partitionId], msg);
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
            return )::");
source->append(Codegen::GetString(node->name));
source->append(R"::(.MessagePassingExtension.)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(()::");
source->append(Codegen::GetString(storage_name_1));
source->append(R"::([partitionId]);
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
            return )::");
source->append(Codegen::GetString(node->name));
source->append(R"::(.MessagePassingExtension.)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(()::");
source->append(Codegen::GetString(storage_name_1));
source->append(R"::([partitionId], msg);
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
            return )::");
source->append(Codegen::GetString(node->name));
source->append(R"::(.MessagePassingExtension.)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(()::");
source->append(Codegen::GetString(storage_name_1));
source->append(R"::([partitionId]);
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
            return )::");
source->append(Codegen::GetString(node->name));
source->append(R"::(.MessagePassingExtension.)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(()::");
source->append(Codegen::GetString(storage_name_1));
source->append(R"::([partitionId], msg);
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
