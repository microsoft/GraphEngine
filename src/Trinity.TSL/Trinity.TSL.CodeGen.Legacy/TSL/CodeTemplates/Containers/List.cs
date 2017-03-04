using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Trinity.TSL.CodeTemplates;
using Trinity.Utilities;

namespace Trinity.TSL
{
    partial class ContainerCodeTemplate
    {
        #region GenerateListCode
        internal static string GenerateListCode(ListType listtype)
        {
            string iterator_type_name = TSLCompiler.GetAccessorTypeName(false, listtype.ElementFieldType);
            #region Header
            if (listtype.Name.Equals("longListAccessor"))
                return "";
            if (listtype.Name.Equals("doubleListAccessor"))
                return "";
            if (listtype.Name.Equals("byteListAccessor"))
                return "";
            if (listtype.Name.Equals("intListAccessor"))
                return "";

            string className = listtype.Name;
            CodeWriter cw = new CodeWriter();
            cw += @"
    public unsafe class " + className + " : IEnumerable<" + iterator_type_name + @">
    {
        internal byte* CellPtr;
        internal long? CellID;
        " + TSLCompiler.ResizeFunctionDelegate() + @"

        internal " + className + @"(byte* _CellPtr,ResizeFunctionDelegate func)
        {
            CellPtr = _CellPtr;
            ResizeFunction = func;
            CellPtr += 4;
";
            cw += TSLCompiler.GenerateAccessorFieldAssignmentCode(listtype, listtype.ElementFieldType, false, "elementAccessor", false);
            cw += @"
        }

        internal int length
        {
            get
            {
                return *(int*)(CellPtr-4);
            }
        }
";

            //ret += ScriptCompiler.GenerateAccessorFieldAssignmentCode(listtype.ElementFieldType, false, "elementAccessor");

            string accessor_type_name = TSLCompiler.GetAccessorTypeName(false, listtype.ElementFieldType);
            cw += accessor_type_name + " elementAccessor ;";
            #endregion
            int elementlength = 0;
            #region Count
            cw += @"
        /// <summary>
        /// Gets the number of elements actually contained in the List. 
        /// </summary>";
            if (listtype.ElementFieldType is FixedFieldType)
            {
                elementlength = (listtype.ElementFieldType as FixedFieldType).Length;
                cw += @"
        public unsafe int Count
        {
            get
            {
                return length / ";
                cw += elementlength.ToString(CultureInfo.InvariantCulture) + @";
            }
        }

";
            }
            else
            {
                cw += @"
        public unsafe int Count
        {
            get
            {
                byte* targetPtr = CellPtr;
                byte* endPtr = CellPtr + length;
                int ret = 0;
                while(targetPtr < endPtr)
                {
";
                cw += (listtype.ElementFieldType as DynamicFieldType).GeneratePushPointerCode();
                cw += @"
                    ++ret;
                }
                return ret;
            }
        }

";
            }
            #endregion
            #region Indexer
            cw += @"
        /// <summary>
        /// Gets or sets the element at the specified index. 
        /// </summary>
        /// <param name=""index"">Given index</param>
        /// <returns>Corresponding element at the specified index</returns>";
            if (listtype.ElementFieldType is AtomType || listtype.ElementFieldType is EnumType)
            {
                cw += @"
        public unsafe " + listtype.ElementFieldType.Name + @" this[int index]
        {
            get
            {";
                if (TSLCompiler.CompileWithDebugFeatures) cw += @"                if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException(""index is less than 0 or index is equal to or greater than Count."");";
                cw += @"
                return *(" + listtype.ElementFieldType.Name + "*)(CellPtr + index * " + elementlength.ToString(CultureInfo.InvariantCulture) + @");
            }
            set
            {";
                if (TSLCompiler.CompileWithDebugFeatures) cw += @"                if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException(""index is less than 0 or index is equal to or greater than Count."");";
                cw += @"
                *(" + listtype.ElementFieldType.Name + "*)(CellPtr + index * " + elementlength.ToString(CultureInfo.InvariantCulture) + @") = value;
            }
        }
";
            }
            else
            {
                string accessorName = TSLCompiler.GetAccessorTypeName(false, listtype.ElementFieldType);
                cw += @"
        public unsafe " + accessorName + @" this[int index]
        {
            get
            {";
                if (TSLCompiler.CompileWithDebugFeatures) cw += @"                if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException(""index is less than 0 or index is equal to or greater than Count."");";
                if (listtype.ElementFieldType is FixedFieldType)
                {
                    cw += @"
                elementAccessor.CellPtr = (this.CellPtr + index * " + elementlength.ToString(CultureInfo.InvariantCulture) + @");";
                    cw += "elementAccessor.CellID = this.CellID;";
                }
                else
                {
                    cw += @"
                byte* targetPtr = CellPtr;
                while(index-- > 0)
                {";
                    cw += (listtype.ElementFieldType as DynamicFieldType).GeneratePushPointerCode();
                    cw += @"
                }";
                    if (listtype.ElementFieldType is ListType || listtype.ElementFieldType is StringType || listtype.ElementFieldType is U8StringType)
                        cw += "elementAccessor.CellPtr = targetPtr + 4;";
                    else
                        cw += "elementAccessor.CellPtr = targetPtr;";
                    cw += "elementAccessor.CellID = this.CellID;";
                }
                cw += @"
                return elementAccessor;
            }
            set
            {";
                if (TSLCompiler.CompileWithDebugFeatures) cw += @"                if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException(""index is less than 0 or index is equal to or greater than Count."");";

                cw += @"
                if ((object)value == null) throw new ArgumentNullException(""The assigned variable is null."");
";
                if (listtype.ElementFieldType is FixedFieldType)
                {
                    cw += @"
                byte* targetPtr = CellPtr;";
                    cw += @"
                targetPtr += ( index * " + elementlength.ToString(CultureInfo.InvariantCulture) + @");";
                    cw += "elementAccessor.CellID = this.CellID;";
                }
                else
                {
                    cw += @"
                byte* targetPtr = CellPtr;
                while(index-- > 0)
                {";
                    cw += (listtype.ElementFieldType as DynamicFieldType).GeneratePushPointerCode();
                    cw += @"
                }";
                    cw += "elementAccessor.CellID = this.CellID;";
                }
                //**************
                Field field = new Field { Type = listtype.ElementFieldType };
                string accessor_field_name = "elementAccessor";
                #region Tip
                //Tip: to better read this piece of code,
                //Open one fold(region) a time. :-)
                //Some of them contains a setter inside.
                #endregion
                #region if (field.Type is StructFieldType || field.Type is DynamicArrayType || field.Type is ArrayType||Guid||DateTime)
                if (field.Type is StructFieldType || field.Type is ArrayType || field.Type is GuidType || field.Type is DateTimeType)
                {
                    if (field.Type is ArrayType)
                    {

                        cw += @"
                " + ImplicitOperatorCodeTemplate.ArraySetPropertiesCode(field, accessor_field_name, false);
                    }
                    else if (field.Type is StructFieldType)
                    {
                        cw += ImplicitOperatorCodeTemplate.FormatSetPropertiesCode(field, false, "elementAccessor");
                    }
                    else if (field.Type is GuidType)
                    {
                        cw += @"
                " + ImplicitOperatorCodeTemplate.GuidSetPropertiesCode(field, false);
                    }
                    else if (field.Type is DateTimeType)
                    {
                        cw += @"
                " + ImplicitOperatorCodeTemplate.DateTimeSetPropertiesCode(field, false);
                    }
                }
                #endregion
                #region else if (field.Type is ListType || field.Type is StringType || field.Type is U8StringType)
                else if (field.Type is ListType || field.Type is StringType || field.Type is U8StringType)
                {
                    cw += ImplicitOperatorCodeTemplate.ContainerListIndexerSetPropertiesCode(field.Type, accessor_field_name);
                }
                cw += @"                
            }
        }
";
            }
                #endregion
            //**************

            #endregion
            #region ToByteArray
            cw += @"
        /// <summary>
        /// Copies the elements to a new byte array
        /// </summary>
        /// <returns>Elements compactly arranged in a byte array.</returns>
        public unsafe byte[] ToByteArray()
        {
            byte[] ret = new byte[length];
            fixed (byte* retptr = ret)
            {
                Memory.Copy(CellPtr, retptr, length);
                return ret;
            }
        }
";
            #endregion
            #region ForEach
            cw += @"
        /// <summary>
        /// Performs the specified action on each elements
        /// </summary>
        /// <param name=""action"">A lambda expression which has one parameter indicates element in List</param>
        public unsafe void ForEach(Action<" + iterator_type_name + @"> action)
        {
            byte* targetPtr = CellPtr;
            byte* endPtr = CellPtr + length;
            while( targetPtr < endPtr )
            {";
            if (listtype.ElementFieldType is AtomType || listtype.ElementFieldType is EnumType)
            {
                cw += @"
                action(*(" + listtype.ElementFieldType.Name + @"*)targetPtr);
                targetPtr += " + elementlength.ToString(CultureInfo.InvariantCulture) + @";";
            }
            else if (listtype.ElementFieldType is FixedFieldType)
            {
                cw += @"
                elementAccessor.CellPtr = targetPtr;
                action(elementAccessor);
                targetPtr += " + elementlength.ToString(CultureInfo.InvariantCulture) + @";";
            }
            else if (listtype.ElementFieldType is DynamicStructFieldType)
            {
                cw += @"
                elementAccessor.CellPtr = targetPtr;
                action(elementAccessor);
                ";
                cw += (listtype.ElementFieldType as DynamicFieldType).GeneratePushPointerCode();
            }
            else
            {
                cw += @"
                elementAccessor.CellPtr = targetPtr + 4;
                action(elementAccessor);
                ";
                cw += (listtype.ElementFieldType as DynamicFieldType).GeneratePushPointerCode();
            }

            cw += @"
            }
        }
        /// <summary>
        /// Performs the specified action on each elements
        /// </summary>
        /// <param name=""action"">A lambda expression which has two parameters. First indicates element in the List and second the index of this element.</param>
        public unsafe void ForEach(Action<" + iterator_type_name + @",int> action)
        {
            byte* targetPtr = CellPtr;
            byte* endPtr = CellPtr + length;
            for(int index=0; targetPtr < endPtr;++index )
            {";
            if (listtype.ElementFieldType is AtomType || listtype.ElementFieldType is EnumType)
            {
                cw += @"
                action(*(" + listtype.ElementFieldType.Name + @"*)targetPtr,index);
                targetPtr += " + elementlength.ToString(CultureInfo.InvariantCulture) + @";";
            }
            else if (listtype.ElementFieldType is FixedFieldType)
            {
                cw += @"
                elementAccessor.CellPtr = targetPtr;
                action(elementAccessor,index);
                targetPtr += " + elementlength.ToString(CultureInfo.InvariantCulture) + @";";
            }
            else if (listtype.ElementFieldType is DynamicStructFieldType)
            {
                cw += @"
                elementAccessor.CellPtr = targetPtr;
                action(elementAccessor,index);
                ";
                cw += (listtype.ElementFieldType as DynamicFieldType).GeneratePushPointerCode();
            }
            else
            {
                cw += @"
                elementAccessor.CellPtr = targetPtr + 4;
                action(elementAccessor,index);
                ";
                cw += (listtype.ElementFieldType as DynamicFieldType).GeneratePushPointerCode();
            }

            cw += @"
            }
        }";
            cw += @"
        internal unsafe struct _iterator
        {
            byte* targetPtr;
            byte* endPtr;
            " + className +@" target;
            internal _iterator(" + className + @" target)
            {
                targetPtr = target.CellPtr;
                endPtr = targetPtr + target.length;
                this.target = target;
            }
            internal bool good()
            {
                return (targetPtr < endPtr);
            }
            internal " + iterator_type_name +@" current()
            {
                ";
            if (listtype.ElementFieldType is AtomType || listtype.ElementFieldType is EnumType)
            {
                cw += @"
                return (*(" + listtype.ElementFieldType.Name + @"*)targetPtr);
                ";
            }
            else if (listtype.ElementFieldType is FixedFieldType)
            {
                cw += @"
                target.elementAccessor.CellPtr = targetPtr;
                return (target.elementAccessor);";
            }
            else if (listtype.ElementFieldType is DynamicStructFieldType)
            {
                cw += @"
                target.elementAccessor.CellPtr = targetPtr;
                return (target.elementAccessor);
                ";
            }
            else
            {
                cw += @"
                target.elementAccessor.CellPtr = targetPtr + 4;
                return (target.elementAccessor);
                ";
            }
            cw += @"
            }
            internal void move_next()
            {
                ";
            if (listtype.ElementFieldType is AtomType || listtype.ElementFieldType is EnumType)
            {
                cw += @"
                targetPtr += " + elementlength.ToString(CultureInfo.InvariantCulture) + @";";
            }
            else if (listtype.ElementFieldType is FixedFieldType)
            {
                cw += @"
                targetPtr += " + elementlength.ToString(CultureInfo.InvariantCulture) + @";";
            }
            else if (listtype.ElementFieldType is DynamicStructFieldType)
            {
                cw += (listtype.ElementFieldType as DynamicFieldType).GeneratePushPointerCode();
            }
            else
            {
                cw += (listtype.ElementFieldType as DynamicFieldType).GeneratePushPointerCode();
            }
            cw += @"
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
            #region Add/Insert/RemoveAt
            cw += @"
        /// <summary>
        /// Adds an item to the end of the List
        /// </summary>
        /// <param name=""element"">The object to be added to the end of the List.</param>
        public unsafe void Add(" + listtype.ElementFieldType.CSharpName + @" element)
        {
            byte* targetPtr = null;
            {
            " + listtype.ElementFieldType.GenerateAssignCodeForConstructor("element", 0, true) + @"
            }" +
             TSLCompiler.AppendCodeForContainer() +
                listtype.ElementFieldType.GenerateAssignCodeForConstructor("element", 0, false) + @"
        }
        /// <summary>
        /// Inserts an element into the List at the specified index.
        /// </summary>
        /// <param name=""index"">The zero-based index at which item should be inserted.</param>
        /// <param name=""element"">The object to insert.</param>
        public unsafe void Insert(int index, " + listtype.ElementFieldType.CSharpName + @" element)
        {
            if (index < 0 || index > Count) throw new IndexOutOfRangeException();
            byte* targetPtr = null;
            {
            " + listtype.ElementFieldType.GenerateAssignCodeForConstructor("element", 0, true) + @"
            }
            int size = (int)targetPtr;

            targetPtr = CellPtr;";
            if (listtype.ElementFieldType is FixedFieldType)
            {
                FixedFieldType fft = listtype.ElementFieldType as FixedFieldType;
                cw.WL("targetPtr += index * {0};", fft.Length);
            }
            else
            {
                cw += @"
            for(int i = 0; i < index; i++)
            {
            " + listtype.ElementFieldType.GeneratePushPointerCode() + @"
            }";
            }
            cw += @"
            int offset = (int)(targetPtr - CellPtr);
            " + TSLCompiler.InsertAndRemoveAtCodeForContainer() + @"
            targetPtr = this.CellPtr + offset;
            " + listtype.ElementFieldType.GenerateAssignCodeForConstructor("element", 0, false) + @"
        }";

            cw += @"
        /// <summary>
        /// Inserts an element into the List at the specified index.
        /// </summary>
        /// <param name=""index"">The zero-based index at which item should be inserted.</param>
        /// <param name=""element"">The object to insert.</param>
";
            cw.WL2("public unsafe void Insert({0} element, Comparison<{1}> comparison)", listtype.ElementFieldType.CSharpName, accessor_type_name);
            cw +=
         @"
        {
            byte* targetPtr = null;
            {
            " + listtype.ElementFieldType.GenerateAssignCodeForConstructor("element", 0, true) + @"
            }
            int size = (int)targetPtr;
            targetPtr = CellPtr;
            byte* endPtr = CellPtr + length;
            while( targetPtr < endPtr )
            {";
            if (listtype.ElementFieldType is AtomType || listtype.ElementFieldType is EnumType)
            {
                cw += @"
                if(comparison(*(" + listtype.ElementFieldType.Name + @"*)targetPtr, element) <=0 )
                {
                    targetPtr += " + elementlength.ToString(CultureInfo.InvariantCulture) + @";
                }
                else
                {
                    break;
                }";
            }
            else if (listtype.ElementFieldType is FixedFieldType)
            {
                cw += @"
                elementAccessor.CellPtr = targetPtr;
                if(comparison(elementAccessor, element)<=0)
                {
                    targetPtr += " + elementlength.ToString(CultureInfo.InvariantCulture) + @";
                }
                else
                {
                    break;
                }";
            }
            else if (listtype.ElementFieldType is DynamicStructFieldType)
            {
                cw += @"
                elementAccessor.CellPtr = targetPtr;
                if(comparison(elementAccessor, element)<=0)
                {
                ";
                cw += (listtype.ElementFieldType as DynamicFieldType).GeneratePushPointerCode();
                cw+=@"
                }
                else
                {
                    break;
                }";
            }
            else
            {
                cw += @"
                elementAccessor.CellPtr = targetPtr + 4;
                if(comparison(elementAccessor, element)<=0)
                {
                ";
                cw += (listtype.ElementFieldType as DynamicFieldType).GeneratePushPointerCode();
                cw+=@"
                }
                else
                {
                    break;
                }";
            }

            cw += @"
            }";

            cw += @"
            int offset = (int)(targetPtr - CellPtr);
            " + TSLCompiler.InsertAndRemoveAtCodeForContainer() + @"
            targetPtr = this.CellPtr + offset;
            " + listtype.ElementFieldType.GenerateAssignCodeForConstructor("element", 0, false) + @"
        }

        /// <summary>
        /// Removes the element at the specified index of the List.
        /// </summary>
        /// <param name=""index"">The zero-based index of the element to remove.</param>
        public unsafe void RemoveAt(int index)
        {
            if (index < 0 || index >= Count) throw new IndexOutOfRangeException();

            byte* targetPtr = CellPtr;    
            for(int i = 0; i < index; i++)
            {
            " + listtype.ElementFieldType.GeneratePushPointerCode() + @"
            }
            int offset = (int)(targetPtr - CellPtr);
            byte* oldtargetPtr = targetPtr;
            " + listtype.ElementFieldType.GeneratePushPointerCode() + @"
            int size = (int)(oldtargetPtr - targetPtr);
            " + TSLCompiler.InsertAndRemoveAtCodeForContainer() + @"
        }
";
            #endregion
            #region AddRange
            cw += @"
        /// <summary>
        /// Adds the elements of the specified collection to the end of the List
        /// </summary>
        /// <param name=""collection"">The collection whose elements should be added to the end of the List. The collection itself cannot be null.</param>
        public unsafe void AddRange(" + listtype.CSharpName + @" collection)
        {
            if (collection == null) throw new ArgumentNullException(""collection is null."");
            " + listtype.Name + @" tcollection = collection;
            int delta = tcollection.length;
            CellPtr = ResizeFunction(CellPtr - 4, *(int*)(CellPtr - 4) + 4, delta);
            Memory.Copy(tcollection.CellPtr, CellPtr + *(int*)CellPtr + 4, delta);
            *(int*)CellPtr += delta;
            this.CellPtr += 4;
        }
        /// <summary>
        /// Adds the elements of the specified collection to the end of the List
        /// </summary>
        /// <param name=""collection"">The collection whose elements should be added to the end of the List. The collection itself cannot be null.</param>
        public unsafe void AddRange(" + listtype.Name + @" collection)
        {
            if (collection == null) throw new ArgumentNullException(""collection is null."");
            int delta = collection.length;
            if (collection.CellID != CellID)
            {
                CellPtr = ResizeFunction(CellPtr - 4, *(int*)(CellPtr - 4) + 4, delta);
                Memory.Copy(collection.CellPtr, CellPtr + *(int*)CellPtr + 4, delta);
                *(int*)CellPtr += delta;
            }
            else
            {
                byte[] tmpcell = new byte[delta];
                fixed (byte* tmpcellptr = tmpcell)
                {
                    Memory.Copy(collection.CellPtr, tmpcellptr, delta);
                    CellPtr = ResizeFunction(CellPtr - 4, *(int*)(CellPtr - 4) + 4, delta);
                    Memory.Copy(tmpcellptr, CellPtr + *(int*)CellPtr + 4, delta);
                    *(int*)CellPtr += delta;
                }
            }
            this.CellPtr += 4;
        }
";
            #endregion
            #region Clear
            cw += @"
        /// <summary>
        /// Removes all elements from the List
        /// </summary>
        public unsafe void Clear()
        {
            int delta = length;
            Memory.memset(CellPtr, 0, (ulong)delta);
            CellPtr = ResizeFunction(CellPtr - 4, 4, -delta);
            *(int*)CellPtr = 0;
            this.CellPtr += 4;
        }
";
            #endregion
            #region Contains
            cw += @"
        /// <summary>
        /// Determines whether an element is in the List
        /// </summary>
        /// <param name=""item"">The object to locate in the List.The value can be null for non-atom types</param>
        /// <returns>true if item is found in the List; otherwise, false.</returns>
";
            if (listtype.ElementFieldType is StructFieldType)
                cw += @"        public unsafe bool Contains(" + listtype.ElementFieldType.Name + @"_Accessor item)";
            else
                cw += @"        public unsafe bool Contains(" + listtype.ElementFieldType.Name + @" item)";
            cw += @"
        {
            bool ret = false;
            ForEach(x =>
            {
                if (item == x) ret = true;
            });
            return ret;
        }
";
            #endregion
            #region Exists
            cw += @"
        /// <summary>
        /// Determines whether the List contains elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name=""match"">The Predicate delegate that defines the conditions of the elements to search for.</param>
        /// <returns>true if the List contains one or more elements that match the conditions defined by the specified predicate; otherwise, false.</returns>
        public unsafe bool Exists(Predicate<" + listtype.ElementFieldType.CSharpName + @"> match)
        {
            bool ret = false;
            ForEach(x => {
                if (match(x)) ret = true;
            });
            return ret;
        }
    ";
            #endregion
            #region CopyTo
            cw += @"
        /// <summary>
        /// Copies the entire List to a compatible one-dimensional array, starting at the beginning of the ptr1 array.
        /// </summary>
        /// <param name=""array"">The one-dimensional Array that is the destination of the elements copied from List. The Array must have zero-based indexing.</param>
        public unsafe void CopyTo(" + listtype.ElementFieldType.CSharpName + @"[] array)
        {
            if (array == null) throw new ArgumentNullException(""array is null."");
            if (array.Length < Count) throw new ArgumentException(""The number of elements in the source List is greater than the number of elements that the destination array can contain."");
            ForEach((x, i) => array[i] = x);
        }

        /// <summary>
        /// Copies the entire List to a compatible one-dimensional array, starting at the specified index of the ptr1 array.
        /// </summary>
        /// <param name=""array"">The one-dimensional Array that is the destination of the elements copied from List. The Array must have zero-based indexing.</param>
        /// <param name=""arrayIndex"">The zero-based index in array at which copying begins.</param>
        public unsafe void CopyTo(" + listtype.ElementFieldType.CSharpName + @"[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(""array is null."");
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(""arrayIndex is less than 0."");
            if (array.Length - arrayIndex < Count) throw new ArgumentException(""The number of elements in the source List is greater than the available space from arrayIndex to the end of the destination array."");
            ForEach((x, i) => array[i + arrayIndex] = x);
        }

        /// <summary>
        /// Copies a range of elements from the List to a compatible one-dimensional array, starting at the specified index of the ptr1 array.
        /// </summary>
        /// <param name=""index"">The zero-based index in the source List at which copying begins.</param>
        /// <param name=""array"">The one-dimensional Array that is the destination of the elements copied from List. The Array must have zero-based indexing.</param>
        /// <param name=""arrayIndex"">The zero-based index in array at which copying begins.</param>;
        /// <param name=""count"">The number of elements to copy.</param>
        public unsafe void CopyTo(int index, " + listtype.ElementFieldType.CSharpName + @"[] array, int arrayIndex, int count)
        {
            if (array == null) throw new ArgumentNullException(""array is null."");
            if (arrayIndex < 0 || index < 0 || count < 0) throw new ArgumentOutOfRangeException(""arrayIndex is less than 0 or index is less than 0 or count is less than 0."");
            if (array.Length - arrayIndex < Count - index) throw new ArgumentException(""The number of elements from index to the end of the source List is greater than the available space from arrayIndex to the end of the destination array. "");
            if (index >= Count) throw new ArgumentException(""index is equal to or greater than the Count of the source List."");
            int j = 0;
            for (int i = index; i < index + count; i++)
            {
                array[j + arrayIndex] = this[i];
                ++j;
            }
        }
";
            #endregion
            #region InsertRange
            cw += @"
        /// <summary>
        /// Inserts the elements of a collection into the List at the specified index.
        /// </summary>
        /// <param name=""index"">The zero-based index at which the new elements should be inserted.</param>
        /// <param name=""collection"">The collection whose elements should be inserted into the List. The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
        public unsafe void InsertRange(int index, " + listtype.CSharpName + @" collection)
        {
            if (collection == null) throw new ArgumentNullException(""collection is null."");
            if (index < 0) throw new ArgumentOutOfRangeException(""index is less than 0."");
            if (index > Count) throw new ArgumentOutOfRangeException(""index is greater than Count."");
            " + listtype.Name + @" tmpAccessor = collection;
            byte* targetPtr = CellPtr;
            for (int i = 0; i < index; i++)
            {
                ";
            cw += listtype.ElementFieldType.GeneratePushPointerCode();
            cw += @"
            }
            int offset = (int)(targetPtr - CellPtr);
            CellPtr = ResizeFunction(CellPtr - 4, offset + 4, tmpAccessor.length);
            Memory.Copy(tmpAccessor.CellPtr, CellPtr + offset + 4, tmpAccessor.length);
            *(int*)CellPtr += tmpAccessor.length;
            this.CellPtr += 4;
        }
";
            #endregion
            #region RemoveRange
            cw += @"
        /// <summary>
        /// Removes a range of elements from the List.
        /// </summary>
        /// <param name=""index"">The zero-based starting index of the range of elements to remove.</param>
        /// <param name=""count"">The number of elements to remove.</param>
        public unsafe void RemoveRange(int index, int count)
        {
            if (index < 0) throw new ArgumentOutOfRangeException(""index is less than 0."");
            if (index > Count) throw new ArgumentOutOfRangeException(""index is greater than Count."");
            if (index + count > Count) throw new ArgumentException(""index and count do not denote a valid range of elements in the List."");
            byte* targetPtr = CellPtr;
            for (int i = 0; i < index; i++)
            {";
            cw += listtype.ElementFieldType.GeneratePushPointerCode();
            cw += @"}
            int offset = (int)(targetPtr - CellPtr);
            byte* oldtargetPtr = targetPtr;
            for (int i = 0; i < count; i++)
            {";
            cw += listtype.ElementFieldType.GeneratePushPointerCode();
            cw += @"}
            int size = (int)(oldtargetPtr - targetPtr);
            CellPtr = ResizeFunction(CellPtr - 4, offset + 4, size);
            *(int*)CellPtr += size;
            this.CellPtr += 4;
        }
";
            #endregion
            //if(!ReadOnly)
            cw += ImplicitOperatorCodeTemplate.GenerateListImplicitOperaterCode(listtype);
            #region Override of "==" Operator
            cw += @"
        public static bool operator == (" + listtype.Name + @" a, " + listtype.Name + @" b)
        {
            if (ReferenceEquals(a, b))
              return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
              return false;
            // If both are same instance, return true.
            if (a.CellPtr == b.CellPtr) return true;
            // If length not equal, return false.
            if (a.length != b.length) return false;
            return Memory.Compare(a.CellPtr, b.CellPtr, a.length);
        }

        public static bool operator !=(" + listtype.Name + @" a, " + listtype.Name + @" b)
        {
            return !(a == b);
        }
";
            #endregion

            cw += @"
    }
";
            return cw;
        }
        #endregion
    }
}
