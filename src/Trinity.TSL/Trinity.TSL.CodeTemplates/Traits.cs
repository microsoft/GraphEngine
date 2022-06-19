#pragma warning disable 0162 // disable the "unreachable code" warning
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL;
using Trinity.TSL.Lib;

/*MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")*/
namespace t_Namespace
{
    [TARGET("NTSL")]
    [MAP_LIST("t_field_type", "Trinity::Codegen::TSLExternalParserDataTypeVector")]
    [MAP_VAR("t_field_type", "")]
    [MAP_VAR("t_field_type_remove_nullable", "Trinity::Codegen::GetNonNullableValueTypeString($$)")]
    [MAP_VAR("t_field_type_element_type", "listElementType")]
    [MAP_VAR("t_field_type_element_type_display", "Trinity::Codegen::GetDataTypeDisplayString($$->listElementType)")]
    [MAP_VAR("t_field_type_display", "Trinity::Codegen::GetDataTypeDisplayString($$)")]
    [MAP_LIST("t_field_type_2", "Trinity::Codegen::TSLExternalParserDataTypeVector")]
    [MAP_VAR("t_field_type_2", "")]
    [MAP_VAR("t_field_type_2_display", "Trinity::Codegen::GetDataTypeDisplayString($$)")]
    [MAP_VAR("t_field_type_2_element_type", "listElementType")]
    [MAP_VAR("t_field_type_2_element_type_display", "Trinity::Codegen::GetDataTypeDisplayString($$->listElementType)")]
    [MAP_LIST("t_array_dimension_list", "arrayInfo.array_dimension_size", MemberOf = "t_field_type")]
    [MAP_VAR("t_array_dimension_list", "")]
    [MAP_LIST("t_cell", "node->cellList")]
    [MAP_VAR("t_cell_name", "name")]
    internal class TypeSystem : __meta
    {
        #region TypeID lookup table
        private static Dictionary<Type, uint> TypeIDLookupTable = new Dictionary<Type, uint>()
        {
            /*FOREACH*/
            /*IF("data_type_need_type_id($t_field_type, Trinity::Codegen::TSLExternalParserDataTypeVector)")*/
            { typeof(t_field_type), GET_ITERATOR_VALUE() }
            /*LITERAL_OUTPUT(",")*/
            /*END*/
            /*END*/
        };
        #endregion

        #region CellTypeID lookup table
        private static Dictionary<Type, uint> CellTypeIDLookupTable = new Dictionary<Type, uint>()
        {
            /*FOREACH(",")*/
            { typeof(t_cell_name), GET_ITERATOR_VALUE() }
            /*END*/
        };
        #endregion

        internal static uint GetTypeID(Type t)
        {
            uint type_id;
            if (!TypeIDLookupTable.TryGetValue(t, out type_id))
                type_id = uint.MaxValue;
            return type_id;
        }

        internal static uint GetCellTypeID(Type t)
        {
            uint type_id;
            if (!CellTypeIDLookupTable.TryGetValue(t, out type_id))
                throw new Exception("Type " + t.ToString() + " is not a cell.");
            return type_id;
        }
    }

    //Should be synchronized with TypeConversionAction in SyntaxNode.h
    internal enum TypeConversionAction
    {
        TC_NONCONVERTIBLE = 0,
        TC_ASSIGN,
        TC_TOSTRING,
        TC_PARSESTRING,
        TC_TOBOOL,
        TC_CONVERTLIST,
        TC_WRAPINLIST,
        TC_ARRAYTOLIST,
        TC_EXTRACTNULLABLE,
    }

    internal interface ITypeConverter<T>
    {
        [FOREACH]
        [IF("data_type_need_set_field($t_field_type, Trinity::Codegen::TSLExternalParserDataTypeVector)")]
        T ConvertFrom_t_field_type_display(t_field_type value);
        t_field_type ConvertTo_t_field_type_display(T value);
        TypeConversionAction GetConversionActionTo_t_field_type_display();
        IEnumerable<t_field_type> Enumerate_t_field_type_display(T value);
        /*END*/
        /*END*/
    }

    internal class TypeConverter<T> : __meta, ITypeConverter<T>
    {
        internal class _TypeConverterImpl : __meta,
            ITypeConverter<object>
            /*FOREACH()*/
            /*  IF("data_type_need_type_id($t_field_type_2, Trinity::Codegen::TSLExternalParserDataTypeVector)")*/
            , ITypeConverter<t_field_type_2>
        /*      END*/
        /*    END*/
        {
            [FOREACH]
            [USE_LIST("t_field_type_2")]//t_field_type_2 as target(T)
            [IF("data_type_need_type_id($t_field_type_2, Trinity::Codegen::TSLExternalParserDataTypeVector)")]
            [FOREACH]
            [USE_LIST("t_field_type")]//t_field_type as source
            [IF("data_type_need_set_field($t_field_type, Trinity::Codegen::TSLExternalParserDataTypeVector)")]
            t_field_type_2 ITypeConverter<t_field_type_2>.ConvertFrom_t_field_type_display(t_field_type value)
            {
                META_VAR("TypeConversionAction", "conversion_action", "$t_field_type_2->get_type_conversion_action($t_field_type)");
                IF("%conversion_action == TypeConversionAction::TC_ASSIGN");
                return (t_field_type_2)value;
                ELIF("%conversion_action == TypeConversionAction::TC_TOSTRING");
                return Serializer.ToString(value);
                ELIF("%conversion_action == TypeConversionAction::TC_PARSESTRING");
                {
                    #region String parse
                    t_field_type_2 intermediate_result;
                    bool conversion_success;
                    IF("data_type_need_external_parser($t_field_type_2)");
                    {
                        conversion_success = ExternalParser.TryParse_t_field_type_2_display(value, out intermediate_result);
                    }
                    ELSE();
                    {
                        conversion_success = t_field_type_2.TryParse(value, out intermediate_result);
                    }
                    END();
                    if (!conversion_success)
                    {
                        IF("$t_field_type_2->is_list()");
                        try
                        {
                            t_field_type_2_element_type element = TypeConverter<t_field_type_2_element_type>.ConvertFrom_t_field_type_display(value);
                            intermediate_result = new t_field_type_2();
                            intermediate_result.Add(element);
                        }
                        catch
                        {
                            throw new ArgumentException("Cannot parse \"" + value + "\" into either 't_field_type_2' or 't_field_type_2_element_type'.");
                        }
                        ELSE();
                        Throw.cannot_parse(value, "t_field_type_2");
                        END();
                    }
                    return intermediate_result;
                    #endregion
                }
                ELIF("%conversion_action == TypeConversionAction::TC_TOBOOL");
                return (value != 0);
                ELIF("%conversion_action == TypeConversionAction::TC_CONVERTLIST");
                {
                    t_field_type_2 intermediate_result = new t_field_type_2();
                    foreach (var element in value)
                    {
                        intermediate_result.Add(TypeConverter<t_field_type_2_element_type>.ConvertFrom_t_field_type_element_type_display(element));
                    }
                    return intermediate_result;
                }
                ELIF("%conversion_action == TypeConversionAction::TC_WRAPINLIST");
                {
                    t_field_type_2 intermediate_result = new t_field_type_2();
                    intermediate_result.Add(TypeConverter<t_field_type_2_element_type>.ConvertFrom_t_field_type_display(value));
                    return intermediate_result;
                }
                ELIF("%conversion_action == TypeConversionAction::TC_ARRAYTOLIST");
                return TypeConverter<t_field_type>.Enumerate_t_field_type_2_element_type_display(value).ToList();
                ELIF("%conversion_action == TypeConversionAction::TC_EXTRACTNULLABLE");
                return TypeConverter<t_field_type_2>.ConvertFrom_t_field_type_remove_nullable(value.Value);
                ELSE();
                throw new InvalidCastException("Invalid cast from '[META_OUTPUT($t_field_type)]' to '[META_OUTPUT($t_field_type_2)]'.");
                END();
            }
            t_field_type ITypeConverter<t_field_type_2>.ConvertTo_t_field_type_display(t_field_type_2 value)
            {
                return TypeConverter<t_field_type>.ConvertFrom_t_field_type_2_display(value);
            }
            TypeConversionAction ITypeConverter<t_field_type_2>.GetConversionActionTo_t_field_type_display()
            {
                META_VAR("TypeConversionAction", "action", "$t_field_type->get_type_conversion_action($t_field_type_2)");
                IF("%action == TypeConversionAction::TC_ARRAYTOLIST");
                return TypeConversionAction.TC_ARRAYTOLIST;
                ELIF("%action == TypeConversionAction::TC_ASSIGN");
                return TypeConversionAction.TC_ASSIGN;
                ELIF("%action == TypeConversionAction::TC_CONVERTLIST");
                return TypeConversionAction.TC_CONVERTLIST;
                ELIF("%action == TypeConversionAction::TC_NONCONVERTIBLE");
                return TypeConversionAction.TC_NONCONVERTIBLE;
                ELIF("%action == TypeConversionAction::TC_PARSESTRING");
                return TypeConversionAction.TC_PARSESTRING;
                ELIF("%action == TypeConversionAction::TC_TOBOOL");
                return TypeConversionAction.TC_TOBOOL;
                ELIF("%action == TypeConversionAction::TC_TOSTRING");
                return TypeConversionAction.TC_TOSTRING;
                ELIF("%action == TypeConversionAction::TC_WRAPINLIST");
                return TypeConversionAction.TC_WRAPINLIST;
                ELIF("%action == TypeConversionAction::TC_EXTRACTNULLABLE");
                return TypeConversionAction.TC_EXTRACTNULLABLE;
                END();
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<t_field_type> ITypeConverter<t_field_type_2>.Enumerate_t_field_type_display(t_field_type_2 value)
            {
                //META_VAR("TypeConversionAction", "action", "$t_field_type->get_type_conversion_action($t_field_type_2)");
                IF("$t_field_type_2->is_list() && $t_field_type->is_convertible_from($t_field_type_2->listElementType)");
                foreach (var element in value)
                    yield return TypeConverter<t_field_type>.ConvertFrom_t_field_type_2_element_type_display(element);
                ELIF("$t_field_type_2->is_array() && $t_field_type->is_convertible_from($t_field_type_2->arrayInfo.arrayElement)");
                {
                    MUTE();
                    int t_array_iterator_length = 1;
                    MUTE_END();
                    MAP_VAR("t_field_type_2_array_element_type_display", "Trinity::Codegen::GetDataTypeDisplayString($$->arrayInfo.arrayElement)", MemberOf = "t_field_type_2");
                    MAP_LIST("t_array_iterator_list", "arrayInfo.array_dimension_size", MemberOf = "t_field_type_2");
                    MAP_VAR("t_array_iterator_prefix", "Trinity::Codegen::GetDataTypeDisplayString($$->arrayInfo.arrayElement)", MemberOf = "t_field_type_2");
                    MAP_VAR("t_array_iterator_length", "", MemberOf = "t_array_iterator_list");
                    MAP_VAR("t_array_iterator_string", "t_array_iterator_prefix + '_' + GetString(GET_ITERATOR_VALUE()) + Discard($$)", MemberOf = "t_array_iterator_list");

                    FOREACH();
                    for (int t_array_iterator_string = 0; t_array_iterator_string < t_array_iterator_length; ++t_array_iterator_string)
                    /*END*/
                    {
                        IF("$t_field_type->is_assignable_from($t_field_type_2->arrayInfo.arrayElement)");
                        yield return (value[/*FOREACH(",")*/t_array_iterator_string/*END*/]);
                        ELSE();
                        yield return (TypeConverter<t_field_type>.ConvertFrom_t_field_type_2_array_element_type_display(value[/*FOREACH(",")*/t_array_iterator_string/*END*/]));
                        END();
                    }
                }
                END();
                yield break;
            }
            [END]//IF
            [END]// FOREACH convert source
            [END]//IF
            [END]// FOREACH convert target (T)


            [FOREACH]
            [USE_LIST("t_field_type")]
            [IF("data_type_need_set_field($t_field_type, Trinity::Codegen::TSLExternalParserDataTypeVector)")]
            object ITypeConverter<object>.ConvertFrom_t_field_type_display(t_field_type value)
            {
                return value;
            }
            t_field_type ITypeConverter<object>.ConvertTo_t_field_type_display(object value)
            {
                throw new NotImplementedException();
            }

            TypeConversionAction ITypeConverter<object>.GetConversionActionTo_t_field_type_display()
            {
                throw new NotImplementedException();
            }

            IEnumerable<t_field_type> ITypeConverter<object>.Enumerate_t_field_type_display(object value)
            {
                throw new NotImplementedException();
            }
            /*END*/
            /*END*/
        }

        internal static readonly ITypeConverter<T> s_type_converter = new _TypeConverterImpl() as ITypeConverter<T> ?? new TypeConverter<T>();

        #region Default implementation
        [FOREACH]
        [IF("data_type_need_set_field($t_field_type, Trinity::Codegen::TSLExternalParserDataTypeVector)")]
        T ITypeConverter<T>.ConvertFrom_t_field_type_display(t_field_type value)
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        t_field_type ITypeConverter<T>.ConvertTo_t_field_type_display(T value)
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        TypeConversionAction ITypeConverter<T>.GetConversionActionTo_t_field_type_display()
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        IEnumerable<t_field_type> ITypeConverter<T>.Enumerate_t_field_type_display(T value)
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        /*  END*/
        /*END*/
        #endregion

        internal static readonly uint type_id = TypeSystem.GetTypeID(typeof(T));
        [FOREACH]
        [IF("data_type_need_set_field($t_field_type, Trinity::Codegen::TSLExternalParserDataTypeVector)")]
        internal static T ConvertFrom_t_field_type_display(t_field_type value)
        {
            return s_type_converter.ConvertFrom_t_field_type_display(value);
        }

        internal static t_field_type ConvertTo_t_field_type_display(T value)
        {
            return s_type_converter.ConvertTo_t_field_type_display(value);
        }

        internal static TypeConversionAction GetConversionActionTo_t_field_type_display()
        {
            return s_type_converter.GetConversionActionTo_t_field_type_display();
        }

        internal static IEnumerable<t_field_type> Enumerate_t_field_type_display(T value)
        {
            return s_type_converter.Enumerate_t_field_type_display(value);
        }
        [END]
        [END]

        [MUTE]
        internal static unsafe T ConvertFrom_t_field_type_2_display(t_field_type t_field_type)
        {
            throw new NotImplementedException();
        }

        private static t_field_type_2_element_type ConvertFrom_t_field_type_element_type_display(t_field_type_list_element_type element)
        {
            throw new NotImplementedException();
        }

        internal static t_data_type ConvertFrom_t_data_type_display(t_data_type value)
        {
            throw new NotImplementedException();
        }
        internal static t_data_type ConvertFrom_t_data_type_list_element_type_display(t_data_type_list_element_type element)
        {
            throw new NotImplementedException();
        }
        internal static t_data_type ConvertFrom_t_data_type_array_element_type_display(string p)
        {
            throw new NotImplementedException();
        }
        internal static unsafe T ConvertFrom_t_field_type_remove_nullable(t_field_type t_field_type)
        {
            throw new NotImplementedException();
        }
        internal static unsafe t_field_type_2 ConvertTo_t_field_type_2_display(object value)
        {
            throw new NotImplementedException();
        }
        internal static unsafe string ConvertTo_string(object value)
        {
            throw new NotImplementedException();
        }
        internal static t_data_type ConvertTo_t_field_type_list_element_type(object value)
        {
            throw new NotImplementedException();
        }
        internal static t_data_type Enumerate_t_field_type_list_element_type(object value)
        {
            throw new NotImplementedException();
        }
        internal static unsafe t_field_type ConvertFrom_t_field_type_2_array_element_type_display(t_field_type t_field_type)
        {
            throw new NotImplementedException();
        }
        internal static t_field_type_2_element_type ConvertFrom_t_field_type_element_type_display(object element)
        {
            throw new NotImplementedException();
        }
        internal static unsafe t_data_type Enumerate_t_field_type_2_element_type_display(t_field_type value)
        {
            throw new NotImplementedException();
        }
        internal static unsafe t_field_type ConvertFrom_t_field_type_2_element_type_display(object element)
        {
            throw new NotImplementedException();
        }
        internal static IEnumerable<object> Enumerate_t_field_type_element_type_display(object value)
        {
            throw new NotImplementedException();
        }
        internal static IEnumerable<object> Enumerate_t_field_type_list_element_type_display(object value)
        {
            throw new NotImplementedException();
        }

        internal static object ConvertTo_t_field_type_list_element_type_display(object value)
        {
            throw new NotImplementedException();
        }
        /*MUTE_END*/
    }
}
