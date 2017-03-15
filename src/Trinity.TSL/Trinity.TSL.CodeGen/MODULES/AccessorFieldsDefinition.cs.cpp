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
AccessorFieldsDefinition(
NStructBase* node, ModuleContext* context)
            {
                string* source = new string();
                
for (size_t iterator_1 = 0; iterator_1 < (node->fieldList)->size();++iterator_1)
{
bool field_fixed_1 = ((*(node->fieldList))[iterator_1]->fieldType->layoutType == LT_FIXED);
bool field_optional_1 = ((*(node->fieldList))[iterator_1]->is_optional());
bool field_need_accessor_1 = (data_type_need_accessor((*(node->fieldList))[iterator_1]->fieldType));
bool field_lenprefix_1 = (data_type_is_length_prefixed((*(node->fieldList))[iterator_1]->fieldType));
OptionalFieldCalculator optcalc_1 = OptionalFieldCalculator(node);
std::string accessor_field_name_1 = (*(*(node->fieldList))[iterator_1]->name) + "_Accessor_Field";
if (field_need_accessor_1)
{
source->append(Codegen::GetString(data_type_get_accessor_name((*(node->fieldList))[iterator_1]->fieldType)));
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::(_Accessor_Field;
        )::");
}
if (field_optional_1)
{
source->append(R"::(
        ///<summary>
        ///Represents the presence of the optional field )::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::(.
        ///</summary>
        public bool Contains_)::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::(
        {
            get
            {
                unchecked
                {
                    return )::");
source->append(Codegen::GetString(optcalc_1.GenerateReadBitExpression((*(node->fieldList))[iterator_1])));
source->append(R"::( ;
                }
            }
            internal set
            {
                unchecked
                {
                    if (value)
                    {
                        )::");
source->append(Codegen::GetString(optcalc_1.GenerateMaskOnCode((*(node->fieldList))[iterator_1])));
source->append(R"::(
                    }
                    else
                    {
                        )::");
source->append(Codegen::GetString(optcalc_1.GenerateMaskOffCode((*(node->fieldList))[iterator_1])));
source->append(R"::(
                    }
                }
            }
        }
        ///<summary>
        ///Removes the optional field )::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::( from the object being operated.
        ///</summary>
        public unsafe void Remove_)::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::(()
        {
            if (!this.Contains_)::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::()
            {
                throw new Exception("Optional field )::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::( doesn't exist for current cell.");
            }
            this.Contains_)::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::( = false;
        }
        )::");
}
source->append(R"::(
        ///<summary>
        ///Provides in-place access to the object field )::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::(.
        ///</summary>
        public unsafe )::");
source->append(Codegen::GetString(data_type_get_accessor_name((*(node->fieldList))[iterator_1]->fieldType)));
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::(
        {
            get
            {
                )::");
if (field_optional_1)
{
source->append(R"::(
                if (!this.Contains_)::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::()
                {
                    throw new Exception("Optional field )::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::( doesn't exist for current cell.");
                }
                )::");
}
source->append(R"::(
                byte* targetPtr = CellPtr;
                )::");
if (!field_need_accessor_1)
{
source->append(R"::(
                return *()::");
source->append(Codegen::GetString(data_type_get_accessor_name((*(node->fieldList))[iterator_1]->fieldType)));
source->append(R"::(*)(targetPtr);
                )::");
}
else if (field_lenprefix_1)
{
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::(_Accessor_Field.CellPtr = targetPtr + 4;
                )::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::(_Accessor_Field.CellID = this.CellID;
                return )::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::(_Accessor_Field;
                )::");
}
else
{
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::(_Accessor_Field.CellPtr = targetPtr;
                )::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::(_Accessor_Field.CellID = this.CellID;
                return )::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::(_Accessor_Field;
                )::");
}
source->append(R"::(
            }
            set
            {
                byte* targetPtr = CellPtr;
                )::");
if (field_need_accessor_1)
{
source->append(R"::(
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                )::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::(_Accessor_Field.CellID = this.CellID;
                )::");
}
if (field_optional_1)
{
source->append(R"::(
                bool creatingOptionalField = (!this.Contains_)::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::();
                if (creatingOptionalField)
                {
                    this.Contains_)::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::( = true;
                    )::");

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = context->m_stack_depth + 1;
module_ctx.m_arguments.push_back(Codegen::GetString(accessor_field_name_1));
module_ctx.m_arguments.push_back(Codegen::GetString("FieldDoesNotExist"));
std::string* module_content = Modules::AccessorToAccessorFieldAssignment((*(node->fieldList))[iterator_1]->fieldType, &module_ctx);
    source->append(*module_content);
    delete module_content;
}
if (!field_need_accessor_1)
{
source->append(R"::( 
                    targetPtr = this.ResizeFunction(targetPtr, 0, )::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->fieldType->type_size()));
source->append(R"::();
                    )::");
}
source->append(R"::(
                }
                else
                {
                    )::");

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = context->m_stack_depth + 1;
module_ctx.m_arguments.push_back(Codegen::GetString(accessor_field_name_1));
module_ctx.m_arguments.push_back(Codegen::GetString("FieldExists"));
std::string* module_content = Modules::AccessorToAccessorFieldAssignment((*(node->fieldList))[iterator_1]->fieldType, &module_ctx);
    source->append(*module_content);
    delete module_content;
}
source->append(R"::(
                }
                )::");
}
else
{

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = context->m_stack_depth + 1;
module_ctx.m_arguments.push_back(Codegen::GetString(accessor_field_name_1));
module_ctx.m_arguments.push_back(Codegen::GetString("FieldExists"));
std::string* module_content = Modules::AccessorToAccessorFieldAssignment((*(node->fieldList))[iterator_1]->fieldType, &module_ctx);
    source->append(*module_content);
    delete module_content;
}
}
source->append(R"::(
                *()::");
source->append(Codegen::GetString(data_type_get_accessor_name((*(node->fieldList))[iterator_1]->fieldType)));
source->append(R"::(*)(targetPtr) = value;
            }
        }
        )::");
}

                return source;
            }
        }
    }
}
