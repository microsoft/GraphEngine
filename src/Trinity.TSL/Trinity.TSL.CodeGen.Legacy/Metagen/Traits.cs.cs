using System.Text;
using System.Collections.Generic;

namespace Trinity.TSL.CodeTemplates
{
    internal partial class SourceFiles
    {
        internal static string 
Traits(
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
using Trinity.TSL;
using Trinity.TSL.Lib;
namespace ");
source.Append(Codegen.GetString(Trinity::Codegen::GetNamespace()));
source.Append(@"
{
    internal class TypeSystem
    {
        #region TypeID lookup table
        private static Dictionary<Type, uint> TypeIDLookupTable = new Dictionary<Type, uint>()
        {
            ");
for (int iterator_1 = 0; iterator_1 < (Trinity::Codegen::TSLExternalParserDataTypeVector).Count;++iterator_1)
{
if (data_type_need_type_id((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1], Trinity::Codegen::TSLExternalParserDataTypeVector))
{
source.Append(@"
            { typeof(");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@"), ");
source.Append(Codegen.GetString(iterator_1));
source.Append(@" }
            ,");
}
}
source.Append(@"
        };
        #endregion
        #region CellTypeID lookup table
        private static Dictionary<Type, uint> CellTypeIDLookupTable = new Dictionary<Type, uint>()
        {
            ");
for (int iterator_1 = 0; iterator_1 < (node->cellList).Count;++iterator_1)
{
source.Append(@"
            { typeof(");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"), ");
source.Append(Codegen.GetString(iterator_1));
source.Append(@" }
            ");
if (iterator_1 < (node->cellList).Count - 1)
source.Append(",");
}
source.Append(@"
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
                throw new Exception(""Type "" + t.ToString() + "" is not a cell."");
            return type_id;
        }
    }
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
        ");
for (int iterator_1 = 0; iterator_1 < (Trinity::Codegen::TSLExternalParserDataTypeVector).Count;++iterator_1)
{
if (data_type_need_set_field((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1], Trinity::Codegen::TSLExternalParserDataTypeVector))
{
source.Append(@"
        T ConvertFrom_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@"(");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@" value);
        ");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@" ConvertTo_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@"(T value);
        TypeConversionAction GetConversionActionTo_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@"();
        IEnumerable<");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@"> Enumerate_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@"(T value);
        ");
}
}
source.Append(@"
    }
    internal class TypeConverter<T> : ITypeConverter<T>
    {
        internal class _TypeConverterImpl : ITypeConverter<object>
            ");
for (int iterator_1 = 0; iterator_1 < (Trinity::Codegen::TSLExternalParserDataTypeVector).Count;++iterator_1)
{
if (data_type_need_type_id((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1], Trinity::Codegen::TSLExternalParserDataTypeVector))
{
source.Append(@"
            , ITypeConverter<");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@">
        ");
}
}
source.Append(@"
        {
            ");
for (int iterator_1 = 0; iterator_1 < (Trinity::Codegen::TSLExternalParserDataTypeVector).Count;++iterator_1)
{
if (data_type_need_type_id((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1], Trinity::Codegen::TSLExternalParserDataTypeVector))
{
for (int iterator_2 = 0; iterator_2 < (Trinity::Codegen::TSLExternalParserDataTypeVector).Count;++iterator_2)
{
if (data_type_need_set_field((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_2], Trinity::Codegen::TSLExternalParserDataTypeVector))
{
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@" ITypeConverter<");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@">.ConvertFrom_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_2])));
source.Append(@"(");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_2]));
source.Append(@" value)
            {
                ");
TypeConversionAction conversion_action_1 = (Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->get_type_conversion_action((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_2]);
if (conversion_action_1 == TypeConversionAction::TC_ASSIGN)
{
source.Append(@"
                return (");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@")value;
                ");
}
else if (conversion_action_1 == TypeConversionAction::TC_TOSTRING)
{
source.Append(@"
                return Serializer.ToString(value);
                ");
}
else if (conversion_action_1 == TypeConversionAction::TC_PARSESTRING)
{
source.Append(@"
                {
                    #region String parse
                    ");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@" intermediate_result;
                    bool conversion_success;
                    ");
if (data_type_need_external_parser((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]))
{
source.Append(@"
                    {
                        conversion_success = ExternalParser.TryParse_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@"(value, out intermediate_result);
                    }
                    ");
}
else
{
source.Append(@"
                    {
                        conversion_success = ");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@".TryParse(value, out intermediate_result);
                    }
                    ");
}
source.Append(@"
                    if (!conversion_success)
                    {
                        ");
if ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->is_list())
{
source.Append(@"
                        try
                        {
                            ");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].listElementType));
source.Append(@" element = TypeConverter<");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].listElementType));
source.Append(@">.ConvertFrom_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_2])));
source.Append(@"(value);
                            intermediate_result = new ");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@"();
                            intermediate_result.Add(element);
                        }
                        catch
                        {
                            throw new ArgumentException(""Cannot parse \"""" + value + ""\"" into either '");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@"' or '");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].listElementType));
source.Append(@"'."");
                        }
                        ");
}
else
{
source.Append(@"
                        Throw.cannot_parse(value, """);
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@""");
                        ");
}
source.Append(@"
                    }
                    return intermediate_result;
                    #endregion
                }
                ");
}
else if (conversion_action_1 == TypeConversionAction::TC_TOBOOL)
{
source.Append(@"
                return (value != 0);
                ");
}
else if (conversion_action_1 == TypeConversionAction::TC_CONVERTLIST)
{
source.Append(@"
                {
                    ");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@" intermediate_result = new ");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@"();
                    foreach (var element in value)
                    {
                        intermediate_result.Add(TypeConverter<");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].listElementType));
source.Append(@">.ConvertFrom_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_2]->listElementType)));
source.Append(@"(element));
                    }
                    return intermediate_result;
                }
                ");
}
else if (conversion_action_1 == TypeConversionAction::TC_WRAPINLIST)
{
source.Append(@"
                {
                    ");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@" intermediate_result = new ");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@"();
                    intermediate_result.Add(TypeConverter<");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].listElementType));
source.Append(@">.ConvertFrom_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_2])));
source.Append(@"(value));
                    return intermediate_result;
                }
                ");
}
else if (conversion_action_1 == TypeConversionAction::TC_ARRAYTOLIST)
{
source.Append(@"
                return TypeConverter<");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_2]));
source.Append(@">.Enumerate_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->listElementType)));
source.Append(@"(value).ToList();
                ");
}
else if (conversion_action_1 == TypeConversionAction::TC_EXTRACTNULLABLE)
{
source.Append(@"
                return TypeConverter<");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@">.ConvertFrom_");
source.Append(Codegen.GetString(Trinity::Codegen::GetNonNullableValueTypeString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_2])));
source.Append(@"(value.Value);
                ");
}
else
{
source.Append(@"
                throw new InvalidCastException(""Invalid cast from '");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_2]));
source.Append(@"' to '");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@"'."");
                ");
}
source.Append(@"
            }
            ");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_2]));
source.Append(@" ITypeConverter<");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@">.ConvertTo_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_2])));
source.Append(@"(");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@" value)
            {
                return TypeConverter<");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_2]));
source.Append(@">.ConvertFrom_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@"(value);
            }
            TypeConversionAction ITypeConverter<");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@">.GetConversionActionTo_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_2])));
source.Append(@"()
            {
                ");
TypeConversionAction action_1 = (Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_2]->get_type_conversion_action((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]);
if (action_1 == TypeConversionAction::TC_ARRAYTOLIST)
{
source.Append(@"
                return TypeConversionAction.TC_ARRAYTOLIST;
                ");
}
else if (action_1 == TypeConversionAction::TC_ASSIGN)
{
source.Append(@"
                return TypeConversionAction.TC_ASSIGN;
                ");
}
else if (action_1 == TypeConversionAction::TC_CONVERTLIST)
{
source.Append(@"
                return TypeConversionAction.TC_CONVERTLIST;
                ");
}
else if (action_1 == TypeConversionAction::TC_NONCONVERTIBLE)
{
source.Append(@"
                return TypeConversionAction.TC_NONCONVERTIBLE;
                ");
}
else if (action_1 == TypeConversionAction::TC_PARSESTRING)
{
source.Append(@"
                return TypeConversionAction.TC_PARSESTRING;
                ");
}
else if (action_1 == TypeConversionAction::TC_TOBOOL)
{
source.Append(@"
                return TypeConversionAction.TC_TOBOOL;
                ");
}
else if (action_1 == TypeConversionAction::TC_TOSTRING)
{
source.Append(@"
                return TypeConversionAction.TC_TOSTRING;
                ");
}
else if (action_1 == TypeConversionAction::TC_WRAPINLIST)
{
source.Append(@"
                return TypeConversionAction.TC_WRAPINLIST;
                ");
}
else if (action_1 == TypeConversionAction::TC_EXTRACTNULLABLE)
{
source.Append(@"
                return TypeConversionAction.TC_EXTRACTNULLABLE;
                ");
}
source.Append(@"
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_2]));
source.Append(@"> ITypeConverter<");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@">.Enumerate_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_2])));
source.Append(@"(");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@" value)
            {
                ");
if ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->is_list() && (Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_2]->is_convertible_from((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->listElementType))
{
source.Append(@"
                foreach (var element in value)
                    yield return TypeConverter<");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_2]));
source.Append(@">.ConvertFrom_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->listElementType)));
source.Append(@"(element);
                ");
}
else if ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->is_array() && (Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_2]->is_convertible_from((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->arrayInfo.arrayElement))
{
source.Append(@"
                {
                    ");
for (int iterator_3 = 0; iterator_3 < ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.array_dimension_size).Count;++iterator_3)
{
source.Append(@"
                    for (int ");
source.Append(Codegen.GetString(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->arrayInfo.arrayElement)) + '_' + GetString(iterator_3) + Discard(((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.array_dimension_size)[iterator_3])));
source.Append(@" = 0; ");
source.Append(Codegen.GetString(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->arrayInfo.arrayElement)) + '_' + GetString(iterator_3) + Discard(((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.array_dimension_size)[iterator_3])));
source.Append(@" < ");
source.Append(Codegen.GetString(((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.array_dimension_size)[iterator_3]));
source.Append(@"/*_*/ ; ++");
source.Append(Codegen.GetString(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->arrayInfo.arrayElement)) + '_' + GetString(iterator_3) + Discard(((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.array_dimension_size)[iterator_3])));
source.Append(@")
                    ");
}
source.Append(@"
                    {
                        ");
if ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_2]->is_assignable_from((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->arrayInfo.arrayElement))
{
source.Append(@"
                        yield return (value[");
for (int iterator_3 = 0; iterator_3 < ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.array_dimension_size).Count;++iterator_3)
{
source.Append(Codegen.GetString(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->arrayInfo.arrayElement)) + '_' + GetString(iterator_3) + Discard(((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.array_dimension_size)[iterator_3])));
if (iterator_3 < ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.array_dimension_size).Count - 1)
source.Append(",");
}
source.Append(@"]);
                        ");
}
else
{
source.Append(@"
                        yield return (TypeConverter<");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_2]));
source.Append(@">.ConvertFrom_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->arrayInfo.arrayElement)));
source.Append(@"(value[");
for (int iterator_3 = 0; iterator_3 < ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.array_dimension_size).Count;++iterator_3)
{
source.Append(Codegen.GetString(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->arrayInfo.arrayElement)) + '_' + GetString(iterator_3) + Discard(((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.array_dimension_size)[iterator_3])));
if (iterator_3 < ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.array_dimension_size).Count - 1)
source.Append(",");
}
source.Append(@"]));
                        ");
}
source.Append(@"
                    }
                }
                ");
}
source.Append(@"
                yield break;
            }
            ");
}
}
}
}
for (int iterator_1 = 0; iterator_1 < (Trinity::Codegen::TSLExternalParserDataTypeVector).Count;++iterator_1)
{
if (data_type_need_set_field((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1], Trinity::Codegen::TSLExternalParserDataTypeVector))
{
source.Append(@"
            object ITypeConverter<object>.ConvertFrom_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@"(");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@" value)
            {
                return value;
            }
            ");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@" ITypeConverter<object>.ConvertTo_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@"(object value)
            {
                throw new NotImplementedException();
            }
            TypeConversionAction ITypeConverter<object>.GetConversionActionTo_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@"()
            {
                throw new NotImplementedException();
            }
            IEnumerable<");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@"> ITypeConverter<object>.Enumerate_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@"(object value)
            {
                throw new NotImplementedException();
            }
            ");
}
}
source.Append(@"
        }
        internal static readonly ITypeConverter<T> s_type_converter = new _TypeConverterImpl() as ITypeConverter<T> ?? new TypeConverter<T>();
        #region Default implementation
        ");
for (int iterator_1 = 0; iterator_1 < (Trinity::Codegen::TSLExternalParserDataTypeVector).Count;++iterator_1)
{
if (data_type_need_set_field((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1], Trinity::Codegen::TSLExternalParserDataTypeVector))
{
source.Append(@"
        T ITypeConverter<T>.ConvertFrom_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@"(");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@" value)
        {
            throw new NotImplementedException(""Internal error T5013."");
        }
        ");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@" ITypeConverter<T>.ConvertTo_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@"(T value)
        {
            throw new NotImplementedException(""Internal error T5013."");
        }
        TypeConversionAction ITypeConverter<T>.GetConversionActionTo_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@"()
        {
            throw new NotImplementedException(""Internal error T5013."");
        }
        IEnumerable<");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@"> ITypeConverter<T>.Enumerate_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@"(T value)
        {
            throw new NotImplementedException(""Internal error T5013."");
        }
        ");
}
}
source.Append(@"
        #endregion
        internal static readonly uint type_id = TypeSystem.GetTypeID(typeof(T));
        ");
for (int iterator_1 = 0; iterator_1 < (Trinity::Codegen::TSLExternalParserDataTypeVector).Count;++iterator_1)
{
if (data_type_need_set_field((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1], Trinity::Codegen::TSLExternalParserDataTypeVector))
{
source.Append(@"
        internal static T ConvertFrom_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@"(");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@" value)
        {
            return s_type_converter.ConvertFrom_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@"(value);
        }
        internal static ");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@" ConvertTo_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@"(T value)
        {
            return s_type_converter.ConvertTo_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@"(value);
        }
        internal static TypeConversionAction GetConversionActionTo_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@"()
        {
            return s_type_converter.GetConversionActionTo_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@"();
        }
        internal static IEnumerable<");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@"> Enumerate_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@"(T value)
        {
            return s_type_converter.Enumerate_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@"(value);
        }
        ");
}
}
source.Append(@"
    }
}
");

            return source.ToString();
        }
    }
}
