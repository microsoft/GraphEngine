using System.Text;
using System.Collections.Generic;

namespace Trinity.TSL.CodeTemplates
{
    internal partial class Modules
    {
        internal static string 
PushPointer(
NStructBase node, ModuleContext context)
        {
            StringBuilder source = new StringBuilder();
            
for (int iterator_1 = 0; iterator_1 < (node->fieldList).Count;++iterator_1)
{
source.Append(@"
            {
                ");
if ((node->fieldList)[iterator_1].fieldType->layoutType == LT_FIXED)
{
source.Append(@"
                {
                    ");
source.Append(Codegen.GetString(context->m_arguments[0]));
source.Append(@" += ");
source.Append(Codegen.GetString(0));
source.Append(@";
                }
                ");
}
else if ((node->fieldList)[iterator_1].fieldType->fieldType == FT_LIST)
{
source.Append(@"
                {
                    ");
if ((node->fieldList)[iterator_1].fieldType->listElementType->layoutType == LT_FIXED)
{
source.Append(@"
                    {
                        ");
source.Append(Codegen.GetString(context->m_arguments[0]));
source.Append(@" += sizeof(int) + ");
source.Append(Codegen.GetString(0));
source.Append(@" * (*(int*)");
source.Append(Codegen.GetString(context->m_arguments[0]));
source.Append(@");
                    }
                    ");
}
else if ((node->fieldList)[iterator_1].fieldType->listElementType->fieldType == FT_REFERENCE)
{
source.Append(@"
                    {
                        for (int ");
source.Append(Codegen.GetString(std::string("push_iterator_") + GetString(context->m_stack_depth)));
source.Append(@" = 0, ");
source.Append(Codegen.GetString(std::string("push_size_") + GetString(context->m_stack_depth)));
source.Append(@" = *(int*)");
source.Append(Codegen.GetString(context->m_arguments[0]));
source.Append(@"; ");
source.Append(Codegen.GetString(std::string("push_iterator_") + GetString(context->m_stack_depth)));
source.Append(@" < ");
source.Append(Codegen.GetString(std::string("push_size_") + GetString(context->m_stack_depth)));
source.Append(@"; ++");
source.Append(Codegen.GetString(std::string("push_iterator_") + GetString(context->m_stack_depth)));
source.Append(@")
                        {
                            ");

{
    ModuleContext module_ctx = new ModuleContext();
    module_ctx.m_stack_depth = context.m_stack_depth + 1;
module_ctx.m_arguments.Add(Codegen.GetString(context->m_arguments[0]));
string module_content = Modules.PushPointer(tsl->find_struct((node->fieldList)[iterator_1].fieldType->referencedTypeName), module_ctx);
    source.Append(module_content);
}
source.Append(@"
                        }
                    }
                    ");
}
source.Append(@"
                }
                ");
}
else if ((node->fieldList)[iterator_1].fieldType->fieldType == FT_REFERENCE)
{
source.Append(@"
                {
                    ");

{
    ModuleContext module_ctx = new ModuleContext();
    module_ctx.m_stack_depth = context.m_stack_depth + 1;
module_ctx.m_arguments.Add(Codegen.GetString(context->m_arguments[0]));
string module_content = Modules.PushPointer(tsl->find_struct((node->fieldList)[iterator_1].fieldType->referencedTypeName), module_ctx);
    source.Append(module_content);
}
source.Append(@"
                }
                ");
}
source.Append(@"
            }
            ");
}

            return source.ToString();
        }
    }
}
