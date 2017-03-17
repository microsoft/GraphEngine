#include "common.h"
#include <string>
#include "SyntaxNode.h"

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        string* 
Traits(
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
using Trinity.TSL;
using Trinity.TSL.Lib;
namespace )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(
{
    internal class TypeSystem
    {
        #region TypeID lookup table
        private static Dictionary<Type, uint> TypeIDLookupTable = new Dictionary<Type, uint>()
        {
            )::");
for (size_t iterator_1 = 0; iterator_1 < (Trinity::Codegen::TSLExternalParserDataTypeVector)->size();++iterator_1)
{
if (data_type_need_type_id((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1], Trinity::Codegen::TSLExternalParserDataTypeVector))
{
source->append(R"::(
            { typeof()::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::(), )::");
source->append(Codegen::GetString(iterator_1));
source->append(R"::( }
            ,)::");
}
}
source->append(R"::(
        };
        #endregion
        #region CellTypeID lookup table
        private static Dictionary<Type, uint> CellTypeIDLookupTable = new Dictionary<Type, uint>()
        {
            )::");
for (size_t iterator_1 = 0; iterator_1 < (node->cellList)->size();++iterator_1)
{
source->append(R"::(
            { typeof()::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(), )::");
source->append(Codegen::GetString(iterator_1));
source->append(R"::( }
            )::");
if (iterator_1 < (node->cellList)->size() - 1)
source->append(",");
}
source->append(R"::(
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
        )::");
for (size_t iterator_1 = 0; iterator_1 < (Trinity::Codegen::TSLExternalParserDataTypeVector)->size();++iterator_1)
{
if (data_type_need_set_field((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1], Trinity::Codegen::TSLExternalParserDataTypeVector))
{
source->append(R"::(
        T ConvertFrom_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::(()::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::( value);
        )::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::( ConvertTo_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::((T value);
        TypeConversionAction GetConversionActionTo_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::(();
        IEnumerable<)::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::(> Enumerate_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::((T value);
        )::");
}
}
source->append(R"::(
    }
    internal class TypeConverter<T> : ITypeConverter<T>
    {
        internal class _TypeConverterImpl : ITypeConverter<object>
            )::");
for (size_t iterator_1 = 0; iterator_1 < (Trinity::Codegen::TSLExternalParserDataTypeVector)->size();++iterator_1)
{
if (data_type_need_type_id((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1], Trinity::Codegen::TSLExternalParserDataTypeVector))
{
source->append(R"::(
            , ITypeConverter<)::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::(>
        )::");
}
}
source->append(R"::(
        {
            )::");
for (size_t iterator_1 = 0; iterator_1 < (Trinity::Codegen::TSLExternalParserDataTypeVector)->size();++iterator_1)
{
if (data_type_need_type_id((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1], Trinity::Codegen::TSLExternalParserDataTypeVector))
{
for (size_t iterator_2 = 0; iterator_2 < (Trinity::Codegen::TSLExternalParserDataTypeVector)->size();++iterator_2)
{
if (data_type_need_set_field((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_2], Trinity::Codegen::TSLExternalParserDataTypeVector))
{
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::( ITypeConverter<)::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::(>.ConvertFrom_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_2])));
source->append(R"::(()::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_2]));
source->append(R"::( value)
            {
                )::");
TypeConversionAction conversion_action_1 = (*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->get_type_conversion_action((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_2]);
if (conversion_action_1 == TypeConversionAction::TC_ASSIGN)
{
source->append(R"::(
                return ()::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::()value;
                )::");
}
else if (conversion_action_1 == TypeConversionAction::TC_TOSTRING)
{
source->append(R"::(
                return Serializer.ToString(value);
                )::");
}
else if (conversion_action_1 == TypeConversionAction::TC_PARSESTRING)
{
source->append(R"::(
                {
                    #region String parse
                    )::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::( intermediate_result;
                    bool conversion_success;
                    )::");
if (data_type_need_external_parser((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]))
{
source->append(R"::(
                    {
                        conversion_success = ExternalParser.TryParse_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::((value, out intermediate_result);
                    }
                    )::");
}
else
{
source->append(R"::(
                    {
                        conversion_success = )::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::(.TryParse(value, out intermediate_result);
                    }
                    )::");
}
source->append(R"::(
                    if (!conversion_success)
                    {
                        )::");
if ((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->is_list())
{
source->append(R"::(
                        try
                        {
                            )::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->listElementType));
source->append(R"::( element = TypeConverter<)::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->listElementType));
source->append(R"::(>.ConvertFrom_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_2])));
source->append(R"::((value);
                            intermediate_result = new )::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::(();
                            intermediate_result.Add(element);
                        }
                        catch
                        {
                            throw new ArgumentException("Cannot parse \"" + value + "\" into either ')::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::(' or ')::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->listElementType));
source->append(R"::('.");
                        }
                        )::");
}
else
{
source->append(R"::(
                        Throw.cannot_parse(value, ")::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::(");
                        )::");
}
source->append(R"::(
                    }
                    return intermediate_result;
                    #endregion
                }
                )::");
}
else if (conversion_action_1 == TypeConversionAction::TC_TOBOOL)
{
source->append(R"::(
                return (value != 0);
                )::");
}
else if (conversion_action_1 == TypeConversionAction::TC_CONVERTLIST)
{
source->append(R"::(
                {
                    )::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::( intermediate_result = new )::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::(();
                    foreach (var element in value)
                    {
                        intermediate_result.Add(TypeConverter<)::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->listElementType));
source->append(R"::(>.ConvertFrom_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_2]->listElementType)));
source->append(R"::((element));
                    }
                    return intermediate_result;
                }
                )::");
}
else if (conversion_action_1 == TypeConversionAction::TC_WRAPINLIST)
{
source->append(R"::(
                {
                    )::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::( intermediate_result = new )::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::(();
                    intermediate_result.Add(TypeConverter<)::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->listElementType));
source->append(R"::(>.ConvertFrom_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_2])));
source->append(R"::((value));
                    return intermediate_result;
                }
                )::");
}
else if (conversion_action_1 == TypeConversionAction::TC_ARRAYTOLIST)
{
source->append(R"::(
                return TypeConverter<)::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_2]));
source->append(R"::(>.Enumerate_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->listElementType)));
source->append(R"::((value).ToList();
                )::");
}
else if (conversion_action_1 == TypeConversionAction::TC_EXTRACTNULLABLE)
{
source->append(R"::(
                return TypeConverter<)::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::(>.ConvertFrom_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetNonNullableValueTypeString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_2])));
source->append(R"::((value.Value);
                )::");
}
else
{
source->append(R"::(
                throw new InvalidCastException("Invalid cast from ')::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_2]));
source->append(R"::(' to ')::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::('.");
                )::");
}
source->append(R"::(
            }
            )::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_2]));
source->append(R"::( ITypeConverter<)::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::(>.ConvertTo_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_2])));
source->append(R"::(()::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::( value)
            {
                return TypeConverter<)::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_2]));
source->append(R"::(>.ConvertFrom_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::((value);
            }
            TypeConversionAction ITypeConverter<)::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::(>.GetConversionActionTo_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_2])));
source->append(R"::(()
            {
                )::");
TypeConversionAction action_1 = (*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_2]->get_type_conversion_action((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]);
if (action_1 == TypeConversionAction::TC_ARRAYTOLIST)
{
source->append(R"::(
                return TypeConversionAction.TC_ARRAYTOLIST;
                )::");
}
else if (action_1 == TypeConversionAction::TC_ASSIGN)
{
source->append(R"::(
                return TypeConversionAction.TC_ASSIGN;
                )::");
}
else if (action_1 == TypeConversionAction::TC_CONVERTLIST)
{
source->append(R"::(
                return TypeConversionAction.TC_CONVERTLIST;
                )::");
}
else if (action_1 == TypeConversionAction::TC_NONCONVERTIBLE)
{
source->append(R"::(
                return TypeConversionAction.TC_NONCONVERTIBLE;
                )::");
}
else if (action_1 == TypeConversionAction::TC_PARSESTRING)
{
source->append(R"::(
                return TypeConversionAction.TC_PARSESTRING;
                )::");
}
else if (action_1 == TypeConversionAction::TC_TOBOOL)
{
source->append(R"::(
                return TypeConversionAction.TC_TOBOOL;
                )::");
}
else if (action_1 == TypeConversionAction::TC_TOSTRING)
{
source->append(R"::(
                return TypeConversionAction.TC_TOSTRING;
                )::");
}
else if (action_1 == TypeConversionAction::TC_WRAPINLIST)
{
source->append(R"::(
                return TypeConversionAction.TC_WRAPINLIST;
                )::");
}
else if (action_1 == TypeConversionAction::TC_EXTRACTNULLABLE)
{
source->append(R"::(
                return TypeConversionAction.TC_EXTRACTNULLABLE;
                )::");
}
source->append(R"::(
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<)::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_2]));
source->append(R"::(> ITypeConverter<)::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::(>.Enumerate_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_2])));
source->append(R"::(()::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::( value)
            {
                )::");
if ((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->is_list() && (*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_2]->is_convertible_from((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->listElementType))
{
source->append(R"::(
                foreach (var element in value)
                    yield return TypeConverter<)::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_2]));
source->append(R"::(>.ConvertFrom_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->listElementType)));
source->append(R"::((element);
                )::");
}
else if ((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->is_array() && (*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_2]->is_convertible_from((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.arrayElement))
{
source->append(R"::(
                {
                    )::");
for (size_t iterator_3 = 0; iterator_3 < ((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.array_dimension_size)->size();++iterator_3)
{
source->append(R"::(
                    for (int )::");
source->append(Codegen::GetString(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.arrayElement)) + '_' + GetString(iterator_3) + Discard((*((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.array_dimension_size))[iterator_3])));
source->append(R"::( = 0; )::");
source->append(Codegen::GetString(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.arrayElement)) + '_' + GetString(iterator_3) + Discard((*((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.array_dimension_size))[iterator_3])));
source->append(R"::( < )::");
source->append(Codegen::GetString((*((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.array_dimension_size))[iterator_3]));
source->append(R"::(/*_*/ ; ++)::");
source->append(Codegen::GetString(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.arrayElement)) + '_' + GetString(iterator_3) + Discard((*((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.array_dimension_size))[iterator_3])));
source->append(R"::()
                    )::");
}
source->append(R"::(
                    {
                        )::");
if ((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_2]->is_assignable_from((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.arrayElement))
{
source->append(R"::(
                        yield return (value[)::");
for (size_t iterator_3 = 0; iterator_3 < ((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.array_dimension_size)->size();++iterator_3)
{
source->append(Codegen::GetString(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.arrayElement)) + '_' + GetString(iterator_3) + Discard((*((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.array_dimension_size))[iterator_3])));
if (iterator_3 < ((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.array_dimension_size)->size() - 1)
source->append(",");
}
source->append(R"::(]);
                        )::");
}
else
{
source->append(R"::(
                        yield return (TypeConverter<)::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_2]));
source->append(R"::(>.ConvertFrom_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.arrayElement)));
source->append(R"::((value[)::");
for (size_t iterator_3 = 0; iterator_3 < ((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.array_dimension_size)->size();++iterator_3)
{
source->append(Codegen::GetString(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.arrayElement)) + '_' + GetString(iterator_3) + Discard((*((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.array_dimension_size))[iterator_3])));
if (iterator_3 < ((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.array_dimension_size)->size() - 1)
source->append(",");
}
source->append(R"::(]));
                        )::");
}
source->append(R"::(
                    }
                }
                )::");
}
source->append(R"::(
                yield break;
            }
            )::");
}
}
}
}
for (size_t iterator_1 = 0; iterator_1 < (Trinity::Codegen::TSLExternalParserDataTypeVector)->size();++iterator_1)
{
if (data_type_need_set_field((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1], Trinity::Codegen::TSLExternalParserDataTypeVector))
{
source->append(R"::(
            object ITypeConverter<object>.ConvertFrom_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::(()::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::( value)
            {
                return value;
            }
            )::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::( ITypeConverter<object>.ConvertTo_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::((object value)
            {
                throw new NotImplementedException();
            }
            TypeConversionAction ITypeConverter<object>.GetConversionActionTo_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::(()
            {
                throw new NotImplementedException();
            }
            IEnumerable<)::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::(> ITypeConverter<object>.Enumerate_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::((object value)
            {
                throw new NotImplementedException();
            }
            )::");
}
}
source->append(R"::(
        }
        internal static readonly ITypeConverter<T> s_type_converter = new _TypeConverterImpl() as ITypeConverter<T> ?? new TypeConverter<T>();
        #region Default implementation
        )::");
for (size_t iterator_1 = 0; iterator_1 < (Trinity::Codegen::TSLExternalParserDataTypeVector)->size();++iterator_1)
{
if (data_type_need_set_field((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1], Trinity::Codegen::TSLExternalParserDataTypeVector))
{
source->append(R"::(
        T ITypeConverter<T>.ConvertFrom_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::(()::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::( value)
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        )::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::( ITypeConverter<T>.ConvertTo_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::((T value)
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        TypeConversionAction ITypeConverter<T>.GetConversionActionTo_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::(()
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        IEnumerable<)::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::(> ITypeConverter<T>.Enumerate_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::((T value)
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        )::");
}
}
source->append(R"::(
        #endregion
        internal static readonly uint type_id = TypeSystem.GetTypeID(typeof(T));
        )::");
for (size_t iterator_1 = 0; iterator_1 < (Trinity::Codegen::TSLExternalParserDataTypeVector)->size();++iterator_1)
{
if (data_type_need_set_field((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1], Trinity::Codegen::TSLExternalParserDataTypeVector))
{
source->append(R"::(
        internal static T ConvertFrom_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::(()::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::( value)
        {
            return s_type_converter.ConvertFrom_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::((value);
        }
        internal static )::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::( ConvertTo_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::((T value)
        {
            return s_type_converter.ConvertTo_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::((value);
        }
        internal static TypeConversionAction GetConversionActionTo_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::(()
        {
            return s_type_converter.GetConversionActionTo_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::(();
        }
        internal static IEnumerable<)::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::(> Enumerate_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::((T value)
        {
            return s_type_converter.Enumerate_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::((value);
        }
        )::");
}
}
source->append(R"::(
    }
}
)::");

            return source;
        }
    }
}
