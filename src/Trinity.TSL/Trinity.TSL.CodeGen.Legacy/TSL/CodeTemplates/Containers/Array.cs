using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Trinity.TSL.CodeTemplates;
using Trinity.Utilities;

namespace Trinity.TSL
{
    internal partial class ContainerCodeTemplate
    {
        #region GenerateArrayCode
        static public string GenerateArrayCode(ArrayType arraytype)
        {
            string ElementLength = arraytype.ElementType.Length.ToString(CultureInfo.InvariantCulture);
            string iterator_type_name = TSLCompiler.GetAccessorTypeName(false, arraytype.ElementType);
            string array_length = "Length * " + ElementLength;
            #region header
            CodeWriter ret = new CodeWriter();
            ret += @"
    public unsafe class " + arraytype.Name + @" : IEnumerable<" + iterator_type_name +  @">
    {
";
            for (int i = 1; i <= arraytype.lengths.Length; i++)
                ret += @"
        private static readonly int SizeDim" + i.ToString(CultureInfo.InvariantCulture) + " = " + arraytype.lengths[i - 1].ToString(CultureInfo.InvariantCulture) + @";";
            ret += @"
        /// <summary>
        /// Gets the rank (number of dimensions) of the Array.
        /// </summary>
        public static readonly int Rank = " + arraytype.lengths.Length.ToString(CultureInfo.InvariantCulture) + @";
        internal byte* CellPtr;
        internal long? CellID;
        internal " + arraytype.Name + @"(byte* _CellPtr)
        {
            this.CellPtr = _CellPtr;" +
            TSLCompiler.GenerateAccessorFieldAssignmentCodeForFixedField(arraytype.ElementType, false, "elementAccessor") + @"
        }";
            #endregion
            #region Length
            ret += @"
        /// <summary>
        /// Gets the total number of elements
        /// </summary>
        public int Length
        {
            get
            {
                return ";
            for (int i = 1; i <= arraytype.lengths.Length; i++)
                ret += "SizeDim" + i.ToString(CultureInfo.InvariantCulture) + "*";
            ret = ret.TrimEnd('*') + ";";
            ret += @"
            }
        }
";
            #endregion

            #region ToByteArray
            ret += @"
        /// <summary>
        /// Returns a byte array that contains the value of this instance.
        /// </summary>
        /// <returns>a byte array.</returns>
        public unsafe byte[] ToByteArray()
        {
            byte[] ret = new byte[Length * " + ElementLength + @"];
            fixed (byte* ptr = ret)
            {
                Memory.Copy(CellPtr, ptr, Length * " + ElementLength + @");
            }
            return ret;
        }
";
            #endregion
            #region Indexer
            /*
                    public unsafe int this[int indexDim1, int indexDim2]
                    {
                        get
                        {
             */
            string accessor_type_name = TSLCompiler.GetAccessorTypeName(false, arraytype.ElementType);
            ret += accessor_type_name + " elementAccessor ;";
            ret += @"
        /// <summary>
        /// Gets or sets the element at the specified index
        /// </summary>
        /// <returns>Corresponding element at the specified index</returns>
        public unsafe " + accessor_type_name + @" this[";
            for (int i = 1; i < arraytype.lengths.Length; i++)
                ret += "int indexDim" + i.ToString(CultureInfo.InvariantCulture) + ", ";
            ret += "int indexDim" + arraytype.lengths.Length.ToString(CultureInfo.InvariantCulture) + @"]
        {
            get
            {
";

            /*  [i_1,i_2,i_3,...,i_n]
             *  i_n + i_n-1 * sd_n + i_n-2 * sd_n * sd_n-1 + ... + i_1 * s_2 * s_3 * ... * s_n
             * 
             */
            string offset = "indexDim" + arraytype.lengths.Length.ToString(CultureInfo.InvariantCulture);
            string multiplier = "";
            string boundarycheck = "if (";
            int cnt = 0;
            for (int i = arraytype.lengths.Length; i > 1; i--)
            {
                if (cnt++ == 0) multiplier += "SizeDim" + i.ToString(CultureInfo.InvariantCulture);
                else multiplier += "*SizeDim" + i.ToString(CultureInfo.InvariantCulture);
                offset += "+indexDim" + (i - 1).ToString(CultureInfo.InvariantCulture) + "*" + multiplier;
                boundarycheck += "indexDim" + i.ToString(CultureInfo.InvariantCulture) + " > SizeDim" + i.ToString(CultureInfo.InvariantCulture) + " || indexDim" + i.ToString(CultureInfo.InvariantCulture) + " < 0 || ";
            }
            boundarycheck += "indexDim1 > SizeDim1 || indexDim1 < 0" + @") throw (new IndexOutOfRangeException(""The index is out of array size.""));";
            offset = "CellPtr + (" + offset + ") * " + ElementLength;



            /*
                            //if (indexH > sizeH - 1 || indexW > sizeW - 1 || indexH < 0 || indexW < 0) throw (new IndexOutOfRangeException("The index is out of array size."));
                            return *(int*)(CellPtr + indexDim1 * SizeDim2 * 4 + indexDim2 * 4);
                        }
                        set
                        {
                            //if (indexH > sizeH - 1 || indexW > sizeW - 1 || indexH < 0 || indexW < 0) throw (new IndexOutOfRangeException("The index is out of array size."));
                            *(int*)(CellPtr + indexDim1 * SizeDim2 * 4 + indexDim2 * 4) = value;
                        }
                    }
             * If the Element is Atom. Both get, set
             * Otherwise only get and need new object
            */
            if (arraytype.ElementType is AtomType || arraytype.ElementType is EnumType)
            {
                ret += @"                " + (TSLCompiler.CompileWithDebugFeatures ? boundarycheck : "") + @"    
                return *(" + arraytype.ElementType.Name + @"*)(" + offset + @");
            }
            set
            {
                " + (TSLCompiler.CompileWithDebugFeatures ? boundarycheck : "") + @"
                *(" + arraytype.ElementType.Name + @"*)(" + offset + @") = value;
            }
        }
";
            }
            else
            {
                /*
                            get
                            {
                                return new KVTuple_Accessor(CellPtr + index * 8);
                            }
                        }
                 */
                ret += @"
                elementAccessor.CellPtr = " + offset + @";
                return elementAccessor;
            }
            set
            {
                if ((object)value == null) throw new ArgumentNullException(""The assigned variable is null."");
            " + (TSLCompiler.CompileWithDebugFeatures ? boundarycheck : "") + @"
              Memory.Copy(value.CellPtr,(" + offset + ")," + arraytype.ElementType.Length + @");
            }
        }
";
            }
            #endregion
            #region Foreach
            ret += @"
        /// <summary>
        /// Performs the specified action on each element
        /// </summary>
        /// <param name=""action"">A lambda expression which has one parameter indicates element in array</param>
        public unsafe void ForEach(Action<" + iterator_type_name + @"> action)
        {
            byte* targetPtr = CellPtr;
            byte* endPtr = CellPtr + " + array_length + @";
            while( targetPtr < endPtr )
            {";
            if (arraytype.ElementType is AtomType || arraytype.ElementType is EnumType)
            {
                ret += @"
                action(*(" + arraytype.ElementType.Name + @"*)targetPtr);
                targetPtr += " + ElementLength + @";";
            }
            else
            {
                ret += @"
                elementAccessor.CellPtr = targetPtr;
                action(elementAccessor);
                targetPtr += " + ElementLength + @";";
            }
            ret += @"
            }
        }";
            ret += @"
        internal unsafe struct _iterator
        {
            byte* targetPtr;
            byte* endPtr;
            " + arraytype.Name +@" target;
            internal _iterator(" + arraytype.Name + @" target)
            {
                targetPtr   = target.CellPtr;
            	endPtr      = target.CellPtr + target." + array_length + @";
                this.target = target;
            }
            internal bool good()
            {
                return (targetPtr < endPtr);
            }
            internal " + iterator_type_name +@" current()
            {
                ";
            if (arraytype.ElementType is AtomType || arraytype.ElementType is EnumType)
            {
                ret += @"
                return (*(" + arraytype.ElementType.Name + @"*)targetPtr);";
            }
            else
            {
                ret += @"
                target.elementAccessor.CellPtr = targetPtr;
                return (target.elementAccessor);";
            }
            ret += @"
            }
            internal void move_next()
            {
                ";
            if (arraytype.ElementType is AtomType || arraytype.ElementType is EnumType)
            {
                ret += @"
                targetPtr += " + ElementLength + @";";
            }
            else
            {
                ret += @"
                targetPtr += " + ElementLength + @";";
            }
            ret += @"
            }
        }
        public IEnumerator<" + iterator_type_name + @"> GetEnumerator()
        {
            _iterator _it = new _iterator(this);
            while(_it.good())
            {
                yield return _it.current();
                _it.move_next();
            }
        }
        unsafe IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }";
            #endregion
            #region Clear
            ret += @"
        /// <summary>
        /// Sets a range of elements in the Array to zero, to false, or to null, depending on the element type.
        /// </summary>
        /// <param name=""index"">The starting index of the range of elements to clear.</param>
        /// <param name=""length"">The number of elements to clear.</param>
        public unsafe void Clear(int index, int length)
        {
            if (index < 0 || length < 0 ||index >= Length || index+length > Length ) throw new IndexOutOfRangeException();
            Memory.memset(CellPtr+index*" + ElementLength + @",0,(ulong)(length*" + ElementLength + @"));
        }
";
            #endregion
            ret += ImplicitOperatorCodeTemplate.GenerateArrayImplicitOperatorCode(arraytype);
            #region Override of "==" Operator
            ret += @"
        public static bool operator == (" + arraytype.Name + @" a, " + arraytype.Name + @" b)
        {
            if (ReferenceEquals(a, b))
              return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
              return false;
            // If both are same instance, return true.
            if (a.CellPtr == b.CellPtr) return true;
            return Memory.Compare(a.CellPtr, b.CellPtr, a.Length * " + arraytype.ElementType.Length.ToString(CultureInfo.InvariantCulture) + @");
        }

        public static bool operator != (" + arraytype.Name + @" a, " + arraytype.Name + @" b)
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
