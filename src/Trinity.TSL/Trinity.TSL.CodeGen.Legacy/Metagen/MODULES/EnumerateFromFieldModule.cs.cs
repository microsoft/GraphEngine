using System.Text;
using System.Collections.Generic;

namespace Trinity.TSL.CodeTemplates
{
    internal partial class Modules
    {
        internal static string 
EnumerateFromFieldModule(
NField node, ModuleContext context)
        {
            StringBuilder source = new StringBuilder();
            
bool for_accessor_1 = context->m_arguments[0] == "true";
if (node->is_optional())
{
if (for_accessor_1)
{
source.Append(@"
            if (this.Contains_");
source.Append(Codegen.GetString(node->name));
source.Append(@")
                ");
}
else
{
source.Append(@"
            if (this.");
source.Append(Codegen.GetString(node->name));
source.Append(@" != null)
                ");
}
}
source.Append(@"
            switch (TypeConverter<T>.type_id)
            {
                ");
for (int iterator_1 = 0; iterator_1 < (Trinity::Codegen::TSLExternalParserDataTypeVector).Count;++iterator_1)
{
int enum_depth_1 = node->fieldType->enumerate_depth((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]);
if (enum_depth_1 >= 0)
{
source.Append(@"
                case  ");
source.Append(Codegen.GetString(iterator_1));
source.Append(@":
                    {
                        ");
if (enum_depth_1 == 0)
{
source.Append(@"
                        yield return TypeConverter<T>.ConvertFrom_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString(node->fieldType)));
source.Append(@"(this.");
source.Append(Codegen.GetString(node->name));
source.Append(@");
                        ");
}
else
{
source.Append(@"
                        {
                            ");
int enum_iter_1 = 0;
NFieldType* enum_type_1 = node->fieldType;
source.Append(@"
                            var element0 = this.");
source.Append(Codegen.GetString(node->name));
source.Append(@";
                            ");
for(enum_iter_1 = 1; enum_iter_1 <= enum_depth_1; ++enum_iter_1){
enum_type_1 = enum_type_1->get_container_element_type();
source.Append(@"
                            foreach (var element");
source.Append(Codegen.GetString(enum_iter_1));
source.Append(@" in element");
source.Append(Codegen.GetString(enum_iter_1 - 1));
source.Append(@")
                            ");
}
source.Append(@"
                            {
                                yield return TypeConverter<T>.ConvertFrom_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString(enum_type_1)));
source.Append(@"(element");
source.Append(Codegen.GetString(enum_depth_1));
source.Append(@");
                            }
                        }
                        ");
}
source.Append(@"
                    }
                    break;
                ");
}
}
source.Append(@"
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            ");

            return source.ToString();
        }
    }
}
