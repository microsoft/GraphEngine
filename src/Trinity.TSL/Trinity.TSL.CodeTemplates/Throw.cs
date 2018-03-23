#pragma warning disable 0162 // disable the "unreachable code" warning
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.Storage;
using Trinity.TSL;
using Trinity.TSL.Lib;

/*MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")*/
namespace t_Namespace
{
    [TARGET("NTSL")]
    [MAP_LIST("t_data_type", "Trinity::Codegen::TSLExternalParserDataTypeVector")]
    [MAP_VAR("t_data_type", "")]
    [MAP_VAR("t_data_type_display", "Trinity::Codegen::GetDataTypeDisplayString($$)")]
    class Throw
    {
        [FOREACH]
        [IF("data_type_need_set_field($t_data_type, Trinity::Codegen::TSLExternalParserDataTypeVector) ")]
        internal static void parse_t_data_type_display(string value)
        {
            throw new ArgumentException("Cannot parse \""+value+"\" into [META_OUTPUT($t_data_type)].");
        }
        internal static void incompatible_with_t_data_type_display()
        {
            throw new DataTypeIncompatibleException("Data type '[META_OUTPUT($t_data_type)]' not compatible with the target field.");
        }
        [END]
        [END]

        internal static void data_type_incompatible_with_list(string type)
        {
            throw new DataTypeIncompatibleException("Data type '" + type + "' not compatible with the target list.");
        }

        internal static void data_type_incompatible_with_field(string type)
        {
            throw new DataTypeIncompatibleException("Data type '" + type + "' not compatible with the target field.");
        }

        internal static void target__field_not_list()
        {
            throw new DataTypeIncompatibleException("Target field is not a List, value or a string, cannot perform append operation.");
        }

        internal static void list_incompatible_list(string type)
        {
            throw new DataTypeIncompatibleException("List type '" + type + "' not compatible with the target list.");
        }

        internal static void incompatible_with_cell()
        {
            throw new DataTypeIncompatibleException("Data type incompatible with the cell.");
        }

        internal static void array_dimension_size_mismatch(string type)
        {
            throw new ArgumentException(type + ": Array dimension size mismatch.");
        }

        internal static void invalid_cell_type()
        {
            throw new ArgumentException("Invalid cell type name. If you want a new cell type, please define it in your TSL.");
        }

        internal static void undefined_field()
        {
            throw new ArgumentException("Undefined field.");
        }

        [MUTE]
        internal static void parse_t_field_type_display(string value)
        {
            throw new NotImplementedException();
        }
        internal static void parse_t_field_type_list_element_type_display(string value)
        {
            throw new NotImplementedException();
        }
        internal static void incompatible_with_t_field_type_display()
        {
            throw new NotImplementedException();
        }
        [MUTE_END]

        internal static void member_access_on_non_struct__field(string field_name_string)
        {
            throw new DataTypeIncompatibleException("Cannot apply member access method on a non-struct field'" + field_name_string + "'.");
        }

        internal static void cell_not_found()
        {
            throw new CellNotFoundException("The cell is not found.");
        }

        internal static void cell_not_found(long cellId)
        {
            throw new CellNotFoundException("The cell with id = " + cellId + " not found.");
        }

        internal static void wrong_cell_type()
        {
            throw new CellTypeNotMatchException("Cell type mismatched.");
        }

        internal static unsafe void cannot_parse(string value, string type_str)
        {
            throw new ArgumentException("Cannot parse \""+value+"\" into " + type_str + ".");
        }

        internal static unsafe byte* invalid_resize_on_fixed_struct()
        {
            throw new InvalidOperationException("Invalid resize operation on a fixed struct.");
        }
    }
}
