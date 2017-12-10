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
SerializeParametersToBuffer(
NStructBase* node, ModuleContext* context)
            {
                string* source = new string();
                
bool forcell_1 = (context->m_arguments[0] == "cell");
source->append(R"::(
            byte* targetPtr;
            )::");
if (forcell_1)
{
source->append(R"::(
            targetPtr = null;
            )::");
}
else
{
source->append(R"::(
            targetPtr = (byte*)preservedHeaderLength;
            )::");
}

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = context->m_stack_depth + 1;
module_ctx.m_arguments.push_back(Codegen::GetString("push"));
std::string* module_content = Modules::PushPointerFromParameters(node, &module_ctx);
    source->append(*module_content);
    delete module_content;
}
if (forcell_1)
{
source->append(R"::(
            byte[] tmpcell = new byte[(int)(targetPtr)];
            fixed (byte* _tmpcellptr = tmpcell)
            {
                targetPtr = _tmpcellptr;
                )::");

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = context->m_stack_depth + 1;
module_ctx.m_arguments.push_back(Codegen::GetString("assign"));
std::string* module_content = Modules::PushPointerFromParameters(node, &module_ctx);
    source->append(*module_content);
    delete module_content;
}
source->append(R"::(
            }
            )::");
}
else
{
source->append(R"::(
            byte* tmpcellptr = (byte*)Memory.malloc((ulong)targetPtr);
            {
                BufferLength     = (int)targetPtr;
                Memory.memset(tmpcellptr, 0, (ulong)targetPtr);
                targetPtr = tmpcellptr;
                tmpcellptr += preservedHeaderLength;
                targetPtr  += preservedHeaderLength;
                )::");

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = context->m_stack_depth + 1;
module_ctx.m_arguments.push_back(Codegen::GetString("assign"));
std::string* module_content = Modules::PushPointerFromParameters(node, &module_ctx);
    source->append(*module_content);
    delete module_content;
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
