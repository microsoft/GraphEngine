using System.Text;
using System.Collections.Generic;

namespace Trinity.TSL.CodeTemplates
{
    internal partial class SourceFiles
    {
        internal static string 
GenericFieldAccessor(
NTSL node)
        {
            StringBuilder source = new StringBuilder();
            
source.Append(@"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.TSL;
using Trinity.TSL.Lib;
namespace ");
source.Append(Codegen.GetString(Trinity::Codegen::GetNamespace()));
source.Append(@"
{
    internal struct GenericFieldAccessor
    {
        #region FieldID lookup table
        ");
for (int iterator_1 = 0; iterator_1 < (node->structList).Count;++iterator_1)
{
source.Append(@"
        static Dictionary<string, uint> FieldLookupTable_");
source.Append(Codegen.GetString((node->structList)[iterator_1].name));
source.Append(@" = new Dictionary<string, uint>()
        {
            ");
for (int iterator_2 = 0; iterator_2 < ((node->structList)[iterator_1].fieldList).Count;++iterator_2)
{
source.Append(@"
            {""");
source.Append(Codegen.GetString(((node->structList)[iterator_1].fieldList)[iterator_2].name));
source.Append(@""" , ");
source.Append(Codegen.GetString(iterator_2));
source.Append(@"}
            ");
if (iterator_2 < ((node->structList)[iterator_1].fieldList).Count - 1)
source.Append(",");
}
source.Append(@"
        };
        ");
}
source.Append(@"
        #endregion
        ");
for (int iterator_1 = 0; iterator_1 < (node->structList).Count;++iterator_1)
{
source.Append(@"
        internal static void SetField<T>(");
source.Append(Codegen.GetString(GetString((node->structList)[iterator_1]->name) + "_Accessor"));
source.Append(@" accessor, string fieldName, int field_name_idx, T value)
        {
            uint member_id;
            int field_divider_idx = fieldName.IndexOf('.', field_name_idx);
            if (-1 != field_divider_idx)
            {
                string member_name_string = fieldName.Substring(field_name_idx, field_divider_idx - field_name_idx);
                if (!FieldLookupTable_");
source.Append(Codegen.GetString((node->structList)[iterator_1].name));
source.Append(@".TryGetValue(member_name_string, out member_id))
                    Throw.undefined_field();
                switch (member_id)
                {
                    ");
for (int iterator_2 = 0; iterator_2 < ((node->structList)[iterator_1].fieldList).Count;++iterator_2)
{
if (((node->structList)[iterator_1].fieldList)[iterator_2].fieldType->is_struct())
{
source.Append(@"
                    case ");
source.Append(Codegen.GetString(iterator_2));
source.Append(@":
                        GenericFieldAccessor.SetField(accessor.");
source.Append(Codegen.GetString(((node->structList)[iterator_1].fieldList)[iterator_2].name));
source.Append(@", fieldName, field_divider_idx + 1, value);
                        break;
                    ");
}
}
source.Append(@"
                    default:
                        Throw.member_access_on_non_struct__field(member_name_string);
                        break;
                }
                return;
            }
            fieldName = fieldName.Substring(field_name_idx);
            if (!FieldLookupTable_");
source.Append(Codegen.GetString((node->structList)[iterator_1].name));
source.Append(@".TryGetValue(fieldName, out member_id))
                Throw.undefined_field();
            switch (member_id)
            {
                ");
for (int iterator_2 = 0; iterator_2 < ((node->structList)[iterator_1].fieldList).Count;++iterator_2)
{
source.Append(@"
                case ");
source.Append(Codegen.GetString(iterator_2));
source.Append(@":
                    {
                        ");
source.Append(Codegen.GetString(((node->structList)[iterator_1].fieldList)[iterator_2].fieldType));
source.Append(@" conversion_result = TypeConverter<T>.ConvertTo_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString(((node->structList)[iterator_1].fieldList)[iterator_2]->fieldType)));
source.Append(@"(value);
                        ");

{
    ModuleContext module_ctx = new ModuleContext();
    module_ctx.m_stack_depth = 0;
module_ctx.m_arguments.Add(Codegen.GetString("accessor"));
module_ctx.m_arguments.Add(Codegen.GetString("conversion_result"));
string module_content = Modules.AccessorFieldAssignment(((node->structList)[iterator_1].fieldList)[iterator_2], module_ctx);
    source.Append(module_content);
}
source.Append(@"
                        break;
                    }
                ");
}
source.Append(@"
            }
        }
        internal static T GetField<T>(");
source.Append(Codegen.GetString(GetString((node->structList)[iterator_1]->name) + "_Accessor"));
source.Append(@" accessor, string fieldName, int field_name_idx)
        {
            uint member_id;
            int field_divider_idx = fieldName.IndexOf('.', field_name_idx);
            if (-1 != field_divider_idx)
            {
                string member_name_string = fieldName.Substring(field_name_idx, field_divider_idx - field_name_idx);
                if (!FieldLookupTable_");
source.Append(Codegen.GetString((node->structList)[iterator_1].name));
source.Append(@".TryGetValue(member_name_string, out member_id))
                    Throw.undefined_field();
                switch (member_id)
                {
                    ");
for (int iterator_2 = 0; iterator_2 < ((node->structList)[iterator_1].fieldList).Count;++iterator_2)
{
if (((node->structList)[iterator_1].fieldList)[iterator_2].fieldType->is_struct())
{
source.Append(@"
                    case ");
source.Append(Codegen.GetString(iterator_2));
source.Append(@":
                        return GenericFieldAccessor.GetField<T>(accessor.");
source.Append(Codegen.GetString(((node->structList)[iterator_1].fieldList)[iterator_2].name));
source.Append(@", fieldName, field_divider_idx + 1);
                    ");
}
}
source.Append(@"
                    default:
                        Throw.member_access_on_non_struct__field(member_name_string);
                        break;
                }
            }
            fieldName = fieldName.Substring(field_name_idx);
            if (!FieldLookupTable_");
source.Append(Codegen.GetString((node->structList)[iterator_1].name));
source.Append(@".TryGetValue(fieldName, out member_id))
                Throw.undefined_field();
            switch (member_id)
            {
                ");
for (int iterator_2 = 0; iterator_2 < ((node->structList)[iterator_1].fieldList).Count;++iterator_2)
{
source.Append(@"
                case ");
source.Append(Codegen.GetString(iterator_2));
source.Append(@":
                    return TypeConverter<T>.ConvertFrom_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString(((node->structList)[iterator_1].fieldList)[iterator_2]->fieldType)));
source.Append(@"(accessor.");
source.Append(Codegen.GetString(((node->structList)[iterator_1].fieldList)[iterator_2].name));
source.Append(@");
                    break;
                ");
}
source.Append(@"
            }
            /* Should not reach here */
            throw new Exception(""Internal error T5008"");
        }
        ");
}
source.Append(@"
    }
}
");

            return source.ToString();
        }
    }
}
