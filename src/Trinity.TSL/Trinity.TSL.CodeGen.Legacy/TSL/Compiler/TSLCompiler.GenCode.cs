using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            //DONE, see AccessorInitialization.cpp
            return "";
        }

        internal static string GenerateAccessorFieldAssignmentCode(FieldType parent, FieldType fieldType, bool isReadOnly, string accessor_field_name, bool parentIsCell)
        {
            //DONE, see AccessorInitialization.cpp
            return "";
        }

        internal static string ResizeFunctionDelegate()
        {
            return "internal ResizeFunctionDelegate ResizeFunction;";
        }

        /// <summary>
        /// targetPtr will be ready for a Assign run at the newly allocated space
        /// </summary>
        /// <returns></returns>
        internal static string AppendCodeForContainer()
        {
            //DONE, see List.cs
            return "";
        }

        internal static string InsertAndRemoveAtCodeForContainer()
        {
            //DONE, see List.cs
            return "";
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
