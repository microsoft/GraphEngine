#pragma warning disable 0162 // disable the "unreachable code" warning
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.TSL;
using Trinity.TSL.Lib;

/*MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")*/
namespace t_Namespace
{
    [TARGET("NTSL")]
    //TODO better name... TSLDataTypeVector is for cell, TSLExternalParserDataTypeVector is for all data types
    [MAP_LIST("t_data_type", "Trinity::Codegen::TSLExternalParserDataTypeVector")]
    [MAP_VAR("t_data_type", "")]
    [MAP_VAR("t_data_type_display", "Trinity::Codegen::GetDataTypeDisplayString($$)")]
    [MAP_LIST("t_field_type", "node->structList")]
    [MAP_VAR("t_field_type", "GetString($$->name) + \"_Accessor\"")]
    [MAP_VAR("t_struct", "", MemberOf = "t_field_type")]
    [MAP_VAR("t_field_name", "name", MemberOf = "t_field_type")]
    [MAP_LIST("t_member", "fieldList", MemberOf = "t_field_type")]
    [MAP_VAR("t_member", "")]
    [MAP_VAR("t_member_name", "name")]
    [MAP_VAR("t_member_type", "fieldType", MemberOf = "t_member")]
    [MAP_LIST("t_array_dimension_list", "fieldType->arrayInfo.array_dimension_size", MemberOf = "t_member")]
    [MAP_VAR("t_array_dimension_list", "")]
    [MAP_VAR("t_field_type_2", "fieldType", MemberOf = "t_member")]
    [MAP_VAR("t_field_type_2_display", "Trinity::Codegen::GetDataTypeDisplayString($$->fieldType)", MemberOf = "t_member")]
    [MAP_VAR("t_uint", "GET_ITERATOR_VALUE()")]
    [STRUCT]
    internal class GenericFieldAccessor : __meta
    {
        #region FieldID lookup table
        [FOREACH]
        static Dictionary<string, uint> FieldLookupTable_t_field_name = new Dictionary<string, uint>()
        {
            /*FOREACH(",")*/
            {"t_member_name" , GET_ITERATOR_VALUE()}
            /*END*/
        };
        [END]
        #endregion

        [FOREACH]
        [USE_LIST("t_field_type")]
        internal static void SetField<T>(t_field_type accessor, string fieldName, int field_name_idx, T value)
        {
            uint member_id;

            int field_divider_idx = fieldName.IndexOf('.', field_name_idx);
            if (-1 != field_divider_idx)
            {
                string member_name_string = fieldName.Substring(field_name_idx, field_divider_idx - field_name_idx);
                if (!FieldLookupTable_t_field_name.TryGetValue(member_name_string, out member_id))
                    Throw.undefined_field();

                switch (member_id)
                {
                    /*FOREACH*/
                    /*USE_LIST("t_member")*/
                    /*  IF("$t_member_type->is_struct()")*/
                    case t_uint:
                        GenericFieldAccessor.SetField(accessor.t_member_name, fieldName, field_divider_idx + 1, value);
                        break;
                    /*  END*/
                    /*END*/
                    default:
                        // @note   We cannot go further unless it's a struct.
                        //         Throw exception now.
                        Throw.member_access_on_non_struct__field(member_name_string);
                        break;
                }
                return;
            }

            fieldName = fieldName.Substring(field_name_idx);
            if (!FieldLookupTable_t_field_name.TryGetValue(fieldName, out member_id))
                Throw.undefined_field();
            switch (member_id)
            {
                /*FOREACH*/
                /*USE_LIST("t_member")*/
                case t_uint:
                    {
                        t_field_type_2 conversion_result = TypeConverter<T>.ConvertTo_t_field_type_2_display(value);
                        MODULE_CALL("AccessorFieldAssignment", "$t_member", "\"accessor\"", "\"conversion_result\"");
                        break;
                    }
                /*END*/
            }
        }

        internal static T GetField<T>(t_field_type accessor, string fieldName, int field_name_idx)
        {
            uint member_id;

            int field_divider_idx = fieldName.IndexOf('.', field_name_idx);
            if (-1 != field_divider_idx)
            {
                string member_name_string = fieldName.Substring(field_name_idx, field_divider_idx - field_name_idx);
                if (!FieldLookupTable_t_field_name.TryGetValue(member_name_string, out member_id))
                    Throw.undefined_field();
                switch (member_id)
                {
                    /*FOREACH*/
                    /*USE_LIST("t_member")*/
                    /*  IF("$t_member_type->is_struct()")*/
                    case t_uint:
                        return GenericFieldAccessor.GetField<T>(accessor.t_member_name, fieldName, field_divider_idx + 1);
                    /*  END*/
                    /*END*/
                    default:
                        // @note   We cannot go further unless it's a struct.
                        //         Throw exception now.
                        Throw.member_access_on_non_struct__field(member_name_string);
                        break;
                }
            }

            fieldName = fieldName.Substring(field_name_idx);
            if (!FieldLookupTable_t_field_name.TryGetValue(fieldName, out member_id))
                Throw.undefined_field();
            switch (member_id)
            {
                /*FOREACH*/
                /*USE_LIST("t_member")*/
                case t_uint:
                    return TypeConverter<T>.ConvertFrom_t_field_type_2_display(accessor.t_member_name);
                    break;
                /*END*/
            }

            /* Should not reach here */
            throw new Exception("Internal error T5008");
        }
        [END]//FOREACH t_field_type


#pragma warning disable
        [MUTE]
        int foo;
        /*MUTE_END*/
    }
}
