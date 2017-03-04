using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Trinity.Utilities;

namespace Trinity.TSL
{
    internal partial class StructDescriptor : AbstractStruct
    {

        private void SortFields(Lexer l)
        {
            List<FieldInfo> sorted_fields = new List<FieldInfo>();
            List<string> sorted_fielNameList = new List<string>();

            List<FieldInfo> fields_copy = new List<FieldInfo>(FieldInfoList);

            foreach (var fi in fields_copy)
            {
                if (fi.field.Type is GuidType)
                {
                    sorted_fields.Add(fi);
                    FieldInfoList.Remove(fi);

                    sorted_fielNameList.Add(fi.field.Name);
                    FieldNameList.Remove(fi.field.Name);
                }
            }
            foreach (var fi in fields_copy)
            {
                if (fi.field.Type is DateTimeType)
                {
                    sorted_fields.Add(fi);
                    FieldInfoList.Remove(fi);

                    sorted_fielNameList.Add(fi.field.Name);
                    FieldNameList.Remove(fi.field.Name);
                }
            }

            List<FieldInfo> atom_fields = new List<FieldInfo>();

            foreach (var fi in fields_copy)
            {
                if (fi.field.Type is AtomType)
                {
                    atom_fields.Add(fi);

                    FieldInfoList.Remove(fi);
                    FieldNameList.Remove(fi.field.Name);
                }
            }

            SortAlgorithm.InsertionSort<FieldInfo>(atom_fields, (x, y) =>
            {
                if (((FixedFieldType)x.field.Type).Length < ((FixedFieldType)y.field.Type).Length)
                    return 1;
                else if (((FixedFieldType)x.field.Type).Length > ((FixedFieldType)y.field.Type).Length)
                    return -1;
                else
                    return 0;
            }
            );

            foreach (var fi in atom_fields)
            {
                sorted_fields.Add(fi);
                sorted_fielNameList.Add(fi.field.Name);
            }

            foreach (var fi in fields_copy)
            {
                if (fi.field.Type is ArrayType)
                {
                    sorted_fields.Add(fi);
                    FieldInfoList.Remove(fi);

                    sorted_fielNameList.Add(fi.field.Name);
                    FieldNameList.Remove(fi.field.Name);
                }
            }
            foreach (var fi in fields_copy)
            {
                if (fi.field.Type is BitArrayType)
                {
                    sorted_fields.Add(fi);
                    FieldInfoList.Remove(fi);

                    sorted_fielNameList.Add(fi.field.Name);
                    FieldNameList.Remove(fi.field.Name);
                }
            }
            foreach (var fi in fields_copy)
            {
                if (fi.field.Type is FixedStructFieldType)
                {
                    sorted_fields.Add(fi);
                    FieldInfoList.Remove(fi);

                    sorted_fielNameList.Add(fi.field.Name);
                    FieldNameList.Remove(fi.field.Name);
                }
            }

            foreach (var fi in fields_copy)
            {
                if (fi.field.Type is StringType)
                {
                    sorted_fields.Add(fi);
                    FieldInfoList.Remove(fi);

                    sorted_fielNameList.Add(fi.field.Name);
                    FieldNameList.Remove(fi.field.Name);
                }
            }

            foreach (var fi in fields_copy)
            {
                if (fi.field.Type is DynamicArrayType)
                {
                    sorted_fields.Add(fi);
                    FieldInfoList.Remove(fi);

                    sorted_fielNameList.Add(fi.field.Name);
                    FieldNameList.Remove(fi.field.Name);
                }
            }

            foreach (var fi in fields_copy)
            {
                if (fi.field.Type is ListType)
                {
                    sorted_fields.Add(fi);
                    FieldInfoList.Remove(fi);

                    sorted_fielNameList.Add(fi.field.Name);
                    FieldNameList.Remove(fi.field.Name);
                }
            }

            foreach (var fi in fields_copy)
            {
                if (fi.field.Type is BitListType)
                {
                    sorted_fields.Add(fi);
                    FieldInfoList.Remove(fi);

                    sorted_fielNameList.Add(fi.field.Name);
                    FieldNameList.Remove(fi.field.Name);
                }
            }

            foreach (var fi in fields_copy)
            {
                if (fi.field.Type is SetType)
                {
                    sorted_fields.Add(fi);
                    FieldInfoList.Remove(fi);

                    sorted_fielNameList.Add(fi.field.Name);
                    FieldNameList.Remove(fi.field.Name);
                }
            }

            foreach (var fi in fields_copy)
            {
                if (fi.field.Type is DictionaryType)
                {
                    sorted_fields.Add(fi);
                    FieldInfoList.Remove(fi);

                    sorted_fielNameList.Add(fi.field.Name);
                    FieldNameList.Remove(fi.field.Name);
                }
            }

            foreach (var fi in fields_copy)
            {
                if (fi.field.Type is DynamicStructFieldType)
                {
                    sorted_fields.Add(fi);
                    FieldInfoList.Remove(fi);

                    sorted_fielNameList.Add(fi.field.Name);
                    FieldNameList.Remove(fi.field.Name);
                }
            }

            foreach (var fi in FieldInfoList)
            {
                sorted_fields.Add(fi);
                sorted_fielNameList.Add(fi.field.Name);
            }

            Fields.Clear();

            StringBuilder sb = new StringBuilder();
            sb.Append(l.src.Substring(0, StructStartPos));

            int new_struct_length = 0;
            foreach (var fi in sorted_fields)
            {
                sb.Append(fi.src);
                new_struct_length += fi.src.Length;
                Fields.Add(fi.field);
            }


            sb.AppendLine("}"); //Close the new struct
            sb.Append(l.src.Substring(StructEndPos));

            l.src = sb.ToString();

            l.src_offset = StructStartPos + new_struct_length + 1;

            FieldNameList = sorted_fielNameList;
        }

        /// <summary>
        /// reorder the fields list: Fixed AtomType, ArrayType, FixedStructFieldType, Fields, Container, DynamicStructFieldType
        /// </summary>
        /// <param name="newOrder">pass an empty list,will return the the index of the ith in the origin fields</param>
        private void SortFields(List<int> newOrder)
        {
            newOrder.Clear();

            List<Field> sorted_fields = new List<Field>();
            List<string> sorted_fielNameList = new List<string>();

            List<Field> fields_copy = new List<Field>(Fields);

            foreach (var field in fields_copy)
            {
                if (field.Type is GuidType)
                {
                    sorted_fields.Add(field);
                    Fields.Remove(field);

                    sorted_fielNameList.Add(field.Name);
                    FieldNameList.Remove(field.Name);
                }
            }
            foreach (var field in fields_copy)
            {
                if (field.Type is DateTimeType)
                {
                    sorted_fields.Add(field);
                    Fields.Remove(field);

                    sorted_fielNameList.Add(field.Name);
                    FieldNameList.Remove(field.Name);
                }
            }

            List<Field> atom_fields = new List<Field>();

            foreach (var field in fields_copy)
            {
                if (field.Type is AtomType)
                {
                    atom_fields.Add(field);

                    Fields.Remove(field);
                    FieldNameList.Remove(field.Name);
                }
            }

            atom_fields.Sort((x, y) =>
            {
                if (((FixedFieldType)x.Type).Length < ((FixedFieldType)y.Type).Length)
                    return 1;
                else if (((FixedFieldType)x.Type).Length > ((FixedFieldType)y.Type).Length)
                    return -1;
                else
                    return 0;
            });

            foreach (var field in atom_fields)
            {
                sorted_fields.Add(field);
                sorted_fielNameList.Add(field.Name);
            }

            foreach (var field in fields_copy)
            {
                if (field.Type is ArrayType)
                {
                    sorted_fields.Add(field);
                    Fields.Remove(field);

                    sorted_fielNameList.Add(field.Name);
                    FieldNameList.Remove(field.Name);
                }
            }
            foreach (var field in fields_copy)
            {
                if (field.Type is BitArrayType)
                {
                    sorted_fields.Add(field);
                    Fields.Remove(field);

                    sorted_fielNameList.Add(field.Name);
                    FieldNameList.Remove(field.Name);
                }
            }
            foreach (var field in fields_copy)
            {
                if (field.Type is FixedStructFieldType)
                {
                    sorted_fields.Add(field);
                    Fields.Remove(field);

                    sorted_fielNameList.Add(field.Name);
                    FieldNameList.Remove(field.Name);
                }
            }

            foreach (var field in fields_copy)
            {
                if (field.Type is StringType)
                {
                    sorted_fields.Add(field);
                    Fields.Remove(field);

                    sorted_fielNameList.Add(field.Name);
                    FieldNameList.Remove(field.Name);
                }
            }

            foreach (var field in fields_copy)
            {
                if (field.Type is DynamicArrayType)
                {
                    sorted_fields.Add(field);
                    Fields.Remove(field);

                    sorted_fielNameList.Add(field.Name);
                    FieldNameList.Remove(field.Name);
                }
            }

            foreach (var field in fields_copy)
            {
                if (field.Type is ListType)
                {
                    sorted_fields.Add(field);
                    Fields.Remove(field);

                    sorted_fielNameList.Add(field.Name);
                    FieldNameList.Remove(field.Name);
                }
            }

            foreach (var field in fields_copy)
            {
                if (field.Type is BitListType)
                {
                    sorted_fields.Add(field);
                    Fields.Remove(field);

                    sorted_fielNameList.Add(field.Name);
                    FieldNameList.Remove(field.Name);
                }
            }

            foreach (var field in fields_copy)
            {
                if (field.Type is SetType)
                {
                    sorted_fields.Add(field);
                    Fields.Remove(field);

                    sorted_fielNameList.Add(field.Name);
                    FieldNameList.Remove(field.Name);
                }
            }

            foreach (var field in fields_copy)
            {
                if (field.Type is DictionaryType)
                {
                    sorted_fields.Add(field);
                    Fields.Remove(field);

                    sorted_fielNameList.Add(field.Name);
                    FieldNameList.Remove(field.Name);
                }
            }

            foreach (var field in fields_copy)
            {
                if (field.Type is DynamicStructFieldType)
                {
                    sorted_fields.Add(field);
                    Fields.Remove(field);

                    sorted_fielNameList.Add(field.Name);
                    FieldNameList.Remove(field.Name);
                }
            }

            foreach (var field in Fields)
            {
                sorted_fields.Add(field);
                sorted_fielNameList.Add(field.Name);
            }

            foreach (Field f in sorted_fields)
            {
                newOrder.Add(fields_copy.FindIndex(0, fields_copy.Count, match =>
                {
                    if (match == f)
                        return true;
                    else
                        return false;
                }));
            }

            Fields = sorted_fields;
            FieldNameList = sorted_fielNameList;
        }
    }
}
