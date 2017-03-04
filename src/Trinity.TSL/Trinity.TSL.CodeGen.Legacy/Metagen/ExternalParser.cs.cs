using System.Text;
using System.Collections.Generic;

namespace Trinity.TSL.CodeTemplates
{
    internal partial class SourceFiles
    {
        internal static string 
ExternalParser(
NTSL node)
        {
            StringBuilder source = new StringBuilder();
            
source.Append(@"
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
namespace ");
source.Append(Codegen.GetString(Trinity::Codegen::GetNamespace()));
source.Append(@"
{
    internal class ExternalParser
    {
        ");
for (int iterator_1 = 0; iterator_1 < (Trinity::Codegen::TSLExternalParserDataTypeVector).Count;++iterator_1)
{
if (data_type_need_external_parser((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]) && data_type_need_set_field((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1], Trinity::Codegen::TSLExternalParserDataTypeVector))
{
source.Append(@"
        internal static unsafe bool TryParse_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@"(string s, out ");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@" value)
        {
            ");
source.Append(Codegen.GetString(Trinity::Codegen::GetNonNullableValueTypeString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@" value_type_value;
            JArray jarray;
            ");
if ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->is_bool())
{
source.Append(@"
            {
                ");
if ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->is_nullable())
{
source.Append(@"
                {
                    if (string.IsNullOrEmpty(s) || string.Compare(s, ""null"", ignoreCase: true) == 0)
                    {
                        value = default(");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@");
                        return true;
                    }
                }
                ");
}
source.Append(@"
                double double_val;
                if (");
source.Append(Codegen.GetString(Trinity::Codegen::GetNonNullableValueTypeString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@".TryParse(s, out value_type_value))
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
                    value = default(");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@");
                    return false;
                }
            }
            ");
}
else if ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->is_datetime())
{
source.Append(@"
            {
                ");
if ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->is_nullable())
{
source.Append(@"
                {
                    if (string.IsNullOrEmpty(s) || string.Compare(s, ""null"", ignoreCase: true) == 0)
                    {
                        value = default(");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@");
                        return true;
                    }
                }
                ");
}
source.Append(@"
                if (");
source.Append(Codegen.GetString(Trinity::Codegen::GetNonNullableValueTypeString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@".TryParse(s, null, System.Globalization.DateTimeStyles.RoundtripKind, out value_type_value))
                {
                    value = value_type_value;
                    return true;
                }
                if (s.EndsWith("" UTC"", StringComparison.Ordinal) && ");
source.Append(Codegen.GetString(Trinity::Codegen::GetNonNullableValueTypeString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@".TryParse(s.Substring(0, s.Length - 4) + 'Z', null, System.Globalization.DateTimeStyles.RoundtripKind, out value_type_value))
                {
                    value = value_type_value;
                    return true;
                }
                value  = default(");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@");
                return false;
            }
            ");
}
else if ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->is_nullable() /* and has built-in TryParse */)
{
source.Append(@"
            if (string.IsNullOrEmpty(s) || string.Compare(s, ""null"", ignoreCase: true) == 0)
            {
                value = default(");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@");
                return true;
            }
            else if (");
source.Append(Codegen.GetString(Trinity::Codegen::GetNonNullableValueTypeString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@".TryParse(s, out value_type_value))
            {
                value = value_type_value;
                return true;
            }
            else
            {
                value = default(");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@");
                return false;
            }
            ");
}
else if ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->is_array())
{
source.Append(@"
            try
            {
                jarray = JArray.Parse(s);
                value = new ");
source.Append(Codegen.GetString(data_type_get_array_type_with_size_string((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1])));
source.Append(@";");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.arrayElement));
source.Append(@" element;
                int ");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->arrayInfo.arrayElement)));
source.Append(@"_offset = 0;
                ");
for (int iterator_2 = 0; iterator_2 < ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.array_dimension_size).Count;++iterator_2)
{
source.Append(@"
                for (int ");
source.Append(Codegen.GetString(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->arrayInfo.arrayElement)) + '_' + GetString(iterator_2) + Discard(((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.array_dimension_size)[iterator_2])));
source.Append(@" = 0; ");
source.Append(Codegen.GetString(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->arrayInfo.arrayElement)) + '_' + GetString(iterator_2) + Discard(((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.array_dimension_size)[iterator_2])));
source.Append(@" < ");
source.Append(Codegen.GetString(((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.array_dimension_size)[iterator_2]));
source.Append(@"/*_*/ ; ++");
source.Append(Codegen.GetString(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->arrayInfo.arrayElement)) + '_' + GetString(iterator_2) + Discard(((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.array_dimension_size)[iterator_2])));
source.Append(@")
                ");
}
source.Append(@"
                {
                    ");
if (data_type_need_external_parser((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.arrayElement))
{
source.Append(@"
                    {
                        if (!ExternalParser.TryParse_");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.arrayElement));
source.Append(@"((string)jarray[");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->arrayInfo.arrayElement)));
source.Append(@"_offset++], out element))
                            continue;
                        value[");
for (int iterator_2 = 0; iterator_2 < ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.array_dimension_size).Count;++iterator_2)
{
source.Append(Codegen.GetString(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->arrayInfo.arrayElement)) + '_' + GetString(iterator_2) + Discard(((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.array_dimension_size)[iterator_2])));
if (iterator_2 < ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.array_dimension_size).Count - 1)
source.Append(",");
}
source.Append(@"] = element;
                    }
                    ");
}
else if ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.arrayElement->is_string())
{
source.Append(@"
                    {
                        value[");
for (int iterator_2 = 0; iterator_2 < ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.array_dimension_size).Count;++iterator_2)
{
source.Append(Codegen.GetString(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->arrayInfo.arrayElement)) + '_' + GetString(iterator_2) + Discard(((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.array_dimension_size)[iterator_2])));
if (iterator_2 < ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.array_dimension_size).Count - 1)
source.Append(",");
}
source.Append(@"] = (string)jarray[");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->arrayInfo.arrayElement)));
source.Append(@"_offset++];
                    }
                    ");
}
else
{
source.Append(@"
                    {
                        if (!");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.arrayElement));
source.Append(@".TryParse((string)jarray[");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->arrayInfo.arrayElement)));
source.Append(@"_offset++], out element))
                            continue;
                        value[");
for (int iterator_2 = 0; iterator_2 < ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.array_dimension_size).Count;++iterator_2)
{
source.Append(Codegen.GetString(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->arrayInfo.arrayElement)) + '_' + GetString(iterator_2) + Discard(((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.array_dimension_size)[iterator_2])));
if (iterator_2 < ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].arrayInfo.array_dimension_size).Count - 1)
source.Append(",");
}
source.Append(@"] = element;
                    }
                    ");
}
source.Append(@"
                }
                return true;
            }
            catch
            {
                value = default(");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@");
                return false;
            }
            ");
}
else if ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->is_list())
{
source.Append(@"
            try
            {
                value = new ");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@"();
                jarray = JArray.Parse(s);
                foreach (var jarray_element in jarray)
                {
                    ");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].listElementType));
source.Append(@" element;
                    ");
if (data_type_need_external_parser((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->listElementType))
{
source.Append(@"
                    if (!ExternalParser.TryParse_");
source.Append(Codegen.GetString(Trinity::Codegen::GetDataTypeDisplayString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->listElementType)));
source.Append(@"((string)jarray_element, out element))
                    {
                        continue;
                    }
                    value.Add(element);
                    ");
}
else if ((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]->listElementType->is_string())
{
source.Append(@"
                    value.Add((string)jarray_element);
                    ");
}
else
{
source.Append(@"
                    if (!");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1].listElementType));
source.Append(@".TryParse((string)jarray_element, out element))
                    {
                        continue;
                    }
                    value.Add(element);
                    ");
}
source.Append(@"
                }
                return true;
            }
            catch
            {
                value = default(");
source.Append(Codegen.GetString((Trinity::Codegen::TSLExternalParserDataTypeVector)[iterator_1]));
source.Append(@");
                return false;
            }
            ");
}
source.Append(@"
        }
        ");
}
}
source.Append(@"
        #region Mute
        
        #endregion
    }
}
");

            return source.ToString();
        }
    }
}
