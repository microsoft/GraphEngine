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
            string ret = "";
            ret += @"
        public unsafe static implicit operator " + arraytype.ElementType.CSharpName + @"[";
            for (int i = 0; i < arraytype.lengths.Length - 1; i++) ret += ',';
            ret += "](" + arraytype.Name + @" accessor)
        {
            " + arraytype.ElementType.CSharpName + "[";
            for (int i = 0; i < arraytype.lengths.Length - 1; i++) ret += ',';
            ret += @"] ret = new " + arraytype.ElementType.CSharpName + "[";
            for (int i = 0; i < arraytype.lengths.Length; i++) ret += arraytype.Name + ".SizeDim" + (i + 1).ToString(CultureInfo.InvariantCulture) + ",";
            ret = ret.TrimEnd(",".ToCharArray());
            ret += @"];";
            if (TSLCompiler.IsValueType(arraytype.ElementType))
            {
                ret += @"
            fixed (" + arraytype.ElementType.CSharpName + @"* p = ret)
            {
                Memory.Copy(accessor.CellPtr, p, ";
                for (int i = 0; i < arraytype.lengths.Length; i++) ret += arraytype.Name + ".SizeDim" + (i + 1).ToString(CultureInfo.InvariantCulture) + "*";
                ret += arraytype.ElementType.Length.ToString(CultureInfo.InvariantCulture) + @");
            }";
            }
            else//We can't get pointer!
            {
                string accessor_type;
                if (arraytype.ElementType is StructFieldType)
                {
                    accessor_type = arraytype.ElementType.Name + "_Accessor";
                }
                else
                {
                    accessor_type = arraytype.ElementType.Name;
                }
                ret += @"
            " + accessor_type + " elementAccessor = new " + accessor_type + @"(accessor.CellPtr);
            ";
                string varName = "ret[";
                for (int i = 0; i < arraytype.lengths.Length; ++i)
                {
                    ret += string.Format(CultureInfo.InvariantCulture, "for(int iterator_{0} = 0;iterator_{0} < {1};++iterator_{0})\r\n", i, arraytype.lengths[i]);
                    varName += "iterator_" + i + ",";
                }
                varName = varName.TrimEnd(",".ToCharArray()) + "]";
                ret += @"
                {
                    " + varName + " = " + @"elementAccessor;
                    elementAccessor.CellPtr += " + arraytype.ElementType.Length + @";
                }
            ";
            }
            ret += @"
            return ret;
        }
";
            ret += GenerateReverseImplicitOperaterCode(arraytype, false, false);

            return ret;
        }

        //All container'storage set properties code are the same, except for BitListType
        public static string ContainerSetPropertiesCode(FieldType type, string accessorName, bool CreateOptionalField)
        {
            string ret = "";

            ret += @"
              int length = *(int*)(value.CellPtr - 4);";

            if (!CreateOptionalField)
                ret += @"

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != " + accessorName + @".CellID)
                {
                    //if not in the same Cell
                    " + accessorName + @".CellPtr = " + accessorName + @".ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, " + accessorName + @".CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        " + accessorName + @".CellPtr = " + accessorName + @".ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, " + accessorName + @".CellPtr, length + 4);
                    }
                }
                " + accessorName + @".CellPtr += 4;
";
            else//We're creating the container, not resizing it, so we will not access *targetPtr for an old length
                ret += @"
                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                if (value.CellID != " + accessorName + @".CellID)
                {
                    //if not in the same Cell
                    " + accessorName + @".CellPtr = " + accessorName + @".ResizeFunction(targetPtr, 0, length + 4);
                    Memory.Copy(value.CellPtr - 4, " + accessorName + @".CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[*(int*)(value.CellPtr - 4) + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        " + accessorName + @".CellPtr = " + accessorName + @".ResizeFunction(targetPtr, 0, length + 4);
                        Memory.Copy(tmpcellptr, " + accessorName + @".CellPtr, length + 4);
                    }
                }
                " + accessorName + @".CellPtr += 4;
";
            return ret;
        }


        //All list indexer container'storage set properties code are the same, except for BitListType
        public static string ContainerListIndexerSetPropertiesCode(FieldType type, string accessorName)
        {
            string ret = "";

            ret += @"
              int length = *(int*)(value.CellPtr - 4);";

            ret += @"
                int offset = (int)(targetPtr-CellPtr);
                int oldlength = *(int*)targetPtr;
                if (value.CellID != this.CellID)
                {
                    //if not in the same Cell
                  this.CellPtr = " + accessorName + @".ResizeFunction(this.CellPtr, (int)(targetPtr-CellPtr), length - oldlength);
                    Memory.Copy(value.CellPtr - 4, this.CellPtr+offset, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        this.CellPtr = " + accessorName + @".ResizeFunction(this.CellPtr,(int)(targetPtr-CellPtr), length - oldlength);
                        Memory.Copy(tmpcellptr, this.CellPtr + offset, length + 4);
                    }
                }
";
            return ret;
        }
 
        internal static string ArraySetPropertiesCode(Field field, string accessor_field_name, bool createOptionalField)
        {
            string ret = "";

            //string accessor_field_name = field.Name + "_Accessor_Field";
            string elementLength = "";
            if (field.Type is ArrayType) elementLength = (field.Type as ArrayType).ElementType.Length.ToString(CultureInfo.InvariantCulture);
            if (!createOptionalField)
            {
                ret = @"
                Memory.Copy(value.CellPtr, targetPtr, " + accessor_field_name + @" .Length * " + elementLength + @");";
            }
            else
            {
                ret = @"
                int offset = (int)(targetPtr - CellPtr);
                int length = " + accessor_field_name + @".Length * " + elementLength + @";
                if (value.CellID != this.CellID)
                {
                    this.CellPtr = this.ResizeFunction(this.CellPtr, offset, length);
                    Memory.Copy(value.CellPtr, this.CellPtr + offset, length);
                }
                else
                {
                    byte[] tmpcell = new byte[length];
                    fixed (byte* tmpcellptr = tmpcell)
                    {
                        Memory.Copy(value.CellPtr, tmpcellptr, length);
                        this.CellPtr = this.ResizeFunction(this.CellPtr, offset, length);
                        Memory.Copy(tmpcellptr, this.CellPtr + offset, length);
                    }
                }
";
            }
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="createOptionalField"></param>
        /// <param name="whoseReizeFunc">this parameter is for List[indexer]'storage setter</param>
        /// <returns></returns>
        internal static string FormatSetPropertiesCode(Field field, bool createOptionalField, string whoseReizeFunc = "this")
        {
            bool isfixed = field.Type is FixedStructFieldType;
            string ret = @"
                int offset = (int)(targetPtr - CellPtr);
                byte* oldtargetPtr = targetPtr;
";
            if (!createOptionalField)
            {
                ret += field.Type.GeneratePushPointerCode();
                ret += @"int oldlength = (int)(targetPtr - oldtargetPtr);";
            }
            else
                ret += @"int oldlength = 0;
";
            ret += @"                targetPtr = value.CellPtr;
";
            ret += field.Type.GeneratePushPointerCode() + @"
                int newlength = (int)(targetPtr - value.CellPtr);
";
            if (isfixed)
                ret += @"Memory.Copy(value.CellPtr, oldtargetPtr, oldlength);";
            else
                ret += @"
                if (newlength != oldlength)
                {
                    if (value.CellID != this.CellID)
                    {
                        this.CellPtr = "+whoseReizeFunc+@".ResizeFunction(this.CellPtr, offset, newlength - oldlength);
                        Memory.Copy(value.CellPtr, this.CellPtr + offset, newlength);
                    }
                    else
                    {
                        byte[] tmpcell = new byte[newlength];
                        fixed (byte* tmpcellptr = tmpcell)
                        {
                            Memory.Copy(value.CellPtr, tmpcellptr, newlength);
                            this.CellPtr = "+whoseReizeFunc+@".ResizeFunction(this.CellPtr, offset, newlength - oldlength);
                            Memory.Copy(tmpcellptr, this.CellPtr + offset, newlength);
                        }
                    }
                }
                else
                {
                    Memory.Copy(value.CellPtr, oldtargetPtr, oldlength);
                }
";

            return ret;
        }

        internal static string GuidSetPropertiesCode(Field field, bool createOptionalField)
        {
            string ret = "";
            if (!createOptionalField)
            {
                ret = @"
                Memory.Copy(value.CellPtr, targetPtr, 16);";
            }
            else
            {
                ret = @"
                int offset = (int)(targetPtr - CellPtr);
                if (value.CellID != this.CellID)
                {
                    this.CellPtr = this.ResizeFunction(this.CellPtr, offset, 16);
                    Memory.Copy(value.CellPtr, this.CellPtr + offset, 16);
                }
                else
                {
                    byte[] tmpcell = new byte[16];
                    fixed (byte* tmpcellptr = tmpcell)
                    {
                        Memory.Copy(value.CellPtr, tmpcellptr, 16);
                        this.CellPtr = this.ResizeFunction(this.CellPtr, offset, 16);
                        Memory.Copy(tmpcellptr, this.CellPtr + offset, 16);
                    }
                }
";
            }
            return ret;
        }

        internal static string DateTimeSetPropertiesCode(Field field, bool createOptionalField)
        {
            string ret = "";
            if (!createOptionalField)
            {
                ret = @"
                *(long*)targetPtr = *(long*)value.CellPtr;";
            }
            else
            {
                ret = @"
                int offset = (int)(targetPtr - CellPtr);
                if (value.CellID != this.CellID)
                {
                    this.CellPtr = this.ResizeFunction(this.CellPtr, offset, 8);
                    *(long*)(CellPtr + offset) = *(long*)value.CellPtr;
                }
                else
                {
                    long tmp = *(long*)value.CellPtr;
                    this.CellPtr = this.ResizeFunction(this.CellPtr, offset, 8);
                    *(long*)(CellPtr + offset) = tmp;
                }
";
            }
            return ret;
        }

        internal static string EnumSetPropertiesCode(Field field, bool createOptionalField)
        {
            string ret = "";
            if (!createOptionalField)
            {
                ret = @"
                *(byte*)targetPtr = *(byte*)value.CellPtr;";
            }
            else
            {
                ret = @"
                int offset = (int)(targetPtr - CellPtr);
                if (value.CellID != this.CellID)
                {
                    this.CellPtr = this.ResizeFunction(this.CellPtr, offset, sizeof(byte));
                    *(byte*)(CellPtr + offset) = *(byte*)value.CellPtr;
                }
                else
                {
                    byte tmp = *(byte*)value.CellPtr;
                    this.CellPtr = this.ResizeFunction(this.CellPtr, offset, sizeof(byte));
                    *(byte*)(CellPtr + offset) = tmp;
                }
";
            }
            return ret;
        }
    }
}
