using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Trinity.TSL
{
    /// <summary>
    /// If the pointer of a field cannot be represented as a const, or BasePtr + const, code will be generated like this:
    /// byte* targetPtr = CellPtr;
    /// targetPtr += Some_Fixed_Lengths;
    /// generated push pointer code (always add on "targetPtr"!)
    /// </summary>
    internal interface DynamicFieldType : FieldType
    {
    }

    internal class ListType : DynamicFieldType
    {
        public FieldType ElementFieldType;

        public ListType(FieldType fieldType)
        {
            this.ElementFieldType = fieldType;
        }

        public bool isFixedList()
        {
            return ElementFieldType is FixedFieldType;
        }

        public string Name
        {
            get
            {
                string ret = "";
                ret += ElementFieldType.Name + "ListAccessor";
                return ret;
            }
        }
        public string CSharpName
        {
            get
            {
                return "List<" + ElementFieldType.CSharpName + ">";
            }
        }

        public string GeneratePushPointerCode()
        {
            return "targetPtr += 4 + *(int*)targetPtr;\r\n";
        }

        public string GenerateAssignCodeForConstructor(string VarName, int currentLevel, bool OnlyPushPointer)
        {
            string iterator_name = "iterator_" + currentLevel.ToString(CultureInfo.InvariantCulture);
            string pointer_name = "storedPtr_" + currentLevel.ToString(CultureInfo.InvariantCulture);
            string ret = "";
            if (isFixedList())
            {
                string precalculated_length = VarName + ".Count*" + (ElementFieldType as FixedFieldType).Length.ToString(CultureInfo.InvariantCulture);
                if (!OnlyPushPointer)
                {
                    ret += @"
if(" + VarName + @"!= null)
{
    *(int*)targetPtr = " + precalculated_length + @";";

                    ret += @"
    targetPtr += sizeof(int);
    for(int " + iterator_name + " = 0;" + iterator_name + "<" + VarName + ".Count;++" + iterator_name + @")
    {
";
                    ret += ElementFieldType.GenerateAssignCodeForConstructor(VarName + "[" + iterator_name + "]", currentLevel + 1, OnlyPushPointer);
                    ret += @"
    }
";
                    ret += @"
}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}
";

                }
                else//Only push pointer
                {

                    ret += @"
if(" + VarName + @"!= null)
{
    targetPtr += " + precalculated_length + @"+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}

";
                }
            }
            else
            {
                ret += @"
{
";
                if (!OnlyPushPointer)
                {
                    ret += @"byte *" + pointer_name + @" = targetPtr;
";
                }

                ret += @"
    targetPtr += sizeof(int);
    if(" + VarName + @"!= null)
    {
        for(int " + iterator_name + " = 0;" + iterator_name + "<" + VarName + ".Count;++" + iterator_name + @")
        {
";
                ret += ElementFieldType.GenerateAssignCodeForConstructor(VarName + "[" + iterator_name + "]", currentLevel + 1, OnlyPushPointer);
                ret += @"
        }
    }
";

                if (!OnlyPushPointer)
                {
                    ret += "*(int*)" + pointer_name + " = (int)(targetPtr - " + pointer_name + @" - 4);
";
                }
                ret += @"
}
";
            }
            return ret;
        }
    }

    internal class StringType : DynamicFieldType
    {
        public string Name
        {
            get { return "StringAccessor"; }
        }

        public string CSharpName
        {
            get
            {
                return "string";
            }
        }
        public string GeneratePushPointerCode()
        {
            return "targetPtr += 4 + *(int*)targetPtr;\r\n";
        }

        public string GenerateAssignCodeForConstructor(string VarName, int currentLevel, bool OnlyPushPointer)
        {
            string ret = "";
            string iterator_name = "iterator_" + currentLevel.ToString(CultureInfo.InvariantCulture);
            string strlen_name   = "strlen_"   + currentLevel.ToString(CultureInfo.InvariantCulture);
            string pstr_name     = "pstr_"     + currentLevel.ToString(CultureInfo.InvariantCulture);

            if (!OnlyPushPointer)
            {
                ret += @"
        if(" + VarName + @"!= null)
        {
            int " + strlen_name + " = " + VarName + @".Length * 2;
            *(int*)targetPtr = " + strlen_name + @";
            targetPtr += sizeof(int);
            fixed(char* " + pstr_name + " = " + VarName + @")
            {
                Memory.Copy(" + pstr_name + ", targetPtr, " + strlen_name + @");
                targetPtr += "+ strlen_name + @";
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }
";
            }
            else//Only push pointer
            {
                ret += @"
        if(" + VarName + @"!= null)
        {
            int " + strlen_name + " = " + VarName + @".Length * 2;
            targetPtr += " + strlen_name + @"+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }
";
            }

            return ret;
        }
    }

    internal class U8StringType : DynamicFieldType
    {
        public string Name
        {
            get { return "U8StringAccessor"; }
        }

        public string CSharpName
        {
            get
            {
                return "string";
            }
        }
        public string GeneratePushPointerCode()
        {
            return "targetPtr += 4 + *(int*)targetPtr;\r\n";
        }

        public string GenerateAssignCodeForConstructor(string VarName, int currentLevel, bool OnlyPushPointer)
        {
            string ret = "";
            string u8buffer_name = "u8buffer_" + currentLevel.ToString(CultureInfo.InvariantCulture);
            string u8len_name    = "u8len_"    + currentLevel.ToString(CultureInfo.InvariantCulture);

            if (!OnlyPushPointer)
            {
                ret += @"
        if(" + VarName + @"!= null)
        {
            byte[] " + u8buffer_name + " = Encoding.UTF8.GetBytes(" + VarName + @");
            int " + u8len_name + " = " + u8buffer_name + @".Length;
            *(int*)targetPtr = " + u8len_name + @";
            targetPtr += sizeof(int);
            Memory.Copy(" + u8buffer_name + ", targetPtr, " + u8len_name + @");
            targetPtr += "+ u8len_name + @";
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }
";
            }
            else//Only push pointer
            {
                ret += @"
        if(" + VarName + @"!= null)
        {
            int " + u8len_name + " = Encoding.UTF8.GetByteCount(" + VarName +  @");
            targetPtr += " + u8len_name + @" + sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }
";
            }

            return ret;
        }
    }

    internal class DynamicStructFieldType : StructFieldType, DynamicFieldType
    {
        public DynamicStructFieldType(StructDescriptor format)
        {
            if (format.IsFixed())
                CompilerError.Throw(CompilerErrorType.DynamicFormatExpected);
            this.descriptor = format;
        }
        public override string GeneratePushPointerCode()
        {
            string ret = "";
            #region for optional fields
            int optionalHeaderLength = -1;
            string optionalHeaderVarName = TSLCompiler.GenerateRandomVariableName();

            if (descriptor.OptionalFieldSequenceMap.Count != 0)
            {
                optionalHeaderLength = (descriptor.OptionalFieldSequenceMap.Count + 0x07) >> 3;
                ret += @"
                    byte* " + optionalHeaderVarName + @" = targetPtr;
                    targetPtr +=" + optionalHeaderLength.ToString(CultureInfo.InvariantCulture) + ";";
            }
            #endregion
            foreach (Field field in descriptor.Fields)
            {
                bool is_optional = descriptor.OptionalFieldSequenceMap.ContainsKey(field);

                if (is_optional)
                {
                    int optional_index = descriptor.OptionalFieldSequenceMap[field];
                    int optional_array_index = optional_index >> 3;
                    int optional_access_mask = 1 << (optional_index & 0x7);
                    ret += @"
                if(0!=(" + optionalHeaderVarName + @"[ " + optional_array_index + @" ] & " + optional_access_mask + @"))
                {
";
                }

                if (field.Type is FixedFieldType)
                {
                    ret += @"
                        targetPtr += " + (field.Type as FixedFieldType).Length.ToString(CultureInfo.InvariantCulture) + ";";
                }
                else if (field.Type is DynamicFieldType)
                {
                    ret += "\t\t\t\t\t" + (field.Type as DynamicFieldType).GeneratePushPointerCode();
                }

                if (is_optional)
                {
                    ret += @"
                }
                ";
                }
            }

            return ret;
        }
    }


}
