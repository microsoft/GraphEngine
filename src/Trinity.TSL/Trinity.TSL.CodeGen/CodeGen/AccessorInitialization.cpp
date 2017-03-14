#include "common.h"
#include "AccessorType.h"
#include <string>
#include <SyntaxNode.h>

using std::string;
using namespace Trinity::Codegen;

static void _GenerateAccessorFieldInitializationCode(std::string* source, NFieldType* fieldType, std::string accessorName, AccessorType accessorType)
{
    if (!data_type_need_accessor(fieldType)) return;

    source->append(R"::(
        )::").append(accessorName).append(" = new ").append(data_type_get_accessor_name(fieldType));
    if (fieldType->layoutType == LT_FIXED)
    {
        source->append("(null);");
    }
    else
    {
        source->append("(null,");
        switch (accessorType)
        {
        case AT_CELL_FIELD:
            source->append(R"::(
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                })::");
            break;
        case AT_STRUCT_FIELD:
            source->append(R"::(
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.CellPtr = this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                })::");
            break;
        case AT_LIST_ELEMENT:
            source->append(R"::(
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.CellPtr = this.ResizeFunction(this.CellPtr-sizeof(int), ptr_offset + substructure_offset +sizeof(int), delta);
                    *(int*)this.CellPtr += delta;
                    this.CellPtr += sizeof(int);
                    return this.CellPtr + substructure_offset;
                })::");
            break;
        default:
            error(fieldType, "_GenerateACcessorFieldInitializationCode: unrecognized AccessorType");
            break;
        }
        source->append(");");
    }
}


namespace Trinity
{
    namespace Codegen
    {
        namespace Modules
        {
            string* CellFieldAccessorInitialization(NField* node, ModuleContext* context)
            {
                string* source = new string();
                _GenerateAccessorFieldInitializationCode(source, node->fieldType, *node->name + "_Accessor_Field", AT_CELL_FIELD);

                return source;
            }

            string* StructFieldAccessorInitialization(NField* node, ModuleContext* context)
            {
                string* source = new string();
                _GenerateAccessorFieldInitializationCode(source, node->fieldType, *node->name + "_Accessor_Field", AT_STRUCT_FIELD);

                return source;
            }

            string* ListElementAccessorInitialization(NFieldType* node, ModuleContext* context)
            {
                string* source = new string();
                _GenerateAccessorFieldInitializationCode(source, node, "elementAccessor", AT_LIST_ELEMENT);

                return source;
            }
        }
    }
}
