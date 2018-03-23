using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity.Core.Lib;
using Trinity.TSL;
using Trinity.TSL.Lib;
using Trinity.Storage;

/*MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")*/
namespace t_Namespace
{
    /// <summary>
    /// Represents a TSL double list corresponding to List{double}.
    /// </summary>
    [TARGET("NTSL")]
    public unsafe class doubleListAccessor : IAccessor, IEnumerable<double>
    {
        internal byte* m_ptr;
        internal long CellId;

        internal doubleListAccessor(byte* _CellPtr, ResizeFunctionDelegate func)
        {
            m_ptr = _CellPtr;
            ResizeFunction = func;
            m_ptr += sizeof(int);
        }

        internal int length
        {
            get
            {
                return *(int*)(m_ptr - sizeof(int));
            }
        }

        #region IAccessor Implementation

        /// <summary>
        /// Copies the elements to a new byte array
        /// </summary>
        /// <returns>Elements compactly arranged in a byte array.</returns>
        public unsafe byte[] ToByteArray()
        {
            byte[] ret = new byte[length];
            fixed (byte* retptr = ret)
            {
                Memory.Copy(m_ptr, retptr, length);
                return ret;
            }
        }

        /// <summary>
        /// Get the pointer to the underlying buffer.
        /// </summary>
        public unsafe byte* GetUnderlyingBufferPointer()
        {
            return m_ptr - sizeof(int);
        }

        /// <summary>
        /// Get the length of the buffer.
        /// </summary>
        public unsafe int GetBufferLength()
        {
            return length + sizeof(int);
        }

        /// <summary>
        /// The ResizeFunctionDelegate that should be called when this accessor is trying to resize itself.
        /// </summary>
        public ResizeFunctionDelegate ResizeFunction { get; set; }

        #endregion

        /// <summary>
        /// Gets the number of double elements actually contained in the List. 
        /// </summary>
        public unsafe int Count
        {
            get
            {
                return length >> 3;
            }
        }

        internal double* GetUnsafePointer()
        {
            return (double*)m_ptr;
        }

        /// <summary>
        /// Gets or sets the element at the specified index. 
        /// </summary>
        /// <param name="index">Given index</param>
        /// <returns>Corresponding element at the specified index</returns>
        public unsafe double this[int index]
        {
            get
            {
                return *(double*)(m_ptr + (index << 3));
            }
            set
            {
                *(double*)(m_ptr + (index << 3)) = value;
            }
        }

        /// <summary>
        /// Copies the elements to a new double array.
        /// </summary>
        /// <returns>An array of double.</returns>
        public unsafe double[] ToArray()
        {
            double[] ret = new double[length >> 3];
            fixed (double* retptr = ret)
            {
                Memory.Copy(m_ptr, retptr, length);
            }
            return ret;
        }

        /// <summary>
        /// Performs the specified action on each elements
        /// </summary>
        /// <param name="action">A lambda expression which has one parameter indicates element in List</param>
        public unsafe void ForEach(Action<double> action)
        {
            byte* targetPtr = m_ptr;
            byte* endPtr = m_ptr + length;
            while (targetPtr < endPtr)
            {
                action(*(double*)targetPtr);
                targetPtr += 8;
            }
        }
        /// <summary>
        /// Performs the specified action on each elements
        /// </summary>
        /// <param name="action">A lambda expression which has two parameters. First indicates element in the List and second the index of this element.</param>
        public unsafe void ForEach(Action<double, int> action)
        {
            byte* targetPtr = m_ptr;
            byte* endPtr = m_ptr + length;
            for (int index = 0; targetPtr < endPtr; ++index)
            {
                action(*(double*)targetPtr, index);
                targetPtr += 8;
            }
        }

        internal unsafe struct _iterator
        {
            byte* targetPtr;
            byte* endPtr;
            internal _iterator(doubleListAccessor target)
            {
                targetPtr = target.m_ptr;
                endPtr    = target.m_ptr + target.length;
            }

            internal bool good()
            {
                return (targetPtr < endPtr);
            }

            internal double current()
            {
                return *(double*)targetPtr;
            }

            internal void move_next()
            {
                targetPtr += 8;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterate through current list.
        /// </summary>
        /// <returns>
        /// An IEnumerator object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<double> GetEnumerator()
        {
            _iterator it = new _iterator(this);
            while (it.good())
            {
                yield return it.current();
                it.move_next();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds an item to the end of the List
        /// </summary>
        /// <param name="element">The object to be added to the end of the List.</param>
        public unsafe void Add(double element)
        {
            int size = sizeof(double);
            this.m_ptr = this.ResizeFunction(this.m_ptr - sizeof(int), *(int*)(this.m_ptr - sizeof(int)) + sizeof(int), size);
            byte* targetPtr = this.m_ptr + (*(int*)this.m_ptr) + sizeof(int);
            *(int*)this.m_ptr += size;
            this.m_ptr += sizeof(int);
            *(double*)targetPtr = element;
        }

        /// <summary>
        /// Inserts an element into the List at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="element">The object to insert.</param>
        public unsafe void Insert(int index, double element)
        {
            if (index < 0 || index > Count) throw new IndexOutOfRangeException();
            int size = sizeof(double);

            byte* targetPtr = m_ptr + (index << 3);
            int offset = (int)(targetPtr - m_ptr);
            this.m_ptr = this.ResizeFunction(this.m_ptr - 4, offset + 4, size);
            *(int*)this.m_ptr += size;
            this.m_ptr += 4;
            targetPtr = this.m_ptr + offset;
            *(double*)targetPtr = element;
        }

        /// <summary>
        /// Inserts an element into the sorted List using the specified Comparison delegate.
        /// </summary>
        /// <param name="element">The element to insert.</param>
        /// <param name="comparison">The Comparison delegate.</param>
        public unsafe void Insert(double element, Comparison<double> comparison)
        {
            int size = sizeof(double);
            byte* targetPtr = m_ptr;
            byte* endPtr = m_ptr + length;
            while (targetPtr < endPtr)
            {
                if (comparison(*(double*)targetPtr, element) <= 0)
                {
                    targetPtr += sizeof(double);
                }
                else
                {
                    break;
                }
            }
            int offset = (int)(targetPtr - m_ptr);

            this.m_ptr = this.ResizeFunction(this.m_ptr - 4, offset + 4, size);
            *(int*)this.m_ptr += size;
            this.m_ptr += 4;

            targetPtr = this.m_ptr + offset;
            *(double*)targetPtr = element;
        }

        /// <summary>
        /// Removes the element at the specified index of the List.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public unsafe void RemoveAt(int index)
        {
            if (index < 0 || index >= Count) throw new IndexOutOfRangeException();

            byte* targetPtr = m_ptr + (index << 3);
            int offset = (int)(targetPtr - m_ptr);
            int size = -sizeof(double);

            this.m_ptr = this.ResizeFunction(this.m_ptr - 4, offset + 4, size);
            *(int*)this.m_ptr += size;
            this.m_ptr += 4;
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the List
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the end of the List. The collection itself cannot be null.</param>
        public unsafe void AddRange(List<double> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection is null.");
            doubleListAccessor tcollection = collection;
            int delta = tcollection.length;
            m_ptr = ResizeFunction(m_ptr - 4, *(int*)(m_ptr - 4) + 4, delta);
            Memory.Copy(tcollection.m_ptr, m_ptr + *(int*)m_ptr + 4, delta);
            *(int*)m_ptr += delta;
            this.m_ptr += 4;
        }

        /// <summary>
        /// Adds the array of double elements to the end of the List.
        /// </summary>
        /// <param name="collection">The array of double elements.</param>
        public unsafe void AddRange(double[] collection)
        {
            if (collection == null) return;
            int delta = collection.Length << 3;
            m_ptr = ResizeFunction(m_ptr - 4, *(int*)(m_ptr - 4) + 4, delta);
            fixed (double* dp = collection)
            {
                Memory.Copy(dp, m_ptr + *(int*)m_ptr + 4, delta);
            }
            *(int*)m_ptr += delta;
            this.m_ptr += 4;
        }

        /// <summary>
        /// Adds the array of double elements to the end of the List starting from the specified index.
        /// </summary>
        /// <param name="collection">The array of double elements.</param>
        /// <param name="startIndex">The start position of the double array from which we copy the elements.</param>
        /// <param name="count">The number of elements to copy.</param>
        public unsafe void AddRange(double[] collection, int startIndex, int count)
        {
            if (collection == null) return;
            int delta = count << 3;
            m_ptr = ResizeFunction(m_ptr - 4, *(int*)(m_ptr - 4) + 4, delta);
            fixed (double* dp = &collection[startIndex])
            {
                Memory.Copy(dp, m_ptr + *(int*)m_ptr + 4, delta);
            }
            *(int*)m_ptr += delta;
            this.m_ptr += 4;
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the List
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the end of the List. The collection itself cannot be null.</param>
        public unsafe void AddRange(doubleListAccessor collection)
        {
            if (collection == null) throw new ArgumentNullException("collection is null.");
            int delta = collection.length;
            if (collection.CellId != CellId)
            {
                m_ptr = ResizeFunction(m_ptr - 4, *(int*)(m_ptr - 4) + 4, delta);
                Memory.Copy(collection.m_ptr, m_ptr + *(int*)m_ptr + 4, delta);
                *(int*)m_ptr += delta;
            }
            else
            {
                byte[] tmpcell = new byte[delta];
                fixed (byte* tmpcellptr = tmpcell)
                {
                    Memory.Copy(collection.m_ptr, tmpcellptr, delta);
                    m_ptr = ResizeFunction(m_ptr - 4, *(int*)(m_ptr - 4) + 4, delta);
                    Memory.Copy(tmpcellptr, m_ptr + *(int*)m_ptr + 4, delta);
                    *(int*)m_ptr += delta;
                }
            }
            this.m_ptr += 4;
        }

        /// <summary>
        /// Removes all elements from the List
        /// </summary>
        public unsafe void Clear()
        {
            int delta = length;
            Memory.memset(m_ptr, 0, (ulong)delta);
            m_ptr = ResizeFunction(m_ptr - 4, 4, -delta);
            *(int*)m_ptr = 0;
            this.m_ptr += 4;
        }

        /// <summary>
        /// Determines whether an element is in the List
        /// </summary>
        /// <param name="item">The object to locate in the List.The value can be null for non-atom types</param>
        /// <returns>true if item is found in the List; otherwise, false.</returns>
        public unsafe bool Contains(double item)
        {
            bool ret = false;
            ForEach(x =>
            {
                if (item == x) ret = true;
            });
            return ret;
        }

        /// <summary>
        /// Determines whether the List contains elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The Predicate delegate that defines the conditions of the elements to search for.</param>
        /// <returns>true if the List contains one or more elements that match the conditions defined by the specified predicate; otherwise, false.</returns>
        public unsafe bool Exists(Predicate<double> match)
        {
            bool ret = false;
            ForEach(x =>
            {
                if (match(x)) ret = true;
            });
            return ret;
        }

        /// <summary>
        /// Copies the entire List to a compatible one-dimensional array, starting at the beginning of the ptr1 array.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from List. The Array must have zero-based indexing.</param>
        public unsafe void CopyTo(double[] array)
        {
            if (array == null) throw new ArgumentNullException("array is null.");
            if (array.Length < Count) throw new ArgumentException("The number of elements in the source List is greater than the number of elements that the destination array can contain.");
            //ForEach((x, i) => array[i] = x);
            fixed (double* dp = array)
            {
                Memory.Copy(m_ptr, 0, dp, 0, length);
            }
        }

        /// <summary>
        /// Copies the entire List to a compatible one-dimensional array, starting at the specified index of the ptr1 array.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from List. The Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public unsafe void CopyTo(double[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException("array is null.");
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException("arrayIndex is less than 0.");
            if (array.Length - arrayIndex < Count) throw new ArgumentException("The number of elements in the source List is greater than the available space from arrayIndex to the end of the destination array.");
            //ForEach((x, i) => array[i + arrayIndex] = x);
            fixed (double* dp = array)
            {
                Memory.Copy(m_ptr, 0, dp, arrayIndex * sizeof(double), length);
            }
        }

        /// <summary>
        /// Copies a range of elements from the List to a compatible one-dimensional array, starting at the specified index of the ptr1 array.
        /// </summary>
        /// <param name="index">The zero-based index in the source List at which copying begins.</param>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from List. The Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>;
        /// <param name="count">The number of elements to copy.</param>
        public unsafe void CopyTo(int index, double[] array, int arrayIndex, int count)
        {
            if (array == null) throw new ArgumentNullException("array is null.");
            if (arrayIndex < 0 || index < 0 || count < 0) throw new ArgumentOutOfRangeException("arrayIndex is less than 0 or index is less than 0 or count is less than 0.");
            if (array.Length - arrayIndex < Count - index) throw new ArgumentException("The number of elements from index to the end of the source List is greater than the available space from arrayIndex to the end of the destination array. ");
            if (index >= Count) throw new ArgumentException("index is equal to or greater than the Count of the source List.");
            fixed (double* dp = array)
            {
                Memory.Copy(m_ptr, index * sizeof(double), dp, arrayIndex * sizeof(double), count * sizeof(double));
            }
        }

        /// <summary>
        /// Inserts the elements of a collection into the List at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="collection">The collection whose elements should be inserted into the List. The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
        public unsafe void InsertRange(int index, List<double> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection is null.");
            if (index < 0) throw new ArgumentOutOfRangeException("index is less than 0.");
            if (index > Count) throw new ArgumentOutOfRangeException("index is greater than Count.");
            doubleListAccessor tmpAccessor = collection;
            byte* targetPtr = m_ptr + (index << 3);
            int offset = (int)(targetPtr - m_ptr);
            m_ptr = ResizeFunction(m_ptr - 4, offset + 4, tmpAccessor.length);
            Memory.Copy(tmpAccessor.m_ptr, m_ptr + offset + 4, tmpAccessor.length);
            *(int*)m_ptr += tmpAccessor.length;
            this.m_ptr += 4;
        }

        /// <summary>
        /// Removes a range of elements from the List.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        public unsafe void RemoveRange(int index, int count)
        {
            if (index < 0) throw new ArgumentOutOfRangeException("index is less than 0.");
            if (index > Count) throw new ArgumentOutOfRangeException("index is greater than Count.");
            if (index + count > Count) throw new ArgumentException("index and count do not denote a valid range of elements in the List.");
            byte* targetPtr = m_ptr + (index << 3);
            int offset = (int)(targetPtr - m_ptr);
            int size = -(count << 3);
            m_ptr = ResizeFunction(m_ptr - 4, offset + 4, size);
            *(int*)m_ptr += size;
            this.m_ptr += 4;
        }

        /// <summary>
        /// Implicitly converts a doubleList instance to a List{double} instance.
        /// </summary>
        /// <param name="accessor">The doubleList instance.</param>
        /// <returns>The List{double} instance.</returns>
        public unsafe static implicit operator List<double>(doubleListAccessor accessor)
        {
            if ((object)accessor == null) return null;
            List<double> list = new List<double>();
            accessor.ForEach(element => list.Add(element));
            return list;
        }

        /// <summary>
        /// Implicitly converts a List{double} instance to a doubleList instance.
        /// </summary>
        /// <param name="value">The List{double} instance.</param>
        /// <returns>The doubleList instance.</returns>
        public unsafe static implicit operator doubleListAccessor(List<double> value)
        {
            byte* targetPtr = null;

            if (value != null)
            {
                targetPtr += value.Count * 8 + sizeof(int);
            }
            else
            {
                targetPtr += sizeof(int);
            }

            byte* tmpcellptr = BufferAllocator.AllocBuffer((int)targetPtr);
            targetPtr = tmpcellptr;

            if (value != null)
            {
                *(int*)targetPtr = value.Count * 8;
                targetPtr += sizeof(int);
                for (int iterator_0 = 0; iterator_0 < value.Count; ++iterator_0)
                {
                    *(double*)targetPtr = value[iterator_0];
                    targetPtr += 8;
                }
            }
            else
            {
                *(int*)targetPtr = 0;
                targetPtr += sizeof(int);
            }

            doubleListAccessor ret = new doubleListAccessor(tmpcellptr, null);
            return ret;
        }

        /// <summary>
        /// Determines whether two specified doubleList have the same value.
        /// </summary>
        /// <param name="a">The first doubleList to compare, or null. </param>
        /// <param name="b">The second doubleList to compare, or null. </param>
        /// <returns>true if the value of <paramref name="a" /> is the same as the value of <paramref name="b" />; otherwise, false.</returns>
        public static bool operator ==(doubleListAccessor a, doubleListAccessor b)
        {
            if (ReferenceEquals(a, b))
              return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
              return false;
            // If both are same instance, return true.
            if (a.m_ptr == b.m_ptr) return true;
            // If length not equal, return false.
            if (a.length != b.length) return false;
            return Memory.Compare(a.m_ptr, b.m_ptr, a.length);
        }

        /// <summary>Determines whether two specified doubleList have different values.</summary>
        /// <returns>true if the value of <paramref name="a" /> is different from the value of <paramref name="b" />; otherwise, false.</returns>
        /// <param name="a">The first doubleList to compare, or null. </param>
        /// <param name="b">The second doubleList to compare, or null. </param>
        public static bool operator !=(doubleListAccessor a, doubleListAccessor b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Determines whether this instance and a specified object have the same value.
        /// </summary>
        /// <param name="obj">The doubleList to compare to this instance.</param>
        /// <returns>true if <paramref name="obj" /> is a doubleList and its value is the same as this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            doubleListAccessor b = obj as doubleListAccessor;
            if (b == null)
                return false;
            return (this == b);
        }

        /// <summary>
        /// Return the hash code for this doubleList.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return HashHelper.HashBytes(this.m_ptr, this.length);
        }
    }
}
