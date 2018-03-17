#pragma warning disable 0162 // disable the "unreachable code" warning
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.TSL;
using Trinity.TSL.Lib;

/*MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")*/
namespace t_Namespace
{
    [TARGET("NTSL")]
    [MAP_LIST("t_data_type", "Trinity::Codegen::TSLSerializerDataTypeVector")]
    [MAP_VAR("t_data_type", "")]
    [MAP_LIST("t_field", "referencedNStruct->fieldList", MemberOf = "t_data_type")]
    [MAP_VAR("t_field_type", "fieldType")]
    [MAP_VAR("t_field_name", "name")]
    [MAP_LIST("t_cell", "node->cellList")]
    [MAP_VAR("t_cell", "")]
    [MAP_VAR("t_cell_name", "name", MemberOf = "t_cell")]
    /// <summary>
    /// Provides facilities for serializing data to Json strings.
    /// </summary>
    public class Serializer : __meta
    {
        [ThreadStatic]
        static StringBuilder s_stringBuilder;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void s_ensure_string_builder()
        {
            if (s_stringBuilder == null)
                s_stringBuilder = new StringBuilder();
            else
                s_stringBuilder.Clear();
        }

        [FOREACH]
        [IF("data_type_need_type_id($t_data_type, Trinity::Codegen::TSLSerializerDataTypeVector) && data_type_is_not_duplicate_array($t_data_type, Trinity::Codegen::TSLSerializerDataTypeVector)")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Serializes a t_data_type object to Json string.
        /// </summary>
        /// <param name="value">The target object to be serialized.</param>
        /// <returns>The serialized Json string.</returns>
        public static string ToString(t_data_type value)
        {
            s_ensure_string_builder();
            ToString_impl(value, s_stringBuilder, in_json: false);
            return s_stringBuilder.ToString();
        }
        [END]
        [END]

        [MAP_LIST("t_field", "fieldList", MemberOf = "t_cell")]
        [FOREACH]
        /// <summary>
        /// Serializes a t_cell_name object to Json string.
        /// </summary>
        /// <param name="value">The target cell object to be serialized.</param>
        /// <returns>The serialized Json string.</returns>
        public static string ToString(t_cell_name cell)
        {
            s_ensure_string_builder();

            s_stringBuilder.Append('{');
            s_stringBuilder.AppendFormat("\"CellId\":{0}", cell.CellId);

            FOREACH();
            {
                IF("$t_field_type->is_nullable() || !$t_field_type->is_value_type()");
                if (cell.t_field_name != null)
                {
                    END();
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"t_field_name\":");
                    ToString_impl(cell.t_field_name, s_stringBuilder, in_json: true);
                    IF("$t_field_type->is_nullable() || !$t_field_type->is_value_type()");
                }
                END();
            }
            END();
            s_stringBuilder.Append('}');
            return s_stringBuilder.ToString();
        }
        [END]

        [MAP_LIST("t_field", "referencedNStruct->fieldList", MemberOf = "t_data_type")]
        [FOREACH]
        [IF("data_type_need_type_id($t_data_type, Trinity::Codegen::TSLSerializerDataTypeVector) && data_type_is_not_duplicate_array($t_data_type, Trinity::Codegen::TSLSerializerDataTypeVector)")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ToString_impl(t_data_type value, StringBuilder str_builder, bool in_json)
        {
            //For top-level element, just return the literal representation.
            //For non-top-level element, it would be an element in a json structure. (object, or array)
            //Thus we have to escape strings, surround fields with quotes (") if necessary.

            IF("$t_data_type->is_string()");
            if (in_json)
            {
                str_builder.Append(JsonStringProcessor.escape(value));
            }
            else
            {
                str_builder.Append(value);
            }
            ELIF("$t_data_type->is_struct()");
            {
                IF("$t_data_type->is_optional()");
                if (value == null)
                    return;
                END();
                str_builder.Append('{');
                bool first_field = true;
                FOREACH();
                {
                    IF("$t_field_type->is_nullable() || !$t_field_type->is_value_type()");
                    IF("$t_data_type->is_optional()");
                    if (value.Value.t_field_name != null)
                        ELSE();
                    if (value.t_field_name != null)
                    /*END*/
                    {
                        END();
                        if(first_field)
                            first_field = false;
                        else
                            str_builder.Append(',');

                        str_builder.Append("\"t_field_name\":");


                        IF("$t_data_type->is_optional()");
                        ToString_impl(value.Value.t_field_name, str_builder, in_json: true);
                        ELSE();
                        ToString_impl(value.t_field_name, str_builder, in_json: true);
                        END();

                        IF("$t_field_type->is_nullable() || !$t_field_type->is_value_type()");
                    }
                    END();
                }
                END();
                str_builder.Append('}');
            }
            ELIF("$t_data_type->is_value_type()");
            {
                IF("$t_data_type->is_enum() || $t_data_type->is_datetime() || $t_data_type->is_guid()");
                if(in_json)
                    str_builder.Append('"');
                END();

                IF("$t_data_type->is_bool()");
                {
                    str_builder.Append(value.ToString().ToLowerInvariant());
                }
                ELIF("$t_data_type->is_datetime()");
                {
                    IF("$t_data_type->is_nullable()");
                    {
                        str_builder.Append(value.Value.ToString("o", CultureInfo.InvariantCulture));
                    }
                    ELSE();
                    {
                        str_builder.Append(value.ToString("o", CultureInfo.InvariantCulture));
                    }
                    END();
                }
                ELSE();
                {
                    str_builder.Append(value);
                }
                END();

                IF("$t_data_type->is_enum() || $t_data_type->is_datetime() || $t_data_type->is_guid()");
                if(in_json)
                    str_builder.Append('"');
                END();
            }
            ELIF("$t_data_type->is_array() || $t_data_type->is_list()");
            {
                str_builder.Append('[');
                bool first = true;
                foreach (var element in value)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        str_builder.Append(',');
                    }
                    ToString_impl(element, str_builder, in_json:true);
                }
                str_builder.Append(']');
            }
            ELSE();
            throw new Exception("Internal error T5007");
            END();
        }
        [END]
        [END]

        #region mute
        [MUTE]
        private static void ToString_impl(object element, StringBuilder s_stringBuilder, bool in_json)
        {
            throw new NotImplementedException();
        }
        internal static string ToString(t_struct_name response_struct)
        {
            throw new NotImplementedException();
        }
        /*MUTE_END*/
        #endregion
    }
}
