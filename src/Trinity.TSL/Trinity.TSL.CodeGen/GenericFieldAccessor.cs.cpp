#include "common.h"
#include <string>
#include "SyntaxNode.h"

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        string* 
GenericFieldAccessor(
NTSL* node)
        {
            string* source = new string();
            
source->append(R"::(
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.TSL;
using Trinity.TSL.Lib;
namespace )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(
{
    
    internal struct GenericFieldAccessor
    {
        #region FieldID lookup table
        )::");
for (size_t iterator_1 = 0; iterator_1 < (node->structList)->size();++iterator_1)
{
source->append(R"::(
        static Dictionary<string, uint> FieldLookupTable_)::");
source->append(Codegen::GetString((*(node->structList))[iterator_1]->name));
source->append(R"::( = new Dictionary<string, uint>()
        {
            )::");
for (size_t iterator_2 = 0; iterator_2 < ((*(node->structList))[iterator_1]->fieldList)->size();++iterator_2)
{
source->append(R"::(
            {")::");
source->append(Codegen::GetString((*((*(node->structList))[iterator_1]->fieldList))[iterator_2]->name));
source->append(R"::(" , )::");
source->append(Codegen::GetString(iterator_2));
source->append(R"::(}
            )::");
if (iterator_2 < ((*(node->structList))[iterator_1]->fieldList)->size() - 1)
source->append(",");
}
source->append(R"::(
        };
        )::");
}
source->append(R"::(
        #endregion
        )::");
for (size_t iterator_1 = 0; iterator_1 < (node->structList)->size();++iterator_1)
{
source->append(R"::(
        internal static void SetField<T>()::");
source->append(Codegen::GetString(GetString((*(node->structList))[iterator_1]->name) + "_Accessor"));
source->append(R"::( accessor, string fieldName, int field_name_idx, T value)
        {
            uint member_id;
            int field_divider_idx = fieldName.IndexOf('.', field_name_idx);
            if (-1 != field_divider_idx)
            {
                string member_name_string = fieldName.Substring(field_name_idx, field_divider_idx - field_name_idx);
                if (!FieldLookupTable_)::");
source->append(Codegen::GetString((*(node->structList))[iterator_1]->name));
source->append(R"::(.TryGetValue(member_name_string, out member_id))
                    Throw.undefined_field();
                switch (member_id)
                {
                    )::");
for (size_t iterator_2 = 0; iterator_2 < ((*(node->structList))[iterator_1]->fieldList)->size();++iterator_2)
{
if ((*((*(node->structList))[iterator_1]->fieldList))[iterator_2]->fieldType->is_struct())
{
source->append(R"::(
                    case )::");
source->append(Codegen::GetString(iterator_2));
source->append(R"::(:
                        GenericFieldAccessor.SetField(accessor.)::");
source->append(Codegen::GetString((*((*(node->structList))[iterator_1]->fieldList))[iterator_2]->name));
source->append(R"::(, fieldName, field_divider_idx + 1, value);
                        break;
                    )::");
}
}
source->append(R"::(
                    default:
                        Throw.member_access_on_non_struct__field(member_name_string);
                        break;
                }
                return;
            }
            fieldName = fieldName.Substring(field_name_idx);
            if (!FieldLookupTable_)::");
source->append(Codegen::GetString((*(node->structList))[iterator_1]->name));
source->append(R"::(.TryGetValue(fieldName, out member_id))
                Throw.undefined_field();
            switch (member_id)
            {
                )::");
for (size_t iterator_2 = 0; iterator_2 < ((*(node->structList))[iterator_1]->fieldList)->size();++iterator_2)
{
source->append(R"::(
                case )::");
source->append(Codegen::GetString(iterator_2));
source->append(R"::(:
                    {
                        )::");
source->append(Codegen::GetString((*((*(node->structList))[iterator_1]->fieldList))[iterator_2]->fieldType));
source->append(R"::( conversion_result = TypeConverter<T>.ConvertTo_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*((*(node->structList))[iterator_1]->fieldList))[iterator_2]->fieldType)));
source->append(R"::((value);
                        )::");

{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = 0;
module_ctx.m_arguments.push_back(Codegen::GetString("accessor"));
module_ctx.m_arguments.push_back(Codegen::GetString("conversion_result"));
std::string* module_content = Modules::ValueToAccessorFieldAssignment((*((*(node->structList))[iterator_1]->fieldList))[iterator_2], &module_ctx);
    source->append(*module_content);
    delete module_content;
}
source->append(R"::(
                        break;
                    }
                )::");
}
source->append(R"::(
            }
        }
        internal static T GetField<T>()::");
source->append(Codegen::GetString(GetString((*(node->structList))[iterator_1]->name) + "_Accessor"));
source->append(R"::( accessor, string fieldName, int field_name_idx)
        {
            uint member_id;
            int field_divider_idx = fieldName.IndexOf('.', field_name_idx);
            if (-1 != field_divider_idx)
            {
                string member_name_string = fieldName.Substring(field_name_idx, field_divider_idx - field_name_idx);
                if (!FieldLookupTable_)::");
source->append(Codegen::GetString((*(node->structList))[iterator_1]->name));
source->append(R"::(.TryGetValue(member_name_string, out member_id))
                    Throw.undefined_field();
                switch (member_id)
                {
                    )::");
for (size_t iterator_2 = 0; iterator_2 < ((*(node->structList))[iterator_1]->fieldList)->size();++iterator_2)
{
if ((*((*(node->structList))[iterator_1]->fieldList))[iterator_2]->fieldType->is_struct())
{
source->append(R"::(
                    case )::");
source->append(Codegen::GetString(iterator_2));
source->append(R"::(:
                        return GenericFieldAccessor.GetField<T>(accessor.)::");
source->append(Codegen::GetString((*((*(node->structList))[iterator_1]->fieldList))[iterator_2]->name));
source->append(R"::(, fieldName, field_divider_idx + 1);
                    )::");
}
}
source->append(R"::(
                    default:
                        Throw.member_access_on_non_struct__field(member_name_string);
                        break;
                }
            }
            fieldName = fieldName.Substring(field_name_idx);
            if (!FieldLookupTable_)::");
source->append(Codegen::GetString((*(node->structList))[iterator_1]->name));
source->append(R"::(.TryGetValue(fieldName, out member_id))
                Throw.undefined_field();
            switch (member_id)
            {
                )::");
for (size_t iterator_2 = 0; iterator_2 < ((*(node->structList))[iterator_1]->fieldList)->size();++iterator_2)
{
source->append(R"::(
                case )::");
source->append(Codegen::GetString(iterator_2));
source->append(R"::(:
                    return TypeConverter<T>.ConvertFrom_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*((*(node->structList))[iterator_1]->fieldList))[iterator_2]->fieldType)));
source->append(R"::((accessor.)::");
source->append(Codegen::GetString((*((*(node->structList))[iterator_1]->fieldList))[iterator_2]->name));
source->append(R"::();
                    break;
                )::");
}
source->append(R"::(
            }
            /* Should not reach here */
            throw new Exception("Internal error T5008");
        }
        )::");
}
source->append(R"::(
    }
}
)::");

            return source;
        }
    }
}
