using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Trinity.TSL.CodeTemplates;
using Trinity.Utilities;

namespace Trinity.TSL
{
    class StructCodeTemplate
    {
        #region GenerateStructCode
        static public string GenerateAccessorCode(StructDescriptor desc)
        {
            CodeWriter cw = new CodeWriter();
            cw += GenerateAccessorCode(desc, false);
            cw += GenerateAccessorCode(desc, true);
            return cw.Value;
        }

        internal static string GenerateAccessorCode(StructDescriptor structDesc, bool isReadOnly)
        {
            string ret = "";

            string struct_header = structDesc.Name + "_Accessor";
            if (isReadOnly)
                struct_header = structDesc.Name + "_Accessor_ReadOnly";

            ret += @"
    /// <summary>
    /// Provides in-place operations of " + structDesc.Name + @" defined in TSL.
    /// </summary>
    public unsafe class " + struct_header + @"
    {
        ///<summary>
        ///The pointer to the content of the object.
        ///</summary>
        internal byte* CellPtr;
        internal long? CellID;
        ";
            if (!structDesc.IsFixed())//Dynamic struct need resize function
            {
                ret += TSLCompiler.ResizeFunctionDelegate();
                ret += @"
        internal unsafe " + structDesc.Name + ((isReadOnly) ? ("_Accessor_ReadOnly") : ("_Accessor")) + @"(byte* _CellPtr, ResizeFunctionDelegate func)
        {
            CellPtr = _CellPtr;
            ResizeFunction = func;
            ";
                foreach (var field in structDesc.Fields)
                {
                    ret += TSLCompiler.GenerateAccessorFieldAssignmentCode(new DynamicStructFieldType(structDesc), field.Type, isReadOnly, field.Name + "_Accessor_Field", false);
                }
                ret += @"
        }
";


                if (isReadOnly)
                {
                    ret +=
    @"
        public static implicit operator " + struct_header + @"(" + structDesc.Name + @"_Accessor accessor )
        {
            return new " + struct_header + @"(accessor.CellPtr, accessor.ResizeFunction);
        }
";
                }
                else
                {
                    ret +=
    @"
        public static implicit operator " + struct_header + @"(" + structDesc.Name + @"_Accessor_ReadOnly accessor )
        {
            return new " + struct_header + @"(accessor.CellPtr, accessor.ResizeFunction);
        }
";
                }

            }
            else//Fixed
            {
                ret += @"
        internal unsafe " + structDesc.Name + ((isReadOnly) ? ("_Accessor_ReadOnly") : ("_Accessor")) + @"(byte* _CellPtr)
        {
            CellPtr = _CellPtr;";
                foreach (var field in structDesc.Fields)
                {
                    ret += TSLCompiler.GenerateAccessorFieldAssignmentCodeForFixedField(field.Type, isReadOnly, field.Name + "_Accessor_Field");
                }
                ret += @"
        }
";

                if (isReadOnly)
                {
                    ret +=
    @"
        public static implicit operator " + struct_header + @"(" + structDesc.Name + @"_Accessor accessor )
        {
            return new " + struct_header + @"(accessor.CellPtr);
        }
";
                }
                else
                {
                    ret +=
    @"
        public static implicit operator " + struct_header + @"(" + structDesc.Name + @"_Accessor_ReadOnly accessor )
        {
            return new " + struct_header + @"(accessor.CellPtr);
        }
";
                }
            }

            ret += AccessorCodeTemplate.GenerateOptionalFieldMap(structDesc);

            ret += @"
        ///<summary>
        ///Copies the struct content into a byte array.
        ///</summary>
        public byte[] ToByteArray()
        {
            byte* targetPtr = CellPtr;
";
            ret += AccessorCodeTemplate.GenerateFieldPushPointerCode(structDesc, structDesc.Fields.Count, "this");
            {
                ret += @"
            int size = (int)(targetPtr - CellPtr);
            byte[] ret = new byte[size];
            Memory.Copy(CellPtr,0,ret,0,size);
            return ret;";
            }
            ret += @"
        }
";

            ret += AccessorCodeTemplate.GenerateFieldPropertiesCode(structDesc, isReadOnly);

            ret += ImplicitOperatorCodeTemplate.GenerateFormatImplicitOperatorCode(structDesc, isReadOnly, !structDesc.IsFixed());

            ret += EqualOperatorCodeTemplate.GenerateFormatEqualOperatorCode(structDesc, isReadOnly);

            ret += @"
    }
";

            return ret;
        }

        //by default, formats doesn't have a constructor from byte array.
        public static string GenerateStructCode(StructDescriptor struct_desc, bool forCell = false)
        {
            CodeWriter ret = new CodeWriter();
            /*
             *  for a fixed struct with only value type fields, we'll add [StructLayout(LayoutKind.Explicit)] to prevent the compiler from aligning our struct
             *  
             *  !!! NOTE
             *  
             *  For fixed struct with array/bitarray fields, or sub-struct with array/bitarray fields,
             *  Assigning fixed layout to the struct will lead to runtime exception (Incorrectly aligned or overlapped by a non-object field error)
             *  see http://stackoverflow.com/questions/1190079/incorrectly-aligned-or-overlapped-by-a-non-object-field-error
             *  
             *  And, if the struct contains an array/BitArray (which is an object), pointers to the struct
             *  is not allowed ( Cannot take the address of, get the size of, or declare a pointer to a managed type )
             *  see http://stackoverflow.com/questions/9341081/cannot-take-the-address-of-get-the-size-of-or-declare-a-pointer-to-a-managed-t
             *  
             *  So we will only generate the layout arrangement for fixed format with only value type fields.
             * 
             */

            bool genLayout = struct_desc.ContainsOnlyValueTypeFields();

            if (genLayout && !forCell)
                ret += @"
    [StructLayout(LayoutKind.Explicit)]";

            ret += @"
    public partial struct " + struct_desc.Name + @"
    {
";
            if (forCell)
                ret += @"
        ///<summary>
        ///The id of the cell.
        ///</summary>
        public long CellID;";

            #region Constructor with cellID + parameters
            if (forCell)
            {
                ret += TSLCompiler.GenerateAssignmentPrototypeParameterList(@"
        ///<summary>
        ///Initializes a new cell of the type " + struct_desc.Name + @" with the specified parameters.
        ///</summary>
        public " + struct_desc.Name + "(long cell_id", struct_desc);

                foreach (Field field in struct_desc.Fields)
                {
                    ret += "            this." + field.Name + " = " + field.Name + ";\r\n";
                }

                ret += "\t\t\r\n";
                ret += @"CellID = cell_id;";
                ret += "\r\n\t\t}\r\n";
            }
            #endregion

            #region Constructor with parameters
            if (struct_desc.Fields.Count > 0)
            {
                ret += TSLCompiler.GenerateAssignmentPrototypeParameterList(@"
        ///<summary>
        ///Initializes a new instance of the " + struct_desc.Name + @" class with the specified parameters.
        ///</summary>
        public " + struct_desc.Name + "(", struct_desc, false, true);

                foreach (Field field in struct_desc.Fields)
                {
                    ret += "            this." + field.Name + " = " + field.Name + ";\r\n";
                }
                if (forCell)
                {
                    ret += "\r\n\t\t";
                    ret += @"CellID = CellIDFactory.NewCellID();";
                }
                ret += "\r\n\t\t}\r\n";
            }
            #endregion

            if (genLayout && !forCell)
            {
                int FieldOffset = 0;
                foreach (Field field in struct_desc.Fields)
                {
                    ret += @"
        [FieldOffset(" + FieldOffset.ToString(CultureInfo.InvariantCulture) + @")]
";
                    ret += @"
        public " + field.Type.CSharpName + " " + field.Name + @";
";
                    FieldOffset += (field.Type as FixedFieldType).Length;
                }
            }
            else
            {
                foreach (Field field in struct_desc.Fields)
                {
                    if (struct_desc.OptionalFieldSequenceMap.ContainsKey(field))
                    {
                        ret += @"
        public " + TSLCompiler.GetNullableTypeName(field) + " " + field.Name + @";
";
                    }
                    else
                    {
                        ret += @"
        public " + field.Type.CSharpName + " " + field.Name + @";
";
                    }
                }
            }

            #region Implicit conversions from/to a corresponded tuple
            /*
             * public static implicit operator Tuple<int,int,List<int>>(KVTupleExtended FormatStruct)
             * {
             *      return new Tuple<int,int,List<int>>(FormatStruct.WordID,FormatStruct.WordFreq,FormatStruct.list);
             * }
             * 
            */

            if (struct_desc.Fields.Count <= 8 && struct_desc.Fields.Count > 0)
            {

                string tupleTypeString = "Tuple<";
                foreach (Field field in struct_desc.Fields)
                {
                    if (struct_desc.OptionalFieldSequenceMap.ContainsKey(field))
                        tupleTypeString += TSLCompiler.GetNullableTypeName(field) + ",";
                    else
                        tupleTypeString += field.Type.CSharpName + ",";
                }
                tupleTypeString = tupleTypeString.Remove(tupleTypeString.Length - 1) + ">";

                ret += @"
        public static implicit operator " + tupleTypeString + "(" + struct_desc.Name + @" FormatStruct)
        {
            return new " + tupleTypeString + "(";
                foreach (Field field in struct_desc.Fields)
                {
                    ret += "FormatStruct." + field.Name + ",";
                }
                ret = ret.Remove(ret.Length - 1);
                ret += ");" + @"
        }
";

                ret += @"
        public static implicit operator " + struct_desc.Name + @" (" + tupleTypeString + @"tuple)
        {
            return new " + struct_desc.Name + "(";
                for (int i = 0; i < struct_desc.Fields.Count; ++i)
                {
                    if (i < 7)
                        ret += "tuple.Item" + (i + 1).ToString(CultureInfo.InvariantCulture) + ",";
                    else
                        ret += "tuple.Rest,";
                }
                ret = ret.Remove(ret.Length - 1);
                ret += ");" + @"
        }
";

                if (struct_desc.Fields.Count == 2)//A struct with two fields can be converted from/to a KeyValuePair too
                {
                    string keyValuePairString = "KeyValuePair<";
                    foreach (Field field in struct_desc.Fields)
                    {
                        if (struct_desc.OptionalFieldSequenceMap.ContainsKey(field))
                            keyValuePairString += TSLCompiler.GetNullableTypeName(field) + ",";
                        else
                            keyValuePairString += field.Type.CSharpName + ",";
                    }
                    keyValuePairString = keyValuePairString.Remove(keyValuePairString.Length - 1) + ">";

                    ret += @"
        public static implicit operator " + keyValuePairString + "(" + struct_desc.Name + @" FormatStruct)
        {
            return new " + keyValuePairString + "(";
                    foreach (Field field in struct_desc.Fields)
                    {
                        ret += "FormatStruct." + field.Name + ",";
                    }
                    ret = ret.Remove(ret.Length - 1);
                    ret += ");" + @"
        }
";

                    ret += @"
        public static implicit operator " + struct_desc.Name + @" (" + keyValuePairString + @"tuple)
        {
                return new " + struct_desc.Name + "(";
                    ret += "tuple.Key,tuple.Value";
                    ret += ");" + @"
        }
";
                }
            }

            #endregion

            #region Override of "==" Operator
            ret += @"
        public static bool operator == (" + struct_desc.Name + @" a, " + struct_desc.Name + @" b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }
            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }
            // Return true if the fields match:
            return ";
            //a.WordFreq == b.WordFreq && a.WordID == b.WordID
            if (struct_desc.Fields.Count > 0)
            {
                foreach (var field in struct_desc.Fields)
                    ret += "a." + field.Name + " == b." + field.Name + " && ";
                ret = ret.TrimEnd(" &".ToCharArray()) + @";";
            }
            else
                ret += "true;";
            ret +=@"
        }

        public static bool operator != (" + struct_desc.Name + @" a, " + struct_desc.Name + @" b)
        {
            return !(a == b);
        }
";
            #endregion
            ret += @"
    }
";
            return ret;
        }

        #endregion
    }
}
