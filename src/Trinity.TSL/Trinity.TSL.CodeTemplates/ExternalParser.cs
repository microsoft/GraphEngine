#pragma warning disable 0162 // disable the "unreachable code" warning
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.TSL;
using Trinity.TSL.Lib;

/*MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")*/
namespace t_Namespace
{
    [TARGET("NTSL")]
    [MAP_LIST("t_data_type", "Trinity::Codegen::TSLExternalParserDataTypeVector")]
    [MAP_VAR("t_data_type", "")]
    [MAP_VAR("t_data_type_remove_nullable", "Trinity::Codegen::GetNonNullableValueTypeString($$)")]
    [MAP_VAR("t_data_type_display", "Trinity::Codegen::GetDataTypeDisplayString($$)")]
    [MAP_VAR("t_data_type_list_element_type", "listElementType")]
    [MAP_VAR("t_data_type_list_element_type_display", "Trinity::Codegen::GetDataTypeDisplayString($$->listElementType)")]
    [MAP_VAR("t_data_type_array_element_type", "arrayInfo.arrayElement")]
    [MAP_VAR("t_data_type_array_element_type_display", "Trinity::Codegen::GetDataTypeDisplayString($$->arrayInfo.arrayElement)")]
    [MAP_LIST("t_cell", "node->cellList")]
    [MAP_LIST("t_struct", "node->structList")]
    [MAP_VAR("t_cell_name", "name")]
    [MAP_VAR("t_struct_name", "name")]
    [MAP_VAR("t_array_iterator_prefix", "Trinity::Codegen::GetDataTypeDisplayString($$->arrayInfo.arrayElement)", MemberOf = "t_data_type")]
    [MAP_LIST("t_array_iterator_list", "arrayInfo.array_dimension_size", MemberOf = "t_data_type")]
    [MAP_VAR("t_array_iterator_length", "", MemberOf = "t_array_iterator_list")]
    [MAP_VAR("t_array_iterator_string", "t_array_iterator_prefix + '_' + GetString(GET_ITERATOR_VALUE()) + Discard($$)", MemberOf = "t_array_iterator_list")]
    internal class ExternalParser : __meta
    {
        [FOREACH]
        [IF("data_type_need_external_parser($t_data_type) && data_type_need_set_field($t_data_type, Trinity::Codegen::TSLExternalParserDataTypeVector)")]
        internal static unsafe bool TryParse_t_data_type_display(string s, out t_data_type value)
        {
            t_data_type_remove_nullable value_type_value;
            JArray jarray;

            IF("$t_data_type->is_bool()");
            {
                IF("$t_data_type->is_nullable()");
                {
                    if (string.IsNullOrEmpty(s) || string.Compare(s, "null", ignoreCase: true) == 0)
                    {
                        value = default(t_data_type);
                        return true;
                    }
                }
                END();

                double double_val;

                if (t_data_type_remove_nullable.TryParse(s, out value_type_value))
                {
                    value = value_type_value;
                    return true;
                }
                else if (double.TryParse(s, out double_val))
                {
                    LITERAL_OUTPUT("value = (double_val != 0);");
                    MUTE();
                    value = value_type_value;
                    MUTE_END();
                    return true;
                }
                else
                {
                    value = default(t_data_type);
                    return false;
                }
            }
            ELIF("$t_data_type->is_datetime()");
            {
                IF("$t_data_type->is_nullable()");
                {
                    if (string.IsNullOrEmpty(s) || string.Compare(s, "null", ignoreCase: true) == 0)
                    {
                        value = default(t_data_type);
                        return true;
                    }
                }
                END();
                if (t_data_type_remove_nullable.TryParse(s, null, System.Globalization.DateTimeStyles.RoundtripKind, out value_type_value))
                {
                    value = value_type_value;
                    return true;
                }
                //special case for non-standard date time strings
                if (s.EndsWith(" UTC", StringComparison.Ordinal) && t_data_type_remove_nullable.TryParse(s.Substring(0, s.Length - 4) + 'Z', null, System.Globalization.DateTimeStyles.RoundtripKind, out value_type_value))
                {
                    //replace " UTC" with "Z"
                    value = value_type_value;
                    return true;
                }

                value  = default(t_data_type);
                return false;
            }
            ELIF("$t_data_type->is_nullable() /* and has built-in TryParse */");
            if (string.IsNullOrEmpty(s) || string.Compare(s, "null", ignoreCase: true) == 0)
            {
                value = default(t_data_type);
                return true;
            }
            else if (t_data_type_remove_nullable.TryParse(s, out value_type_value))
            {
                value = value_type_value;
                return true;
            }
            else
            {
                value = default(t_data_type);
                return false;
            }
            ELIF("$t_data_type->is_array()");
            try
            {
                MUTE();
                //Just to make csc happy with the meta template.
                value = default(t_data_type);
                int t_array_iterator_length = 1;
                string element = "";
                MUTE_END();
                jarray = JArray.Parse(s);
                //There are some situations that our template system doesn't work...
                //So we use LITERAL_OUTPUT to output something hard to express
                LITERAL_OUTPUT("value = new "); META_OUTPUT("data_type_get_array_type_with_size_string($t_data_type)"); LITERAL_OUTPUT(";");

                META_OUTPUT("$t_data_type_array_element_type"); LITERAL_OUTPUT(" element;");
                int t_array_iterator_prefix_offset = 0;
                FOREACH();
                for (int t_array_iterator_string = 0; t_array_iterator_string < t_array_iterator_length; ++t_array_iterator_string)
                /*END*/
                {
                    IF("data_type_need_external_parser($t_data_type_array_element_type)");
                    {
                        if (!ExternalParser.TryParse_t_data_type_array_element_type((string)jarray[t_array_iterator_prefix_offset++], out element))
                            continue;
                        value[/*FOREACH(",")*/t_array_iterator_string/*END*/] = element;
                    }
                    ELIF("$t_data_type_array_element_type->is_string()");
                    {
                        value[/*FOREACH(",")*/t_array_iterator_string/*END*/] = (string)jarray[t_array_iterator_prefix_offset++];
                    }
                    ELSE();
                    {
                        if (!t_data_type_array_element_type.TryParse((string)jarray[t_array_iterator_prefix_offset++], out element))
                            continue;
                        value[/*FOREACH(",")*/t_array_iterator_string/*END*/] = element;
                    }
                    END();
                }
                return true;
            }
            catch
            {
                value = default(t_data_type);
                return false;
            }
            ELIF("$t_data_type->is_list()");
            try
            {
                value = new t_data_type();
                jarray = JArray.Parse(s);
                foreach (var jarray_element in jarray)
                {
                    t_data_type_list_element_type element;

                    IF("data_type_need_external_parser($t_data_type->listElementType)");
                    if (!ExternalParser.TryParse_t_data_type_list_element_type_display((string)jarray_element, out element))
                    {
                        continue;
                        //TODO what to do if we fail on an item?
                        //Same situation applies for arrays also.
                        //throw new Exception("Cannot parse \""+capture.Value+"\" into [META_OUTPUT($t_data_type->listElementType)].");
                    }
                    value.Add(element);
                    ELIF("$t_data_type->listElementType->is_string()");//Assign unescaped json string.
                    value.Add((string)jarray_element);
                    ELSE();
                    if (!t_data_type_list_element_type.TryParse((string)jarray_element, out element))
                    {
                        continue;
                        //throw new Exception("Cannot parse \""+capture.Value+"\" into [META_OUTPUT($t_data_type->listElementType)].");
                    }
                    value.Add(element);
                    END();
                }
                return true;
            }
            catch
            {
                value = default(t_data_type);
                return false;
            }
            END();
        }

        [END]//IF
        [END]//FOREACH

        #region Mute
        [MUTE]
        private static bool TryParse_t_data_type_list_element_type_display(string p, out t_data_type_list_element_type element)
        {
            throw new NotImplementedException();
        }
        private static bool TryParse_t_data_type_array_element_type(string p, out string element)
        {
            throw new NotImplementedException();
        }
        private static bool TryParse_t_data_type_display(string s, out t_data_type_remove_nullable value_type_value)
        {
            throw new NotImplementedException();
        }
        internal static unsafe bool TryParse_t_field_type_display(string value, out t_field_type t_field_type)
        {
            throw new NotImplementedException();
        }
        internal static bool TryParse_t_field_type_list_element_type_display(t_data_type value, out t_field_type_list_element_type parseResult)
        {
            throw new NotImplementedException();
        }
        internal static bool TryParse_t_field_type_list_element_type_display(string element, out t_field_type_list_element_type parseResult)
        {
            throw new NotImplementedException();
        }
        internal static unsafe bool TryParse_t_field_type_2_display(t_field_type value, out t_field_type_2 intermediate_result)
        {
            throw new NotImplementedException();
        }
        internal static unsafe bool TryParse_t_field_type_2_element_type_display(t_field_type value, out t_field_type_2_element_type intermediate_result)
        {
            throw new NotImplementedException();
        }
        internal static bool TryParse_t_field_type_2_element_type_display(t_field_type_list_element_type element, out t_field_type_2_element_type intermediate_result)
        {
            throw new NotImplementedException();
        }
        /*MUTE_END*/
        #endregion
    }
}
