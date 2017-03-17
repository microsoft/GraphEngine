#include "common.h"
#include <string>
#include "SyntaxNode.h"

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        string* 
ExternalParser(
NTSL* node)
        {
            string* source = new string();
            
source->append(R"::(
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
namespace )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(
{
    internal class ExternalParser
    {
        )::");
for (size_t iterator_1 = 0; iterator_1 < (Trinity::Codegen::TSLExternalParserDataTypeVector)->size();++iterator_1)
{
if (data_type_need_external_parser((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]) && data_type_need_set_field((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1], Trinity::Codegen::TSLExternalParserDataTypeVector))
{
source->append(R"::(
        internal static unsafe bool TryParse_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::((string s, out )::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::( value)
        {
            )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNonNullableValueTypeString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::( value_type_value;
            JArray jarray;
            )::");
if ((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->is_bool())
{
source->append(R"::(
            {
                )::");
if ((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->is_nullable())
{
source->append(R"::(
                {
                    if (string.IsNullOrEmpty(s) || string.Compare(s, "null", ignoreCase: true) == 0)
                    {
                        value = default()::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::();
                        return true;
                    }
                }
                )::");
}
source->append(R"::(
                double double_val;
                if ()::");
source->append(Codegen::GetString(Trinity::Codegen::GetNonNullableValueTypeString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::(.TryParse(s, out value_type_value))
                {
                    value = value_type_value;
                    return true;
                }
                else if (double.TryParse(s, out double_val))
                {
                    value = (double_val != 0);
                    return true;
                }
                else
                {
                    value = default()::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::();
                    return false;
                }
            }
            )::");
}
else if ((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->is_datetime())
{
source->append(R"::(
            {
                )::");
if ((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->is_nullable())
{
source->append(R"::(
                {
                    if (string.IsNullOrEmpty(s) || string.Compare(s, "null", ignoreCase: true) == 0)
                    {
                        value = default()::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::();
                        return true;
                    }
                }
                )::");
}
source->append(R"::(
                if ()::");
source->append(Codegen::GetString(Trinity::Codegen::GetNonNullableValueTypeString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::(.TryParse(s, null, System.Globalization.DateTimeStyles.RoundtripKind, out value_type_value))
                {
                    value = value_type_value;
                    return true;
                }
                if (s.EndsWith(" UTC", StringComparison.Ordinal) && )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNonNullableValueTypeString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::(.TryParse(s.Substring(0, s.Length - 4) + 'Z', null, System.Globalization.DateTimeStyles.RoundtripKind, out value_type_value))
                {
                    value = value_type_value;
                    return true;
                }
                value  = default()::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::();
                return false;
            }
            )::");
}
else if ((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->is_nullable() /* and has built-in TryParse */)
{
source->append(R"::(
            if (string.IsNullOrEmpty(s) || string.Compare(s, "null", ignoreCase: true) == 0)
            {
                value = default()::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::();
                return true;
            }
            else if ()::");
source->append(Codegen::GetString(Trinity::Codegen::GetNonNullableValueTypeString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::(.TryParse(s, out value_type_value))
            {
                value = value_type_value;
                return true;
            }
            else
            {
                value = default()::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::();
                return false;
            }
            )::");
}
else if ((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->is_array())
{
source->append(R"::(
            try
            {
                jarray = JArray.Parse(s);
                value = new )::");
source->append(Codegen::GetString(data_type_get_array_type_with_size_string((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::(;)::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.arrayElement));
source->append(R"::( element;
                int )::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.arrayElement)));
source->append(R"::(_offset = 0;
                )::");
for (size_t iterator_2 = 0; iterator_2 < ((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.array_dimension_size)->size();++iterator_2)
{
source->append(R"::(
                for (int )::");
source->append(Codegen::GetString(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.arrayElement)) + '_' + GetString(iterator_2) + Discard((*((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.array_dimension_size))[iterator_2])));
source->append(R"::( = 0; )::");
source->append(Codegen::GetString(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.arrayElement)) + '_' + GetString(iterator_2) + Discard((*((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.array_dimension_size))[iterator_2])));
source->append(R"::( < )::");
source->append(Codegen::GetString((*((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.array_dimension_size))[iterator_2]));
source->append(R"::(/*_*/ ; ++)::");
source->append(Codegen::GetString(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.arrayElement)) + '_' + GetString(iterator_2) + Discard((*((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.array_dimension_size))[iterator_2])));
source->append(R"::()
                )::");
}
source->append(R"::(
                {
                    )::");
if (data_type_need_external_parser((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.arrayElement))
{
source->append(R"::(
                    {
                        if (!ExternalParser.TryParse_)::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.arrayElement));
source->append(R"::(((string)jarray[)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.arrayElement)));
source->append(R"::(_offset++], out element))
                            continue;
                        value[)::");
for (size_t iterator_2 = 0; iterator_2 < ((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.array_dimension_size)->size();++iterator_2)
{
source->append(Codegen::GetString(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.arrayElement)) + '_' + GetString(iterator_2) + Discard((*((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.array_dimension_size))[iterator_2])));
if (iterator_2 < ((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.array_dimension_size)->size() - 1)
source->append(",");
}
source->append(R"::(] = element;
                    }
                    )::");
}
else if ((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.arrayElement->is_string())
{
source->append(R"::(
                    {
                        value[)::");
for (size_t iterator_2 = 0; iterator_2 < ((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.array_dimension_size)->size();++iterator_2)
{
source->append(Codegen::GetString(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.arrayElement)) + '_' + GetString(iterator_2) + Discard((*((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.array_dimension_size))[iterator_2])));
if (iterator_2 < ((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.array_dimension_size)->size() - 1)
source->append(",");
}
source->append(R"::(] = (string)jarray[)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.arrayElement)));
source->append(R"::(_offset++];
                    }
                    )::");
}
else
{
source->append(R"::(
                    {
                        if (!)::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.arrayElement));
source->append(R"::(.TryParse((string)jarray[)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.arrayElement)));
source->append(R"::(_offset++], out element))
                            continue;
                        value[)::");
for (size_t iterator_2 = 0; iterator_2 < ((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.array_dimension_size)->size();++iterator_2)
{
source->append(Codegen::GetString(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.arrayElement)) + '_' + GetString(iterator_2) + Discard((*((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.array_dimension_size))[iterator_2])));
if (iterator_2 < ((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->arrayInfo.array_dimension_size)->size() - 1)
source->append(",");
}
source->append(R"::(] = element;
                    }
                    )::");
}
source->append(R"::(
                }
                return true;
            }
            catch
            {
                value = default()::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::();
                return false;
            }
            )::");
}
else if ((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->is_list())
{
source->append(R"::(
            try
            {
                value = new )::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::(();
                jarray = JArray.Parse(s);
                foreach (var jarray_element in jarray)
                {
                    )::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->listElementType));
source->append(R"::( element;
                    )::");
if (data_type_need_external_parser((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->listElementType))
{
source->append(R"::(
                    if (!ExternalParser.TryParse_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->listElementType)));
source->append(R"::(((string)jarray_element, out element))
                    {
                        continue;
                    }
                    value.Add(element);
                    )::");
}
else if ((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->listElementType->is_string())
{
source->append(R"::(
                    value.Add((string)jarray_element);
                    )::");
}
else
{
source->append(R"::(
                    if (!)::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]->listElementType));
source->append(R"::(.TryParse((string)jarray_element, out element))
                    {
                        continue;
                    }
                    value.Add(element);
                    )::");
}
source->append(R"::(
                }
                return true;
            }
            catch
            {
                value = default()::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::();
                return false;
            }
            )::");
}
source->append(R"::(
        }
        )::");
}
}
source->append(R"::(
        #region Mute
        
        #endregion
    }
}
)::");

            return source;
        }
    }
}
