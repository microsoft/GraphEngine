#include "common.h"
#include <string>
#include "SyntaxNode.h"

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        string* 
byteListAccessor(
NTSL* node)
        {
            string* source = new string();
            
source->append(R"::(using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity.Core.Lib;
using Trinity.Storage;
using Trinity.TSL;
using Trinity.TSL.Lib;
namespace )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(
{
    /// <summary>
    /// Represents a Trinity TSL type corresponding to List{byte}.
    /// </summary>
    public unsafe class byteListAccessor : IAccessor, IEnumerable<byte>
    {
        internal byte* m_ptr;
        internal long CellId;
        internal byteListAccessor(byte* _CellPtr, ResizeFunctionDelegate func)
        {
            m_ptr = _CellPtr;
            ResizeFunction = func;
            m_ptr += 4;
        }
        internal int length
        {
            get
            {
                return *(int*)(m_ptr - 4);
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
        )::");
source->append(R"::(        return ret;
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
        /// Gets the number of elements actually contained in the List. 
        /// </summary>
        public unsafe int Count
        {
            get
            {
                return *(int*)(m_ptr - 4);
            }
        }
        /// <summary>
        /// Gets or s)::");
source->append(R"::(ets the element at the specified index. 
        /// </summary>
        /// <param name="index">Given index</param>
        /// <returns>Corresponding element at the specified index</returns>
        public unsafe byte this[int index]
        {
            get
            {
                return *(m_ptr + index);
            }
            set
            {
                *(m_ptr + index) = value;
            }
        }
        /// <summary>
        /// Performs the specified action on each elements
        /// </summary>
        /// <param name="action">A lambda expression which has one parameter indicates element in List</param>
        public unsafe void ForEach(Action<byte> action)
        {
            byte* targetPtr = m_ptr;
            byte* endPtr = m_ptr + length;
            while (targetPtr < endPtr)
            {
                action(*targetPtr);
                targetPtr++;
            }
        }
        /// <summary>
        /// Performs the specified action o)::");
source->append(R"::(n each elements
        /// </summary>
        /// <param name="action">A lambda expression which has two parameters. First indicates element in the List and second the index of this element.</param>
        public unsafe void ForEach(Action<byte, int> action)
        {
            byte* targetPtr = m_ptr;
            byte* endPtr = m_ptr + length;
            for (int index = 0; targetPtr < endPtr; ++index)
            {
                action(*targetPtr, index);
                targetPtr++;
            }
        }
        internal unsafe struct _iterator
        {
            byte* targetPtr;
            byte* endPtr;
            internal _iterator(byteListAccessor target)
            {
                targetPtr = target.m_ptr;
                endPtr    = target.m_ptr + target.length;
            }
            internal bool good()
            {
                return (targetPtr < endPtr);
            }
            internal byte current()
            {
                return *(byt)::");
source->append(R"::(e*)targetPtr;
            }
            internal void move_next()
            {
                targetPtr += 1;
            }
        }
        /// <summary>
        /// Returns an enumerator that iterate through current list.
        /// </summary>
        /// <returns>
        /// An IEnumerator object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<byte> GetEnumerator()
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
        public unsafe void Add(byte ele)::");
source->append(R"::(ment)
        {
            byte* targetPtr = null;
            targetPtr++;
            int size = (int)targetPtr;
            this.m_ptr = this.ResizeFunction(this.m_ptr - sizeof(int), *(int*)(this.m_ptr - sizeof(int)) + sizeof(int), size);
            targetPtr = this.m_ptr + (*(int*)this.m_ptr) + sizeof(int);
            *(int*)this.m_ptr += size;
            this.m_ptr += sizeof(int);
            *targetPtr = element;
        }
        /// <summary>
        /// Inserts an element into the List at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="element">The object to insert.</param>
        public unsafe void Insert(int index, byte element)
        {
            if (index < 0 || index > Count) throw new IndexOutOfRangeException();
            int size = sizeof(byte);
            byte* targetPtr = m_ptr;
            targetPtr += index;
            int offset = (int)(targ)::");
source->append(R"::(etPtr - m_ptr);
            this.m_ptr = this.ResizeFunction(this.m_ptr - 4, offset + 4, size);
            *(int*)this.m_ptr += size;
            this.m_ptr += 4;
            targetPtr = this.m_ptr + offset;
            *targetPtr = element;
        }
        /// <summary>
        /// Inserts an element into the sorted List using the specified Comparison delegate.
        /// </summary>
        /// <param name="element">The element to insert.</param>
        /// <param name="comparison">The Comparison delegate.</param>
        public unsafe void Insert(byte element, Comparison<byte> comparison)
        {
            int size = sizeof(byte);
            byte* targetPtr = m_ptr;
            byte* endPtr = m_ptr + length;
            while (targetPtr < endPtr)
            {
                if (comparison(*(byte*)targetPtr, element) <= 0)
                {
                    targetPtr++;
                }
                else
                {
                    break;
               )::");
source->append(R"::( }
            }
            int offset = (int)(targetPtr - m_ptr);
            this.m_ptr = this.ResizeFunction(this.m_ptr - 4, offset + 4, size);
            *(int*)this.m_ptr += size;
            this.m_ptr += 4;
            targetPtr = this.m_ptr + offset;
            *(byte*)targetPtr = element;
        }
        /// <summary>
        /// Removes the element at the specified index of the List.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public unsafe void RemoveAt(int index)
        {
            if (index < 0 || index >= Count) throw new IndexOutOfRangeException();
            byte* targetPtr = m_ptr;
            targetPtr += index;
            int offset = (int)(targetPtr - m_ptr);
            byte* oldtargetPtr = targetPtr;
            targetPtr++;
            int size = (int)(oldtargetPtr - targetPtr);
            this.m_ptr = this.ResizeFunction(this.m_ptr - 4, offset + 4, size);
            *(int*)this.)::");
source->append(R"::(m_ptr += size;
            this.m_ptr += 4;
        }
        /// <summary>
        /// Adds the elements of the specified collection to the end of the List
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the end of the List. The collection itself cannot be null.</param>
        public unsafe void AddRange(List<byte> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection is null.");
            byteListAccessor tcollection = collection;
            int delta = tcollection.length;
            m_ptr = ResizeFunction(m_ptr - 4, *(int*)(m_ptr - 4) + 4, delta);
            Memory.Copy(tcollection.m_ptr, m_ptr + *(int*)m_ptr + 4, delta);
            *(int*)m_ptr += delta;
            this.m_ptr += 4;
        }
        /// <summary>
        /// Adds the elements of the specified collection to the end of the List
        /// </summary>
        /// <param name="collection">The collection whos)::");
source->append(R"::(e elements should be added to the end of the List. The collection itself cannot be null.</param>
        public unsafe void AddRange(byteListAccessor collection)
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
                    *(int*)m_ptr += delt)::");
source->append(R"::(a;
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
        public unsafe bool Contains(byte item)
        {
            bool ret = false;
            ForEach(x =>
            {
                if (item == x) ret = true;
            });
            return ret;
        }
        /// <summary>
        /// Determines whether t)::");
source->append(R"::(he List contains elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The Predicate delegate that defines the conditions of the elements to search for.</param>
        /// <returns>true if the List contains one or more elements that match the conditions defined by the specified predicate; otherwise, false.</returns>
        public unsafe bool Exists(Predicate<byte> match)
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
        public unsafe void CopyTo(byte[] array)
        {)::");
source->append(R"::(
            if (array == null) throw new ArgumentNullException("array is null.");
            if (array.Length < Count) throw new ArgumentException("The number of elements in the source List is greater than the number of elements that the destination array can contain.");
            Memory.Copy(m_ptr, 0, array, 0, length);
        }
        /// <summary>
        /// Copies the entire List to a compatible one-dimensional array, starting at the specified index of the ptr1 array.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from List. The Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public unsafe void CopyTo(byte[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException("array is null.");
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException("arrayIndex is)::");
source->append(R"::( less than 0.");
            if (array.Length - arrayIndex < Count) throw new ArgumentException("The number of elements in the source List is greater than the available space from arrayIndex to the end of the destination array.");
            Memory.Copy(m_ptr, 0, array, arrayIndex, length);
        }
        /// <summary>
        /// Copies a range of elements from the List to a compatible one-dimensional array, starting at the specified index of the ptr1 array.
        /// </summary>
        /// <param name="index">The zero-based index in the source List at which copying begins.</param>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from List. The Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>;
        /// <param name="count">The number of elements to copy.</param>
        public unsafe void CopyTo(int index, byte[] array, int arrayIndex, int )::");
source->append(R"::(count)
        {
            if (array == null) throw new ArgumentNullException("array is null.");
            if (arrayIndex < 0 || index < 0 || count < 0) throw new ArgumentOutOfRangeException("arrayIndex is less than 0 or index is less than 0 or count is less than 0.");
            if (array.Length - arrayIndex < count) throw new ArgumentException("The number of elements from index to the end of the source List is greater than the available space from arrayIndex to the end of the destination array. ");
            if (index + count > Count) throw new ArgumentException("Source list does not have enough elements to copy.");
            Memory.Copy(m_ptr, index, array, arrayIndex, count);
        }
        /// <summary>
        /// Inserts the elements of a collection into the List at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="collection">The collection whose element)::");
source->append(R"::(s should be inserted into the List. The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
        public unsafe void InsertRange(int index, List<byte> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection is null.");
            if (index < 0) throw new ArgumentOutOfRangeException("index is less than 0.");
            if (index > Count) throw new ArgumentOutOfRangeException("index is greater than Count.");
            byteListAccessor tmpAccessor = collection;
            byte* targetPtr = m_ptr;
            targetPtr += index;
            int offset = (int)(targetPtr - m_ptr);
            m_ptr = ResizeFunction(m_ptr - 4, offset + 4, tmpAccessor.length);
            Memory.Copy(tmpAccessor.m_ptr, m_ptr + offset + 4, tmpAccessor.length);
            *(int*)m_ptr += tmpAccessor.length;
            this.m_ptr += 4;
        }
        /// <summary>
        /// Removes a range of elemen)::");
source->append(R"::(ts from the List.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        public unsafe void RemoveRange(int index, int count)
        {
            if (index < 0) throw new ArgumentOutOfRangeException("index is less than 0.");
            if (index > Count) throw new ArgumentOutOfRangeException("index is greater than Count.");
            if (index + count > Count) throw new ArgumentException("index and count do not denote a valid range of elements in the List.");
            byte* targetPtr = m_ptr;
            targetPtr += index;
            int offset = (int)(targetPtr - m_ptr);
            byte* oldtargetPtr = targetPtr;
            targetPtr += count;
            int size = (int)(oldtargetPtr - targetPtr);
            m_ptr = ResizeFunction(m_ptr - 4, offset + 4, size);
            *(int*)m_ptr += size;
            this.m_ptr += 4;
 )::");
source->append(R"::(       }
        /// <summary>
        /// Implicitly casts a byteList to List{byte}.
        /// </summary>
        /// <param name="accessor">A byteList instance.</param>
        /// <returns>A List{byte} instance.</returns>
        public unsafe static implicit operator List<byte>(byteListAccessor accessor)
        {
            if ((object)accessor == null) return null;
            List<byte> list = new List<byte>();
            accessor.ForEach(element => list.Add(element));
            return list;
        }
        /// <summary>
        /// Implicitly casts a List{byte} instance to byteList.
        /// </summary>
        /// <param name="value">The List{byte} instance.</param>
        /// <returns></returns>
        public unsafe static implicit operator byteListAccessor(List<byte> value)
        {
            byte* targetPtr = null;
            if (value != null)
            {
                targetPtr += value.Count * 1 + sizeof(int);
            }
            else
        )::");
source->append(R"::(    {
                targetPtr += sizeof(int);
            }
            byte* tmpcellptr = BufferAllocator.AllocBuffer((int)targetPtr);
            targetPtr = tmpcellptr;
            if (value != null)
            {
                *(int*)targetPtr = value.Count * 1;
                targetPtr += sizeof(int);
                for (int iterator_0 = 0; iterator_0 < value.Count; ++iterator_0)
                {
                    *(byte*)targetPtr = value[iterator_0];
                    targetPtr++;
                }
            }
            else
            {
                *(int*)targetPtr = 0;
                targetPtr += sizeof(int);
            }
            byteListAccessor ret = new byteListAccessor(tmpcellptr, null);
            return ret;
        }
        /// <summary>
        /// Implicitly convert a byte array to a byteList instance.
        /// </summary>
        /// <param name="value">The array of bytes.</param>
        /// <returns>A byteList instance.</returns>
 )::");
source->append(R"::(       public unsafe static implicit operator byteListAccessor(byte[] value)
        {
            byte* targetPtr = null;
            if (value != null)
            {
                targetPtr += value.Length + sizeof(int);
            }
            else
            {
                targetPtr += sizeof(int);
            }
            byte* tmpcellptr = BufferAllocator.AllocBuffer((int)targetPtr);
            targetPtr = tmpcellptr;
            if (value != null)
            {
                *(int*)targetPtr = value.Length;
                targetPtr += sizeof(int);
                for (int iterator_0 = 0; iterator_0 < value.Length; ++iterator_0)
                {
                    *targetPtr = value[iterator_0];
                    targetPtr++;
                }
            }
            else
            {
                *(int*)targetPtr = 0;
                targetPtr += sizeof(int);
            }
            byteListAccessor ret = new byteListAccessor(tmpcellptr, null);
    )::");
source->append(R"::(        return ret;
        }
        /// <summary>
        /// Determines whether two specified byteList have the same value.
        /// </summary>
        /// <param name="a">The first byteList to compare, or null. </param>
        /// <param name="b">The second byteList to compare, or null. </param>
        /// <returns>true if the value of <paramref name="a" /> is the same as the value of <paramref name="b" />; otherwise, false.</returns>
        public static bool operator ==(byteListAccessor a, byteListAccessor b)
        {
            if (ReferenceEquals(a, b))
              return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
              return false;
            if (a.m_ptr == b.m_ptr) return true;
            if (a.length != b.length) return false;
            return Memory.Compare(a.m_ptr, b.m_ptr, a.length);
        }
        /// <summary>Determines whether two specified byteList have different values.</summary>
        /// <returns>true if the v)::");
source->append(R"::(alue of <paramref name="a" /> is different from the value of <paramref name="b" />; otherwise, false.</returns>
        /// <param name="a">The first byteList to compare, or null. </param>
        /// <param name="b">The second byteList to compare, or null. </param>
        public static bool operator !=(byteListAccessor a, byteListAccessor b)
        {
            return !(a == b);
        }
        /// <summary>
        /// Determines whether this instance and a specified object have the same value.
        /// </summary>
        /// <param name="obj">The byteList to compare to this instance.</param>
        /// <returns>true if obj is a byteList and its value is the same as this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            byteListAccessor b = obj as byteListAccessor;
            if (b == null)
                return false;
            return (this == b);
        }
        /// <summary>
        /// Returns the hash code for )::");
source->append(R"::(this byteList.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return HashHelper.HashBytes(this.m_ptr, this.length);
        }
    }
}
)::");

            return source;
        }
    }
}
