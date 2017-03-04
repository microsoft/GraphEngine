using System.Text;
using System.Collections.Generic;

namespace Trinity.TSL.CodeTemplates
{
    internal partial class Modules
    {
        internal static string 
AccessorFieldAssignment(
NField node, ModuleContext context)
        {
            StringBuilder source = new StringBuilder();
            
if (node->fieldType->is_nullable())
{
source.Append(@"
            {
                if (");
source.Append(Codegen.GetString(context->m_arguments[1]));
source.Append(@".HasValue)
                    ");
source.Append(Codegen.GetString(context->m_arguments[0]));
source.Append(@".");
source.Append(Codegen.GetString(node->name));
source.Append(@" = ");
source.Append(Codegen.GetString(context->m_arguments[1]));
source.Append(@".Value;
                else
                    ");
source.Append(Codegen.GetString(context->m_arguments[0]));
source.Append(@".Remove_");
source.Append(Codegen.GetString(node->name));
source.Append(@"();
            }
            ");
}
else if (node->fieldType->is_optional())
{
source.Append(@"
            {
                if (");
source.Append(Codegen.GetString(context->m_arguments[1]));
source.Append(@" != default(");
source.Append(Codegen.GetString(node->fieldType));
source.Append(@"))
                    ");
source.Append(Codegen.GetString(context->m_arguments[0]));
source.Append(@".");
source.Append(Codegen.GetString(node->name));
source.Append(@" = ");
source.Append(Codegen.GetString(context->m_arguments[1]));
source.Append(@";
                else
                    ");
source.Append(Codegen.GetString(context->m_arguments[0]));
source.Append(@".Remove_");
source.Append(Codegen.GetString(node->name));
source.Append(@"();
            }
            ");
}
else
{
source.Append(@"
            {
                ");
source.Append(Codegen.GetString(context->m_arguments[0]));
source.Append(@".");
source.Append(Codegen.GetString(node->name));
source.Append(@" = ");
source.Append(Codegen.GetString(context->m_arguments[1]));
source.Append(@";
            }
            ");
}

            return source.ToString();
        }
    }
}
