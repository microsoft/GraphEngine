#include "common.h"
#include "AccessorType.h"
#include <string>
#include "SyntaxNode.h"

using std::string;
using namespace Trinity::Codegen;

/**
 * Note, these resize lambda expressions do not use the m_ptr field of the accessor itself.
 * So it is fine to not update m_ptr of a accessor field before resizing (or even not update
 * it afterwards, if we are in an accessor field setter and the accessor won't be used).
 * For lists, the resize function will also update the length at (listAccessor.m_ptr - 4).
 */
static void _GenerateAccessorFieldInitializationCode(std::string* source, NFieldType* fieldType, std::string accessorName, AccessorType accessorType)
{
    if (!data_type_need_accessor(fieldType)) return;

    source->append("\
        ").append(accessorName).append(" = new ").append(data_type_get_accessor_name(fieldType));
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
                    int substructure_offset = (int)(ptr - this.m_ptr);
                    this.ResizeFunction(this.m_ptr, ptr_offset + substructure_offset, delta);
                    return this.m_ptr + substructure_offset;
                })::");
            break;
        case AT_STRUCT_FIELD:
            source->append(R"::(
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.m_ptr);
                    this.m_ptr = this.ResizeFunction(this.m_ptr, ptr_offset + substructure_offset, delta);
                    return this.m_ptr + substructure_offset;
                })::");
            break;
        case AT_LIST_ELEMENT:
            source->append(R"::(
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.m_ptr);
                    this.m_ptr = this.ResizeFunction(this.m_ptr-sizeof(int), ptr_offset + substructure_offset +sizeof(int), delta);
                    *(int*)this.m_ptr += delta;
                    this.m_ptr += sizeof(int);
                    return this.m_ptr + substructure_offset;
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
