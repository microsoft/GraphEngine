using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Trinity.TSL.CodeTemplates
{
    class ImplicitOperatorCodeTemplate
    {
        internal static string GenerateReverseImplicitOperaterCode(FieldType fieldtype, bool isReadOnly, bool hasResizeFunction, bool forCell = false)
        {
            string CSharpFieldName = fieldtype.CSharpName;

            string ret = @"
        public unsafe static implicit operator " + TSLCompiler.GetAccessorTypeName(isReadOnly, fieldtype) + "(" + CSharpFieldName + @" field)
        {";

            ret += @"  
            byte* targetPtr = null;
            ";
            ret += fieldtype.GenerateAssignCodeForConstructor("field", 0, true) + @"
            byte* tmpcellptr = BufferAllocator.AllocBuffer((int)targetPtr);
            Memory.memset(tmpcellptr, 0, (ulong)targetPtr);
            targetPtr = tmpcellptr;
        ";
            ret += fieldtype.GenerateAssignCodeForConstructor("field", 0, false) + @"
            " + TSLCompiler.GetAccessorTypeName(isReadOnly, fieldtype) + @" ret = new " + TSLCompiler.GetAccessorTypeName(isReadOnly, fieldtype) +
                            (hasResizeFunction ? "(tmpcellptr,null);" : "(tmpcellptr);");
            if (forCell)
            {
                ret += @"
            ret.CellID = field.CellID;
            return ret;
        }
";
            }
            else
            {
                ret += @"
            ret.CellID = null;
            return ret;
        }
";
            }

            return ret;
        }

        internal static string GenerateDateTimeImplicitOperatorCode(DateTimeType datetype)
        {
            string ret = @"
        public unsafe static implicit operator DateTime(DateTimeAccessor accessor)
        {
            return DateTime.FromBinary(accessor.ToBinary());
        }
";
            ret += GenerateReverseImplicitOperaterCode(datetype, false, false);
            return ret;
        }

        internal static string GenerateEnumImplicitOperatorCode(EnumType datetype)
        {
            string ret = @"
        public unsafe static implicit operator byte(EnumAccessor accessor)
        {
            return *(byte*)accessor.CellPtr;
        }
        public unsafe static implicit operator EnumAccessor(byte value)
        {
            byte* ptr = BufferAllocator.AllocBuffer(sizeof(byte));
            *ptr=value;
            return new EnumAccessor(ptr);
        }
";
            return ret;
        }

        internal static string GenerateGuidImplicitOperatorCode(GuidType guidtype)
        {
            string ret = @"
        public unsafe static implicit operator Guid(GuidAccessor accessor)
        {
            byte[] data = accessor.ToByteArray();
            return new Guid(data);
        }
";
            ret += GenerateReverseImplicitOperaterCode(guidtype, false, false);
            return ret;
        }

        internal static string GenerateListImplicitOperaterCode(DynamicFieldType listtype)
        {
            string ret = "";
            string CSharpListName = listtype.CSharpName;
            ret += @"
        public unsafe static implicit operator " + CSharpListName + "(" + TSLCompiler.GetAccessorTypeName(false, listtype) + @" accessor)
        {
            if((object)accessor == null) return null;
            " + CSharpListName + " list = new " + CSharpListName + @"();
            accessor.ForEach(element => list.Add(element));
            return list;
        }
";
            ret += GenerateReverseImplicitOperaterCode(listtype, false, true);

            return ret;
        }

        internal static string GenerateStringImplicitOperaterCode(StringType stringtype)
        {
            string ret = "";
            string CSharpListName = stringtype.CSharpName;
            ret += @"
        public unsafe static implicit operator string(" + TSLCompiler.GetAccessorTypeName(false, stringtype) + @" accessor)
        {
            if((object)accessor == null) return null;
            StringBuilder sb = new StringBuilder();
            accessor.ForEach(element => sb.Append(element));
            return sb.ToString();
        }
";
            ret += GenerateReverseImplicitOperaterCode(stringtype, false, true);

            return ret;
        }
        public static string GenerateCellImplicitOperatorCode(StructDescriptor cellDesc)
        {
            string ret = "";
            /*
                    public static unsafe implicit operator WordTuple(WordTuple_Accessor_ReadOnly accessor)
                    {
                        return new WordTuple(accessor.WordID, accessor.WordFreq);
                    }
             */
            ret += @"
        public static unsafe implicit operator " + cellDesc.Name + '(' + cellDesc.Name + @"_Accessor accessor)
        {
            ";
            foreach (Field field in cellDesc.Fields)
            {
                if (cellDesc.OptionalFieldSequenceMap.ContainsKey(field))
                {
                    string fieldType = TSLCompiler.GetNullableTypeName(field);
                    string conversionString = "";
                    if (TSLCompiler.FieldNeedNullableWrapper(field.Type))
                        conversionString = "(" + TSLCompiler.GetNullableTypeName(field) + ")";
                    ret += @"
            " + fieldType + " _" + field.Name + " = default(" + fieldType + @");
            if (accessor.Contains_" + field.Name + @")
                 _" + field.Name + " = " + conversionString + "accessor." + field.Name + ";";
                }
            }
            ret += @"
            if(accessor.CellID != null)
            return new " + cellDesc.Name + '(' + "accessor.CellID.Value,";
            foreach (Field field in cellDesc.Fields)
            {
                if (cellDesc.OptionalFieldSequenceMap.ContainsKey(field))
                    ret += "_" + field.Name + ",";
                else
                    ret += "accessor." + field.Name + ",";
            }
            ret = ret.TrimEnd(",".ToCharArray());
            ret += @");";
            ret += @"
            else
            return new " + cellDesc.Name + '(';
            foreach (Field field in cellDesc.Fields)
            {
                if (cellDesc.OptionalFieldSequenceMap.ContainsKey(field))
                    ret += "_" + field.Name + ",";
                else
                    ret += "accessor." + field.Name + ",";
            }
            ret = ret.TrimEnd(",".ToCharArray());
            ret += @");";
            ret += @"
        }
";
            if (cellDesc.IsFixed())
            {
                FixedStructFieldType formattype = new FixedStructFieldType(cellDesc);
                ret += GenerateReverseImplicitOperaterCode(formattype, false, false, forCell: true);
            }
            else
            {
                DynamicStructFieldType formattype = new DynamicStructFieldType(cellDesc);
                ret += GenerateReverseImplicitOperaterCode(formattype, false, false, forCell: true);
            }

            return ret;
        }
        public static string GenerateFormatImplicitOperatorCode(StructDescriptor formatdesc, bool isReadOnly, bool hasResizeFunction)
        {
            string ret = "";
            ret += @"
        public static unsafe implicit operator " + formatdesc.Name + '(' + formatdesc.Name + ((isReadOnly) ? ("_Accessor_ReadOnly") : ("_Accessor")) + @" accessor)
        {
            ";
            foreach (Field field in formatdesc.Fields)
            {
                if (formatdesc.OptionalFieldSequenceMap.ContainsKey(field))
                {
                    string fieldType = TSLCompiler.GetNullableTypeName(field);
                    string conversionString = "";
                    if (TSLCompiler.FieldNeedNullableWrapper(field.Type))
                        conversionString = "(" + TSLCompiler.GetNullableTypeName(field) + ")";
                    ret += @"
            " + fieldType + " _" + field.Name + " = default(" + fieldType + @");
            if (accessor.Contains_" + field.Name + @")
                 _" + field.Name + " = " + conversionString + "accessor." + field.Name + ";";
                }
            }
            ret += @"
            return new " + formatdesc.Name + '(';
            foreach (Field field in formatdesc.Fields)
            {
                if (formatdesc.OptionalFieldSequenceMap.ContainsKey(field))
                    ret += "_" + field.Name + ",";
                else
                    ret += "accessor." + field.Name + ",";
            }
            ret = ret.TrimEnd(",".ToCharArray());
            ret += @");
        }
";
            if (formatdesc.IsFixed())
            {
                FixedStructFieldType formattype = new FixedStructFieldType(formatdesc);
                ret += GenerateReverseImplicitOperaterCode(formattype, isReadOnly, hasResizeFunction);
            }
            else
            {
                DynamicStructFieldType formattype = new DynamicStructFieldType(formatdesc);
                ret += GenerateReverseImplicitOperaterCode(formattype, isReadOnly, hasResizeFunction);
            }

            return ret;
        }
        /// <summary>
        /// Currently not used
        /// </summary>
        public static string GenerateMessageImplicitOperatorCode(StructDescriptor formatdesc, bool isReader, bool hasResizeFunction)
        {
            string ret = "";
            ret += @"
        public static unsafe implicit operator " + formatdesc.Name + '(' + formatdesc.Name + ((isReader) ? ("Reader") : ("Writer")) + @" accessor)
        {
            return new " + formatdesc.Name + '(';
            foreach (Field field in formatdesc.Fields)
            {
                ret += "accessor." + field.Name + ",";
            }
            ret = ret.TrimEnd(",".ToCharArray());
            ret += @");
        }
";
            if (formatdesc.IsFixed())
            {
                FixedStructFieldType formattype = new FixedStructFieldType(formatdesc);
                ret += GenerateReverseImplicitOperaterCode(formattype, isReader, hasResizeFunction);
            }
            else
            {
                DynamicStructFieldType formattype = new DynamicStructFieldType(formatdesc);
                ret += GenerateReverseImplicitOperaterCode(formattype, isReader, hasResizeFunction);
            }

            return ret;
        }


        internal static string GenerateArrayImplicitOperatorCode(ArrayType arraytype)
        {
            return "";
        }
        internal static string ArraySetPropertiesCode(Field field, string accessor_field_name, bool createOptionalField)
        {
            return "";
        }
        internal static string GuidSetPropertiesCode(Field field, bool createOptionalField)
        {
            return "";
        }
        internal static string DateTimeSetPropertiesCode(Field field, bool createOptionalField)
        {
            return "";
        }
        internal static string EnumSetPropertiesCode(Field field, bool createOptionalField)
        {
            return "";
        }
        public static string ContainerSetPropertiesCode(FieldType type, string accessorName, bool CreateOptionalField)
        {
            return "";
        }
        internal static string FormatSetPropertiesCode(Field field, bool createOptionalField, string whoseReizeFunc = "this")
        {
            return "";
        }

    }
}
