using System.Text;
using System.Collections.Generic;

namespace Trinity.TSL.CodeTemplates
{
    internal partial class SourceFiles
    {
        internal static string 
Serializer(
NTSL node)
        {
            StringBuilder source = new StringBuilder();
            
source.Append(@"
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
namespace ");
source.Append(Codegen.GetString(Trinity::Codegen::GetNamespace()));
source.Append(@"
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
        ");
for (int iterator_1 = 0; iterator_1 < (Trinity::Codegen::TSLSerializerDataTypeVector).Count;++iterator_1)
{
if (data_type_need_set_field((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1], Trinity::Codegen::TSLSerializerDataTypeVector) && data_type_is_not_duplicate_array((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1], Trinity::Codegen::TSLSerializerDataTypeVector))
{
source.Append(@"
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Serializes a ");
source.Append(Codegen.GetString((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1]));
source.Append(@" object to Json string.
        /// </summary>
        /// <param name=""value"">The target object to be serialized.</param>
        /// <returns>The serialized Json string.</returns>
        public static string ToString(");
source.Append(Codegen.GetString((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1]));
source.Append(@" value)
        {
            s_ensure_string_builder();
            ToString_impl(value, s_stringBuilder, in_json: false);
            return s_stringBuilder.ToString();
        }
        ");
}
}
for (int iterator_1 = 0; iterator_1 < (node->cellList).Count;++iterator_1)
{
source.Append(@"
        /// <summary>
        /// Serializes a ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@" object to Json string.
        /// </summary>
        /// <param name=""value"">The target cell object to be serialized.</param>
        /// <returns>The serialized Json string.</returns>
        public static string ToString(");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@" cell)
        {
            s_ensure_string_builder();
            s_stringBuilder.Append('{');
            s_stringBuilder.AppendFormat(""\""CellID\"":{0}"", cell.CellID);
            ");
for (int iterator_2 = 0; iterator_2 < ((node->cellList)[iterator_1].fieldList).Count;++iterator_2)
{
source.Append(@"
            {
                ");
if (((node->cellList)[iterator_1].fieldList)[iterator_2].fieldType->is_nullable() || !((node->cellList)[iterator_1].fieldList)[iterator_2].fieldType->is_value_type())
{
source.Append(@"
                if (cell.");
source.Append(Codegen.GetString(((node->cellList)[iterator_1].fieldList)[iterator_2].name));
source.Append(@" != null)
                {
                    ");
}
source.Append(@"
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append(""\""");
source.Append(Codegen.GetString(((node->cellList)[iterator_1].fieldList)[iterator_2].name));
source.Append(@"\"":"");
                    ToString_impl(cell.");
source.Append(Codegen.GetString(((node->cellList)[iterator_1].fieldList)[iterator_2].name));
source.Append(@", s_stringBuilder, in_json: true);
                    ");
if (((node->cellList)[iterator_1].fieldList)[iterator_2].fieldType->is_nullable() || !((node->cellList)[iterator_1].fieldList)[iterator_2].fieldType->is_value_type())
{
source.Append(@"
                }
                ");
}
source.Append(@"
            }
            ");
}
source.Append(@"
            s_stringBuilder.Append('}');
            return s_stringBuilder.ToString();
        }
        ");
}
for (int iterator_1 = 0; iterator_1 < (Trinity::Codegen::TSLSerializerDataTypeVector).Count;++iterator_1)
{
if (data_type_need_set_field((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1], Trinity::Codegen::TSLSerializerDataTypeVector) && data_type_is_not_duplicate_array((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1], Trinity::Codegen::TSLSerializerDataTypeVector))
{
source.Append(@"
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ToString_impl(");
source.Append(Codegen.GetString((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1]));
source.Append(@" value, StringBuilder str_builder, bool in_json)
        {
            ");
if ((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1]->is_string())
{
source.Append(@"
            if (in_json)
            {
                str_builder.Append(JsonStringProcessor.escape(value));
            }
            else
            {
                str_builder.Append(value);
            }
            ");
}
else if ((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1]->is_struct())
{
source.Append(@"
            {
                ");
if ((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1]->is_optional())
{
source.Append(@"
                if (value == null)
                    return;
                ");
}
source.Append(@"
                str_builder.Append('{');
                bool first_field = true;
                ");
for (int iterator_2 = 0; iterator_2 < ((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1].referencedNStruct->fieldList).Count;++iterator_2)
{
source.Append(@"
                {
                    ");
if (((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1].referencedNStruct->fieldList)[iterator_2].fieldType->is_nullable() || !((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1].referencedNStruct->fieldList)[iterator_2].fieldType->is_value_type())
{
if ((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1]->is_optional())
{
source.Append(@"
                    if (value.Value.");
source.Append(Codegen.GetString(((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1].referencedNStruct->fieldList)[iterator_2].name));
source.Append(@" != null)
                        ");
}
else
{
source.Append(@"
                    if (value.");
source.Append(Codegen.GetString(((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1].referencedNStruct->fieldList)[iterator_2].name));
source.Append(@" != null)
                    ");
}
source.Append(@"
                    {
                        ");
}
source.Append(@"
                        if(first_field)
                            first_field = false;
                        else
                            str_builder.Append(',');
                        str_builder.Append(""\""");
source.Append(Codegen.GetString(((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1].referencedNStruct->fieldList)[iterator_2].name));
source.Append(@"\"":"");
                        ");
if ((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1]->is_optional())
{
source.Append(@"
                        ToString_impl(value.Value.");
source.Append(Codegen.GetString(((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1].referencedNStruct->fieldList)[iterator_2].name));
source.Append(@", str_builder, in_json: true);
                        ");
}
else
{
source.Append(@"
                        ToString_impl(value.");
source.Append(Codegen.GetString(((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1].referencedNStruct->fieldList)[iterator_2].name));
source.Append(@", str_builder, in_json: true);
                        ");
}
if (((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1].referencedNStruct->fieldList)[iterator_2].fieldType->is_nullable() || !((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1].referencedNStruct->fieldList)[iterator_2].fieldType->is_value_type())
{
source.Append(@"
                    }
                    ");
}
source.Append(@"
                }
                ");
}
source.Append(@"
                str_builder.Append('}');
            }
            ");
}
else if ((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1]->is_value_type())
{
source.Append(@"
            {
                ");
if ((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1]->is_enum() || (Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1]->is_datetime() || (Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1]->is_guid())
{
source.Append(@"
                if(in_json)
                    str_builder.Append('""');
                ");
}
if ((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1]->is_bool())
{
source.Append(@"
                {
                    str_builder.Append(value.ToString().ToLowerInvariant());
                }
                ");
}
else if ((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1]->is_datetime())
{
source.Append(@"
                {
                    ");
if ((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1]->is_nullable())
{
source.Append(@"
                    {
                        str_builder.Append(value.Value.ToString(""o"", CultureInfo.InvariantCulture));
                    }
                    ");
}
else
{
source.Append(@"
                    {
                        str_builder.Append(value.ToString(""o"", CultureInfo.InvariantCulture));
                    }
                    ");
}
source.Append(@"
                }
                ");
}
else
{
source.Append(@"
                {
                    str_builder.Append(value);
                }
                ");
}
if ((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1]->is_enum() || (Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1]->is_datetime() || (Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1]->is_guid())
{
source.Append(@"
                if(in_json)
                    str_builder.Append('""');
                ");
}
source.Append(@"
            }
            ");
}
else if ((Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1]->is_array() || (Trinity::Codegen::TSLSerializerDataTypeVector)[iterator_1]->is_list())
{
source.Append(@"
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
            ");
}
else
{
source.Append(@"
            throw new Exception(""Internal error T5007"");
            ");
}
source.Append(@"
        }
        ");
}
}
source.Append(@"
        #region mute
        
        #endregion
    }
}
");

            return source.ToString();
        }
    }
}
