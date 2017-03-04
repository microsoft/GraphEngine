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
PushPointer(
NStructBase* node, ModuleContext* context)
            {
                string* source = new string();
                
for (size_t iterator_1 = 0; iterator_1 < (node->fieldList)->size();++iterator_1)
{
source->append(R"::(
            {
                )::");
if ((*(node->fieldList))[iterator_1]->fieldType->layoutType == LT_FIXED)
{
source->append(R"::(
                {
                    )::");
source->append(Codegen::GetString(context->m_arguments[0]));
source->append(R"::( += )::");
source->append(Codegen::GetString(0));
source->append(R"::(;
                }
                )::");
}
else if ((*(node->fieldList))[iterator_1]->fieldType->fieldType == FT_LIST)
{
source->append(R"::(
                {
                    )::");
if ((*(node->fieldList))[iterator_1]->fieldType->listElementType->layoutType == LT_FIXED)
{
source->append(R"::(
                    {
                        )::");
source->append(Codegen::GetString(context->m_arguments[0]));
source->append(R"::( += sizeof(int) + )::");
source->append(Codegen::GetString(0));
source->append(R"::( * (*(int*))::");
source->append(Codegen::GetString(context->m_arguments[0]));
source->append(R"::();
                    }
                    )::");
}
else if ((*(node->fieldList))[iterator_1]->fieldType->listElementType->fieldType == FT_REFERENCE)
{
source->append(R"::(
                    {
                        for (int )::");
source->append(Codegen::GetString(std::string("push_iterator_") + GetString(context->m_stack_depth)));
source->append(R"::( = 0, )::");
source->append(Codegen::GetString(std::string("push_size_") + GetString(context->m_stack_depth)));
source->append(R"::( = *(int*))::");
source->append(Codegen::GetString(context->m_arguments[0]));
source->append(R"::(; )::");
source->append(Codegen::GetString(std::string("push_iterator_") + GetString(context->m_stack_depth)));
source->append(R"::( < )::");
source->append(Codegen::GetString(std::string("push_size_") + GetString(context->m_stack_depth)));
source->append(R"::(; ++)::");
source->append(Codegen::GetString(std::string("push_iterator_") + GetString(context->m_stack_depth)));
source->append(R"::()
                        {
                            )::");

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = context->m_stack_depth + 1;
module_ctx.m_arguments.push_back(Codegen::GetString(context->m_arguments[0]));
std::string* module_content = Modules::PushPointer(tsl->find_struct((*(node->fieldList))[iterator_1]->fieldType->referencedTypeName), &module_ctx);
    source->append(*module_content);
    delete module_content;
}
source->append(R"::(
                        }
                    }
                    )::");
}
source->append(R"::(
                }
                )::");
}
else if ((*(node->fieldList))[iterator_1]->fieldType->fieldType == FT_REFERENCE)
{
source->append(R"::(
                {
                    )::");

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = context->m_stack_depth + 1;
module_ctx.m_arguments.push_back(Codegen::GetString(context->m_arguments[0]));
std::string* module_content = Modules::PushPointer(tsl->find_struct((*(node->fieldList))[iterator_1]->fieldType->referencedTypeName), &module_ctx);
    source->append(*module_content);
    delete module_content;
}
source->append(R"::(
                }
                )::");
}
source->append(R"::(
            }
            )::");
}

                return source;
            }
        }
    }
}
