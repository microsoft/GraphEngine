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
EnumerateFromFieldModule(
NField* node, ModuleContext* context)
            {
                string* source = new string();
                
bool for_accessor_1 = context->m_arguments[0] == "true";
if (node->is_optional())
{
if (for_accessor_1)
{
source->append(R"::(
            if (this.Contains_)::");
source->append(Codegen::GetString(node->name));
source->append(R"::()
                )::");
}
else
{
source->append(R"::(
            if (this.)::");
source->append(Codegen::GetString(node->name));
source->append(R"::( != null)
                )::");
}
}
source->append(R"::(
            switch (TypeConverter<T>.type_id)
            {
                )::");
for (size_t iterator_1 = 0; iterator_1 < (Trinity::Codegen::TSLExternalParserDataTypeVector)->size();++iterator_1)
{
int enum_depth_1 = node->fieldType->enumerate_depth((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]);
if (enum_depth_1 >= 0)
{
source->append(R"::(
                case  )::");
source->append(Codegen::GetString(iterator_1));
source->append(R"::(:
                    {
                        )::");
if (enum_depth_1 == 0)
{
source->append(R"::(
                        yield return TypeConverter<T>.ConvertFrom_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString(node->fieldType)));
source->append(R"::((this.)::");
source->append(Codegen::GetString(node->name));
source->append(R"::();
                        )::");
}
else
{
source->append(R"::(
                        {
                            )::");
int enum_iter_1 = 0;
NFieldType* enum_type_1 = node->fieldType;
source->append(R"::(
                            var element0 = this.)::");
source->append(Codegen::GetString(node->name));
source->append(R"::(;
                            )::");
for(enum_iter_1 = 1; enum_iter_1 <= enum_depth_1; ++enum_iter_1){
enum_type_1 = enum_type_1->get_container_element_type();
source->append(R"::(
                            foreach (var element)::");
source->append(Codegen::GetString(enum_iter_1));
source->append(R"::( in element)::");
source->append(Codegen::GetString(enum_iter_1 - 1));
source->append(R"::()
                            )::");
}
source->append(R"::(
                            {
                                yield return TypeConverter<T>.ConvertFrom_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString(enum_type_1)));
source->append(R"::((element)::");
source->append(Codegen::GetString(enum_depth_1));
source->append(R"::();
                            }
                        }
                        )::");
}
source->append(R"::(
                    }
                    break;
                )::");
}
}
source->append(R"::(
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
            )::");

                return source;
            }
        }
    }
}
