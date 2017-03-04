#include "common.h"
#include <string>
#include <SyntaxNode.h>

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
                
NProtocol* protocol_1;
std::string method_name_1;
for (size_t iterator_1 = 0; iterator_1 < (node->protocolList)->size();++iterator_1)
{
protocol_1 = tsl->find_protocol((*(node->protocolList))[iterator_1]->name);
method_name_1 = (*(node->protocolList))[iterator_1]->name + "To" + node->name
source->append(R"::(
        public unsafe static t_return_type)::");
source->append(Codegen::GetString(method_name_1));
source->append(R"::((this Trinity.Storage.MemoryCloud storage, int )::");
source->append(Codegen::GetString(node->name));
source->append(R"::(Id)
        {
        }
        )::");
}

                return source;
            }
        }
    }
}
