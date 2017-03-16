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
AccessorReverseImplicitOperator(
NFieldType* node, ModuleContext* context)
            {
                string* source = new string();
                
bool has_resize_1 = (node->layoutType != LT_FIXED);
bool for_cell_1 = (node->is_struct() && !node->referencedNStruct->is_struct());
source->append(R"::(
        public unsafe static implicit operator )::");
source->append(Codegen::GetString(data_type_get_accessor_name(node)));
source->append(R"::(()::");
source->append(Codegen::GetString(GetNonNullableValueTypeString(node)));
source->append(R"::( field)
        {
            byte* targetPtr = null;
            byte* tmpcellptr = BufferAllocator.AllocBuffer((int)targetPtr);
            Memory.memset(tmpcellptr, 0, (ulong)targetPtr);
            targetPtr = tmpcellptr;
            )::");
source->append(Codegen::GetString(data_type_get_accessor_name(node)));
source->append(R"::( ret;
            )::");
if (has_resize_1)
{
source->append(R"::(
            ret = new )::");
source->append(Codegen::GetString(data_type_get_accessor_name(node)));
source->append(R"::((tmpcellptr, null);
            )::");
}
else
{
source->append(R"::(
            ret = new )::");
source->append(Codegen::GetString(data_type_get_accessor_name(node)));
source->append(R"::((tmpcellptr);
            )::");
}
if (for_cell_1)
{
source->append(R"::(
            ret.CellID = field.CellID;
            )::");
}
else
{
source->append(R"::(
            ret.CellID = null;
            )::");
}
source->append(R"::(
            return ret;
        }
        )::");

                return source;
            }
        }
    }
}
