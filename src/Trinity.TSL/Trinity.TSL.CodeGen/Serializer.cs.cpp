#include "common.h"
#include <string>
#include "SyntaxNode.h"

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        string* 
Serializer(
NTSL* node)
        {
            string* source = new string();
            
source->append(R"::(
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
namespace )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(
{
    
    /// <summary>
    /// Provides facilities for serializing data to Json strings.
    /// </summary>
    public class Serializer
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
        )::");
for (size_t iterator_1 = 0; iterator_1 < (Trinity::Codegen::TSLSerializerDataTypeVector)->size();++iterator_1)
{
if (data_type_need_type_id((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1], Trinity::Codegen::TSLSerializerDataTypeVector) && data_type_is_not_duplicate_array((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1], Trinity::Codegen::TSLSerializerDataTypeVector))
{
source->append(R"::(
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Serializes a )::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]));
source->append(R"::( object to Json string.
        /// </summary>
        /// <param name="value">The target object to be serialized.</param>
        /// <returns>The serialized Json string.</returns>
        public static string ToString()::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]));
source->append(R"::( value)
        {
            s_ensure_string_builder();
            ToString_impl(value, s_stringBuilder, in_json: false);
            return s_stringBuilder.ToString();
        }
        )::");
}
}
for (size_t iterator_1 = 0; iterator_1 < (node->cellList)->size();++iterator_1)
{
source->append(R"::(
        /// <summary>
        /// Serializes a )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::( object to Json string.
        /// </summary>
        /// <param name="value">The target cell object to be serialized.</param>
        /// <returns>The serialized Json string.</returns>
        public static string ToString()::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::( cell)
        {
            s_ensure_string_builder();
            s_stringBuilder.Append('{');
            s_stringBuilder.AppendFormat("\"CellID\":{0}", cell.CellID);
            )::");
for (size_t iterator_2 = 0; iterator_2 < ((*(node->cellList))[iterator_1]->fieldList)->size();++iterator_2)
{
source->append(R"::(
            {
                )::");
if ((*((*(node->cellList))[iterator_1]->fieldList))[iterator_2]->fieldType->is_nullable() || !(*((*(node->cellList))[iterator_1]->fieldList))[iterator_2]->fieldType->is_value_type())
{
source->append(R"::(
                if (cell.)::");
source->append(Codegen::GetString((*((*(node->cellList))[iterator_1]->fieldList))[iterator_2]->name));
source->append(R"::( != null)
                {
                    )::");
}
source->append(R"::(
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\")::");
source->append(Codegen::GetString((*((*(node->cellList))[iterator_1]->fieldList))[iterator_2]->name));
source->append(R"::(\":");
                    ToString_impl(cell.)::");
source->append(Codegen::GetString((*((*(node->cellList))[iterator_1]->fieldList))[iterator_2]->name));
source->append(R"::(, s_stringBuilder, in_json: true);
                    )::");
if ((*((*(node->cellList))[iterator_1]->fieldList))[iterator_2]->fieldType->is_nullable() || !(*((*(node->cellList))[iterator_1]->fieldList))[iterator_2]->fieldType->is_value_type())
{
source->append(R"::(
                }
                )::");
}
source->append(R"::(
            }
            )::");
}
source->append(R"::(
            s_stringBuilder.Append('}');
            return s_stringBuilder.ToString();
        }
        )::");
}
for (size_t iterator_1 = 0; iterator_1 < (Trinity::Codegen::TSLSerializerDataTypeVector)->size();++iterator_1)
{
if (data_type_need_type_id((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1], Trinity::Codegen::TSLSerializerDataTypeVector) && data_type_is_not_duplicate_array((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1], Trinity::Codegen::TSLSerializerDataTypeVector))
{
source->append(R"::(
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ToString_impl()::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]));
source->append(R"::( value, StringBuilder str_builder, bool in_json)
        {
            )::");
if ((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]->is_string())
{
source->append(R"::(
            if (in_json)
            {
                str_builder.Append(JsonStringProcessor.escape(value));
            }
            else
            {
                str_builder.Append(value);
            }
            )::");
}
else if ((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]->is_struct())
{
source->append(R"::(
            {
                )::");
if ((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]->is_optional())
{
source->append(R"::(
                if (value == null)
                    return;
                )::");
}
source->append(R"::(
                str_builder.Append('{');
                bool first_field = true;
                )::");
for (size_t iterator_2 = 0; iterator_2 < ((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]->referencedNStruct->fieldList)->size();++iterator_2)
{
source->append(R"::(
                {
                    )::");
if ((*((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]->referencedNStruct->fieldList))[iterator_2]->fieldType->is_nullable() || !(*((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]->referencedNStruct->fieldList))[iterator_2]->fieldType->is_value_type())
{
if ((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]->is_optional())
{
source->append(R"::(
                    if (value.Value.)::");
source->append(Codegen::GetString((*((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]->referencedNStruct->fieldList))[iterator_2]->name));
source->append(R"::( != null)
                        )::");
}
else
{
source->append(R"::(
                    if (value.)::");
source->append(Codegen::GetString((*((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]->referencedNStruct->fieldList))[iterator_2]->name));
source->append(R"::( != null)
                    )::");
}
source->append(R"::(
                    {
                        )::");
}
source->append(R"::(
                        if(first_field)
                            first_field = false;
                        else
                            str_builder.Append(',');
                        str_builder.Append("\")::");
source->append(Codegen::GetString((*((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]->referencedNStruct->fieldList))[iterator_2]->name));
source->append(R"::(\":");
                        )::");
if ((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]->is_optional())
{
source->append(R"::(
                        ToString_impl(value.Value.)::");
source->append(Codegen::GetString((*((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]->referencedNStruct->fieldList))[iterator_2]->name));
source->append(R"::(, str_builder, in_json: true);
                        )::");
}
else
{
source->append(R"::(
                        ToString_impl(value.)::");
source->append(Codegen::GetString((*((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]->referencedNStruct->fieldList))[iterator_2]->name));
source->append(R"::(, str_builder, in_json: true);
                        )::");
}
if ((*((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]->referencedNStruct->fieldList))[iterator_2]->fieldType->is_nullable() || !(*((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]->referencedNStruct->fieldList))[iterator_2]->fieldType->is_value_type())
{
source->append(R"::(
                    }
                    )::");
}
source->append(R"::(
                }
                )::");
}
source->append(R"::(
                str_builder.Append('}');
            }
            )::");
}
else if ((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]->is_value_type())
{
source->append(R"::(
            {
                )::");
if ((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]->is_enum() || (*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]->is_datetime() || (*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]->is_guid())
{
source->append(R"::(
                if(in_json)
                    str_builder.Append('"');
                )::");
}
if ((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]->is_bool())
{
source->append(R"::(
                {
                    str_builder.Append(value.ToString().ToLowerInvariant());
                }
                )::");
}
else if ((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]->is_datetime())
{
source->append(R"::(
                {
                    )::");
if ((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]->is_nullable())
{
source->append(R"::(
                    {
                        str_builder.Append(value.Value.ToString("o", CultureInfo.InvariantCulture));
                    }
                    )::");
}
else
{
source->append(R"::(
                    {
                        str_builder.Append(value.ToString("o", CultureInfo.InvariantCulture));
                    }
                    )::");
}
source->append(R"::(
                }
                )::");
}
else
{
source->append(R"::(
                {
                    str_builder.Append(value);
                }
                )::");
}
if ((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]->is_enum() || (*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]->is_datetime() || (*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]->is_guid())
{
source->append(R"::(
                if(in_json)
                    str_builder.Append('"');
                )::");
}
source->append(R"::(
            }
            )::");
}
else if ((*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]->is_array() || (*(Trinity::Codegen::TSLSerializerDataTypeVector))[iterator_1]->is_list())
{
source->append(R"::(
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
            )::");
}
else
{
source->append(R"::(
            throw new Exception("Internal error T5007");
            )::");
}
source->append(R"::(
        }
        )::");
}
}
source->append(R"::(
        #region mute
        
        #endregion
    }
}
)::");

            return source;
        }
    }
}
