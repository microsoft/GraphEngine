using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Trinity.TSL.CodeTemplates;
using Trinity.Utilities;

#region Atom type template
/* 
 * 
Atom type code block template:
        public unsafe <typename> <fieldname>
        {
            get
            {
                return *(<typename>*)(CellPtr + <fieldpos>);
            }
            set
            {
                *(<typename>*)(CellPtr + <fieldpos>) = value;
            }
        }
 * 
Atom type array code block template:
 * One dimension Atom type array
    public unsafe struct <typename>Array_WithoutBoundaryCheck
    {
        private int SizeDim1;
        private byte* CellPtr;

        public unsafe void init(byte* _CellPtr, int _SizeDim1)
        {
            this.SizeDim1 = _SizeDim1;
            this.CellPtr = _CellPtr;
        }

        public unsafe <typename> this[int indexDim1]
        {
            get
            {
                //if (indexDim1 > SizeDim1 - 1) throw (new IndexOutOfRangeException("The index is out of array size."));
                return *(<typename>*)(CellPtr + indexDim1 * sizeof(<typename>));
            }
            set
            {
                //if (indexDim1 > SizeDim1 - 1) throw (new IndexOutOfRangeException("The index is out of array size."));
                *(<typename>*)(CellPtr + indexDim1 * sizeof(<typename>)) = value;
            }
        }

 * Two dimension Atom type array
    public unsafe struct <typename>Array2_WithoutBoundaryCheck
    {
        private int SizeDim1;
        private int SizeDim2;
        private byte* CellPtr;

        public unsafe void init(byte* _CellPtr, int _SizeDim1, int _SizeDim2)
        {
            this.SizeDim1 = _SizeDim1;
            this.SizeDim2 = _SizeDim2;
            this.CellPtr = _CellPtr;
        }

        public unsafe <typename> this[int indexDim1, int indexDim2]
        {
            get
            {
                //if (indexDim1 > SizeDim1 - 1 || indexDim2 > SizeDim2 - 1 || indexDim1 < 0 || indexDim2 < 0) throw (new IndexOutOfRangeException("The index is out of array size."));
                return *(<typename>*)(CellPtr + indexDim1 * SizeDim2 * sizeof(<typename>) + indexDim2 * sizeof(<typename>));
            }
            set
            {
                //if (indexDim1 > SizeDim1 - 1 || indexDim2 > SizeDim2 - 1 || indexDim1 < 0 || indexDim2 < 0) throw (new IndexOutOfRangeException("The index is out of array size."));
                *(<typename>*)(CellPtr + indexDim1 * SizeDim2 * sizeof(<typename>) + indexDim2 * sizeof(<typename>)) = value;
            }
        }
    }
*/
#endregion

#region Format template
/*
    public struct <formatname>
    {
        public <formatname>(<filedtype1> _<fieldname1>, int _WordFreq)
        {
            WordID = _WordID;
            WordFreq = _WordFreq;
        }
        public int WordID;
        public int WordFreq;
    }
 
 * 
 * 
 * 
 */
#endregion

namespace Trinity.TSL
{
    public partial class TSLCompiler
    {
        /// <summary>
        /// For fixed fields
        /// </summary>
        internal static string GenerateAccessorFieldAssignmentCodeForFixedField(FieldType fieldType, bool isReadOnly, string accessor_field_name)
        {
            string ret = "";
            string accessor_type_name = TSLCompiler.GetAccessorTypeName(isReadOnly, fieldType);
            if (!(fieldType is AtomType || fieldType is EnumType))
            {
                ret += @"
        " + accessor_field_name + " = new " + accessor_type_name;

                //non atom fixed types
                ret += @"(null);
";
            }
            return ret;
        }

        internal static string GenerateAccessorFieldAssignmentCode(FieldType parent, FieldType fieldType, bool isReadOnly, string accessor_field_name, bool parentIsCell)
        {
            string ret = "";
            string accessor_type_name = TSLCompiler.GetAccessorTypeName(isReadOnly, fieldType);
            if (!(fieldType is AtomType || fieldType is EnumType))
            {
                ret += @"
        " + accessor_field_name + " = new " + accessor_type_name;

                if (fieldType is DynamicFieldType)
                {
                    ret += @"(null," +
                        ((parent is StructFieldType) ?
                            (TSLCompiler.ResizeLambdaFunctionBodyForFormat(parentIsCell)) :
                            (TSLCompiler.ResizeLambdaFunctionBodyForContainer()))
                        + @");
";
                }
                else//non atom fixed types
                    ret += @"(null);
";
            }
            return ret;
        }

        private static string GenerateChildTypeDeclarations(HashSet<string> processed_types, StructDescriptor struct_desc)
        {
            string ret = "";
            foreach (Field field in struct_desc.AllFields)
            {
                if (processed_types.Contains(field.Type.Name))
                    continue;

                processed_types.Add(field.Type.Name);

                if (field.Type is ArrayType)
                {
                    ret += ContainerCodeTemplate.GenerateArrayCode(field.Type as ArrayType);
                    continue;
                }
                else if (field.Type is StringType)
                {
                    //ret += ContainerCodeTemplate.GenerateStringCode(field.Type as StringType);
                    processed_types.Add(field.Type.Name);
                    continue;
                }
                else if (field.Type is U8StringType)
                {
                    //ret += ContainerCodeTemplate.GenerateStringCode(field.Type as U8StringType);
                    processed_types.Add(field.Type.Name);
                    continue;
                }
                else if (field.Type is ListType)
                {
                    ret += ContainerCodeTemplate.GenerateListCode(field.Type as ListType);
                    StructDescriptor tempFD = new StructDescriptor((field.Type as ListType).ElementFieldType);
                    ret += GenerateChildTypeDeclarations(processed_types, tempFD);
                    continue;
                }
                else if (field.Type is GuidType)
                {
                    //ret += ContainerCodeTemplate.GenerateGuidCode(field.Type as GuidType);
                    processed_types.Add(field.Type.Name);
                    continue;
                }
                else if (field.Type is DateTimeType)
                {
                    //ret += ContainerCodeTemplate.GenerateDateTimeCode(field.Type as DateTimeType);
                    processed_types.Add(field.Type.Name);
                    continue;
                }
                else if (field.Type is EnumType)
                {
                    //ret += ContainerCodeTemplate.GenerateEnumCode(field.Type as EnumType);
                    processed_types.Add(field.Type.Name);
                    continue;
                }
                else if (!(field.Type is AtomType || field.Type is StructFieldType))
                {
                    throw new NotImplementedException();
                }
            }
            return ret;
        }

        internal static string ResizeFunctionDelegate()
        {
            return "internal ResizeFunctionDelegate ResizeFunction;";
        }

        internal static string ResizeLambdaFunctionBodyForFormat(bool parentIsCell)
        {
            if (parentIsCell)
                return @"
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                }";
            else
                return @"
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.CellPtr = this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                }";
        }

        //if parent is a Container, we'll have to increase its size field
        internal static string ResizeLambdaFunctionBodyForContainer()
        {
            return @"
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.CellPtr = this.ResizeFunction(this.CellPtr-sizeof(int), ptr_offset + substructure_offset +sizeof(int), delta);
                    *(int*)this.CellPtr += delta;
                    this.CellPtr += sizeof(int);
                    return this.CellPtr + substructure_offset;
                }";
        }

        /// <summary>
        /// targetPtr will be ready for a Assign run at the newly allocated space
        /// </summary>
        /// <returns></returns>
        internal static string AppendCodeForContainer()
        {
            return @"
            int size = (int)targetPtr;
            this.CellPtr = this.ResizeFunction(this.CellPtr - sizeof(int), *(int*)(this.CellPtr-sizeof(int))+sizeof(int),size);
            targetPtr = this.CellPtr + (*(int*)this.CellPtr)+sizeof(int);
            *(int*)this.CellPtr += size;
            this.CellPtr += sizeof(int);
";
        }

        internal static string InsertAndRemoveAtCodeForContainer()
        {
            return @"
            this.CellPtr = this.ResizeFunction(this.CellPtr - 4, offset + 4, size);
            *(int*)this.CellPtr += size;
            this.CellPtr += 4;
";
        }

        internal static string GenerateAssignmentPrototypeParameterList(string header, StructDescriptor cell_desc, bool noFirstBlockSymbolAndNoParameterTypeAndNoDefaultValues = false, bool no_leading_comma = false, string extraParameterString = null)
        {
            /*
                    internal static unsafe byte[] construct(long CellID, int[,] _CMSketch, List<KVTuple> _keywordlist, List<long> _inlinks, List<long> _outlinks [[, extraParameterString goes here ]] )
                    { // <- if generateFirstBlockBeginSymbol == false, this "{" will not be generated
             */
            CodeWriter ret = new CodeWriter();
            ret += "\t\t" + header;
            if (cell_desc.Fields.Count > 0 && (!no_leading_comma))
            {
                ret += ", ";
            }
            foreach (Field field in cell_desc.Fields)
            {
                if (!noFirstBlockSymbolAndNoParameterTypeAndNoDefaultValues)
                {
                    if (cell_desc.OptionalFieldSequenceMap.ContainsKey(field))
                        ret += GetNullableTypeName(field) + " ";
                    else
                        ret += field.Type.CSharpName + " ";
                }
                ret += field.Name;
                if (noFirstBlockSymbolAndNoParameterTypeAndNoDefaultValues)
                {
                    ret += ",";
                }
                else
                {
                    if (field.DefaultValueString == null)
                    {
                        ret += "=" + GetDefaultString(field.Type, cell_desc.OptionalFieldSequenceMap.ContainsKey(field)) + ",";
                    }
                    else
                    {
                        ret += "=" + field.DefaultValueString + ",";
                    }
                }
            }
            if (cell_desc.Fields.Count != 0 || ret.Last() == ',')
                ret = ret.Remove(ret.Length - 1);

            if (extraParameterString != null)
            {
                if (cell_desc.Fields.Count > 0 || !no_leading_comma)
                {
                    ret += ", ";
                }
                ret += extraParameterString;
            }

            ret += ")";
            if (!noFirstBlockSymbolAndNoParameterTypeAndNoDefaultValues)
            {
                ret += "\r\n";
                ret += "\t\t{\r\n";
            }

            return ret;
        }

        internal static string GenerateAssignmentPrototypeParameterList2(string header, StructDescriptor cellformat, bool noFirstBlockSymbolAndNoParameterTypeAndNoDefaultValues = false)
        {
            /*
                internal static unsafe byte[] construct(long CellID, int[,] _CMSketch, List<KVTuple> _keywordlist, List<long> _inlinks, List<long> _outlinks)
                { // <- if generateFirstBlockBeginSymbol == false, this "{" will not be generated
             */
            string ret = "";
            ret += "\t\t" + header;
            foreach (Field field in cellformat.Fields)
            {
                if (!noFirstBlockSymbolAndNoParameterTypeAndNoDefaultValues)
                {
                    if (cellformat.OptionalFieldSequenceMap.ContainsKey(field))
                        ret += GetNullableTypeName(field) + " ";
                    else
                        ret += field.Type.CSharpName + " ";
                }
                ret += field.Name;
                if (noFirstBlockSymbolAndNoParameterTypeAndNoDefaultValues)
                {
                    ret += ",";
                }
                else
                {
                    if (field.DefaultValueString == null)
                    {
                        ret += "=" + GetDefaultString(field.Type, cellformat.OptionalFieldSequenceMap.ContainsKey(field)) + ",";
                    }
                    else
                    {
                        ret += "=" + field.DefaultValueString + ",";
                    }
                }
            }
            if (cellformat.Fields.Count != 0 || ret.Last() == ',')
                ret = ret.Remove(ret.Length - 1);
            ret += ")";
            if (!noFirstBlockSymbolAndNoParameterTypeAndNoDefaultValues)
            {
                ret += "\r\n";
            }

            return ret;
        }
    }
}
