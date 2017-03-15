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
ValueToAccessorFieldAssignment(
NField* node, ModuleContext* context)
            {
                string* source = new string();
                
if (node->fieldType->is_nullable())
{
source->append(R"::(
            {
                if ()::");
source->append(Codegen::GetString(context->m_arguments[1]));
source->append(R"::(.HasValue)
                    )::");
source->append(Codegen::GetString(context->m_arguments[0]));
source->append(R"::(.)::");
source->append(Codegen::GetString(node->name));
source->append(R"::( = )::");
source->append(Codegen::GetString(context->m_arguments[1]));
source->append(R"::(.Value;
                else
                    )::");
source->append(Codegen::GetString(context->m_arguments[0]));
source->append(R"::(.Remove_)::");
source->append(Codegen::GetString(node->name));
source->append(R"::(();
            }
            )::");
}
else if (node->fieldType->is_optional())
{
source->append(R"::(
            {
                if ()::");
source->append(Codegen::GetString(context->m_arguments[1]));
source->append(R"::( != default()::");
source->append(Codegen::GetString(node->fieldType));
source->append(R"::())
                    )::");
source->append(Codegen::GetString(context->m_arguments[0]));
source->append(R"::(.)::");
source->append(Codegen::GetString(node->name));
source->append(R"::( = )::");
source->append(Codegen::GetString(context->m_arguments[1]));
source->append(R"::(;
                else
                    )::");
source->append(Codegen::GetString(context->m_arguments[0]));
source->append(R"::(.Remove_)::");
source->append(Codegen::GetString(node->name));
source->append(R"::(();
            }
            )::");
}
else
{
source->append(R"::(
            {
                )::");
source->append(Codegen::GetString(context->m_arguments[0]));
source->append(R"::(.)::");
source->append(Codegen::GetString(node->name));
source->append(R"::( = )::");
source->append(Codegen::GetString(context->m_arguments[1]));
source->append(R"::(;
            }
            )::");
}

                return source;
            }
        }
    }
}
