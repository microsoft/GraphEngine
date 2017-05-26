#include "common.h"
#include <string>
#include "SyntaxNode.h"

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        string* 
intListAccessor(
NTSL* node)
        {
            string* source = new string();
            
source->append(R"::(using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity.Core.Lib;
using Trinity.TSL;
using Trinity.TSL.Lib;
namespace )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(
{
    /// <summary>
    /// Represents a TSL double list corresponding to List{double}.
    /// </summary>
    
    public unsafe class intListAccessor : IEnumerable<int>
    {
        internal byte* CellPtr;
        internal long? CellID;
        internal ResizeFunctionDelegate ResizeFunction;
        internal intListAccessor(byte* _CellPtr, ResizeFunctionDelegate func)
        {
            CellPtr = _CellPtr;
            ResizeFunction = func;
            CellPtr += 4;
        }
        internal int length
        {
            get
            {
                return *(int*)(CellPtr - 4);
            }
        }
        /// <summary>
        /// Gets the number of elements actually contained in the List. 
        /// </summary>
        public unsafe int Count
        {
            get
            {
                return (length >> 2);
            }
        }
        /// <summary>
        /// Gets or sets the element at the specified index. 
        /// </summary>
        /// <param name="index">Give)::");
source->append(R"::(n index</param>
        /// <returns>Corresponding element at the specified index</returns>
        public unsafe int this[int index]
        {
            get
            {
                return *(int*)(CellPtr + (index << 2));
            }
            set
            {
                *(int*)(CellPtr + (index << 2)) = value;
            }
        }
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
        /// <summary>
        /// Performs the specified action on each elements
        /// </summary>
        /// <param name="action">A lambda expression which has one parameter indicates element in List</param>
        public unsafe vo)::");
source->append(R"::(id ForEach(Action<int> action)
        {
            byte* targetPtr = CellPtr;
            byte* endPtr = CellPtr + length;
            while (targetPtr < endPtr)
            {
                action(*(int*)targetPtr);
                targetPtr += 4;
            }
        }
        /// <summary>
        /// Performs the specified action on each elements
        /// </summary>
        /// <param name="action">A lambda expression which has two parameters. First indicates element in the List and second the index of this element.</param>
        public unsafe void ForEach(Action<int, int> action)
        {
            byte* targetPtr = CellPtr;
            byte* endPtr = CellPtr + length;
            for (int index = 0; targetPtr < endPtr; ++index)
            {
                action(*(int*)targetPtr, index);
                targetPtr += 4;
            }
        }
        internal unsafe struct _iterator
        {
            byte* targetPtr;
            byte* endPtr;
            internal _iterator(intListAcce)::");
source->append(R"::(ssor target)
            {
                targetPtr = target.CellPtr;
                endPtr    = target.CellPtr + target.length;
            }
            internal bool good()
            {
                return (targetPtr < endPtr);
            }
            internal int current()
            {
                return *(int*)targetPtr;
            }
            internal void move_next()
            {
                targetPtr += 4;
            }
        }
        /// <summary>
        /// Returns an enumerator that iterate through current list.
        /// </summary>
        /// <returns>
        /// An IEnumerator object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<int> GetEnumerator()
        {
            _iterator it = new _iterator(this);
            while (it.good())
            {
                yield return it.current();
                it.move_next();
            }
        }
        System.Collections.IEnumerator System.Collections.IEnu)::");
source->append(R"::(merable.GetEnumerator()
        {
            return GetEnumerator();
        }
        /// <summary>
        /// Adds an item to the end of the List
        /// </summary>
        /// <param name="element">The object to be added to the end of the List.</param>
        public unsafe void Add(int element)
        {
            int size = sizeof(int);
            this.CellPtr = this.ResizeFunction(this.CellPtr - sizeof(int), *(int*)(this.CellPtr - sizeof(int)) + sizeof(int), size);
            byte* targetPtr = this.CellPtr + (*(int*)this.CellPtr) + sizeof(int);
            *(int*)this.CellPtr += size;
            this.CellPtr += sizeof(int);
            *(int*)targetPtr = element;
        }
        /// <summary>
        /// Inserts an element into the List at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="element">The object to insert.</param>
        public unsafe void Insert(int index, int el)::");
source->append(R"::(ement)
        {
            if (index < 0 || index > Count) throw new IndexOutOfRangeException();
            int size = sizeof(int);
            byte* targetPtr = CellPtr + (index << 2);
            int offset = (int)(targetPtr - CellPtr);
            this.CellPtr = this.ResizeFunction(this.CellPtr - 4, offset + 4, size);
            *(int*)this.CellPtr += size;
            this.CellPtr += 4;
            targetPtr = this.CellPtr + offset;
            *(int*)targetPtr = element;
        }
        /// <summary>
        /// Inserts an element into the sorted List using the specified Comparison delegate.
        /// </summary>
        /// <param name="element">The element to insert.</param>
        /// <param name="comparison">The Comparison delegate.</param>
        public unsafe void Insert(int element, Comparison<int> comparison)
        {
            int size = sizeof(int);
            byte* targetPtr = CellPtr;
            byte* endPtr = CellPtr + length;
            while (targetPtr < endPtr)
         )::");
source->append(R"::(   {
                if (comparison(*(int*)targetPtr, element) <= 0)
                {
                    targetPtr += sizeof(int);
                }
                else
                {
                    break;
                }
            }
            int offset = (int)(targetPtr - CellPtr);
            this.CellPtr = this.ResizeFunction(this.CellPtr - 4, offset + 4, size);
            *(int*)this.CellPtr += size;
            this.CellPtr += 4;
            targetPtr = this.CellPtr + offset;
            *(int*)targetPtr = element;
        }
        /// <summary>
        /// Removes the element at the specified index of the List.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public unsafe void RemoveAt(int index)
        {
            if (index < 0 || index >= Count) throw new IndexOutOfRangeException();
            byte* targetPtr = CellPtr + (index << 2);
            int offset = (int)(targetPtr - CellPtr);
            int si)::");
source->append(R"::(ze = -sizeof(int);
            this.CellPtr = this.ResizeFunction(this.CellPtr - 4, offset + 4, size);
            *(int*)this.CellPtr += size;
            this.CellPtr += 4;
        }
        /// <summary>
        /// Adds the elements of the specified collection to the end of the List
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the end of the List. The collection itself cannot be null.</param>
        public unsafe void AddRange(List<int> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection is null.");
            intListAccessor tcollection = collection;
            int delta = tcollection.length;
            CellPtr = ResizeFunction(CellPtr - 4, *(int*)(CellPtr - 4) + 4, delta);
            Memory.Copy(tcollection.CellPtr, CellPtr + *(int*)CellPtr + 4, delta);
            *(int*)CellPtr += delta;
            this.CellPtr += 4;
        }
        /// <summary>
        /// Adds the elements of t)::");
source->append(R"::(he specified collection to the end of the List
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the end of the List. The collection itself cannot be null.</param>
        public unsafe void AddRange(intListAccessor collection)
        {
            if (collection == null) throw new ArgumentNullException("collection is null.");
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
                    CellPtr = ResizeFunction(CellPtr - 4, *(int*)(CellPtr - 4) + 4)::");
source->append(R"::(, delta);
                    Memory.Copy(tmpcellptr, CellPtr + *(int*)CellPtr + 4, delta);
                    *(int*)CellPtr += delta;
                }
            }
            this.CellPtr += 4;
        }
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
        /// <summary>
        /// Determines whether an element is in the List
        /// </summary>
        /// <param name="item">The object to locate in the List.The value can be null for non-atom types</param>
        /// <returns>true if item is found in the List; otherwise, false.</returns>
        public unsafe bool Contains(int item)
        {
            bool ret = false;
            ForEach(x =>
            {
                if (item == x) )::");
source->append(R"::(ret = true;
            });
            return ret;
        }
        /// <summary>
        /// Determines whether the List contains elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The Predicate delegate that defines the conditions of the elements to search for.</param>
        /// <returns>true if the List contains one or more elements that match the conditions defined by the specified predicate; otherwise, false.</returns>
        public unsafe bool Exists(Predicate<int> match)
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
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from List. The Arra)::");
source->append(R"::(y must have zero-based indexing.</param>
        public unsafe void CopyTo(int[] array)
        {
            if (array == null) throw new ArgumentNullException("array is null.");
            if (array.Length < Count) throw new ArgumentException("The number of elements in the source List is greater than the number of elements that the destination array can contain.");
            fixed (int* ip = array)
            {
                Memory.Copy(CellPtr, 0, ip, 0, length);
            }
        }
        /// <summary>
        /// Copies the entire List to a compatible one-dimensional array, starting at the specified index of the ptr1 array.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from List. The Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public unsafe void CopyTo(int[] array, int arrayIndex)
        {
            i)::");
source->append(R"::(f (array == null) throw new ArgumentNullException("array is null.");
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException("arrayIndex is less than 0.");
            if (array.Length - arrayIndex < Count) throw new ArgumentException("The number of elements in the source List is greater than the available space from arrayIndex to the end of the destination array.");
            fixed (int* ip = array)
            {
                Memory.Copy(CellPtr, 0, ip, arrayIndex, length);
            }
        }
        /// <summary>
        /// Copies a range of elements from the List to a compatible one-dimensional array, starting at the specified index of the ptr1 array.
        /// </summary>
        /// <param name="index">The zero-based index in the source List at which copying begins.</param>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from List. The Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The )::");
source->append(R"::(zero-based index in array at which copying begins.</param>;
        /// <param name="count">The number of elements to copy.</param>
        public unsafe void CopyTo(int index, int[] array, int arrayIndex, int count)
        {
            if (array == null) throw new ArgumentNullException("array is null.");
            if (arrayIndex < 0 || index < 0 || count < 0) throw new ArgumentOutOfRangeException("arrayIndex is less than 0 or index is less than 0 or count is less than 0.");
            if (array.Length - arrayIndex < count) throw new ArgumentException("The number of elements from index to the end of the source List is greater than the available space from arrayIndex to the end of the destination array. ");
            if (index + count > Count) throw new ArgumentException("Source list does not have enough elements to copy.");
            fixed (int* ip = array)
            {
                Memory.Copy(CellPtr, index, ip, arrayIndex, count);
            }
        }
        /// <summary>
        /// Inse)::");
source->append(R"::(rts the elements of a collection into the List at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="collection">The collection whose elements should be inserted into the List. The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
        public unsafe void InsertRange(int index, List<int> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection is null.");
            if (index < 0) throw new ArgumentOutOfRangeException("index is less than 0.");
            if (index > Count) throw new ArgumentOutOfRangeException("index is greater than Count.");
            intListAccessor tmpAccessor = collection;
            int offset = (index << 2);
            CellPtr = ResizeFunction(CellPtr - 4, offset + 4, tmpAccessor.length);
            Memory.Copy(tmpAccessor.CellPtr, CellPtr + offs)::");
source->append(R"::(et + 4, tmpAccessor.length);
            *(int*)CellPtr += tmpAccessor.length;
            this.CellPtr += 4;
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
            int offset = (index << 2);
            int size = -(count << 2);
            CellPtr = ResizeFunction(CellPtr - 4, offset + 4, size);
            *(int*)CellPtr += size;
            this.CellPtr += 4;
      )::");
source->append(R"::(  }
        /// <summary>
        /// Implicitly converts an intList instance to a List{int} instance.
        /// </summary>
        /// <param name="accessor">The intList instance.</param>
        /// <returns>A List{int} instance.</returns>
        public unsafe static implicit operator List<int>(intListAccessor accessor)
        {
            if ((object)accessor == null) return null;
            List<int> list = new List<int>();
            accessor.ForEach(element => list.Add(element));
            return list;
        }
        /// <summary>
        /// Implicitly converts a List{int} instance to an intList instance.
        /// </summary>
        /// <param name="value">The List{int} instance.</param>
        /// <returns>An intList instance.</returns>
        public unsafe static implicit operator intListAccessor(List<int> value)
        {
            byte* targetPtr = null;
            if (value != null)
            {
                targetPtr += value.Count * 4 + sizeof(int);
            }
     )::");
source->append(R"::(       else
            {
                targetPtr += sizeof(int);
            }
            byte* tmpcellptr = BufferAllocator.AllocBuffer((int)targetPtr);
            targetPtr = tmpcellptr;
            if (value != null)
            {
                *(int*)targetPtr = value.Count * 4;
                targetPtr += sizeof(int);
                for (int iterator_0 = 0; iterator_0 < value.Count; ++iterator_0)
                {
                    *(int*)targetPtr = value[iterator_0];
                    targetPtr += 4;
                }
            }
            else
            {
                *(int*)targetPtr = 0;
                targetPtr += sizeof(int);
            }
            intListAccessor ret = new intListAccessor(tmpcellptr, null);
            ret.CellID = null;
            return ret;
        }
        /// <summary>
        /// Determines whether two specified intList have the same value.
        /// </summary>
        /// <param name="a">The first intList to compare, or null. </param>
    )::");
source->append(R"::(    /// <param name="b">The second intList to compare, or null. </param>
        /// <returns>true if the value of <paramref name="a" /> is the same as the value of <paramref name="b" />; otherwise, false.</returns>
        public static bool operator ==(intListAccessor a, intListAccessor b)
        {
            if (ReferenceEquals(a, b))
              return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
              return false;
            if (a.CellPtr == b.CellPtr) return true;
            if (a.length != b.length) return false;
            return Memory.Compare(a.CellPtr, b.CellPtr, a.length);
        }
        /// <summary>Determines whether two specified intList have different values.</summary>
        /// <returns>true if the value of <paramref name="a" /> is different from the value of <paramref name="b" />; otherwise, false.</returns>
        /// <param name="a">The first intList to compare, or null. </param>
        /// <param name="b">The second intList to compa)::");
source->append(R"::(re, or null. </param>
        public static bool operator !=(intListAccessor a, intListAccessor b)
        {
            return !(a == b);
        }
        /// <summary>
        /// Determines whether this instance and a specified object have the same value.
        /// </summary>
        /// <param name="obj">The intList to compare to this instance.</param>
        /// <returns>true if <paramref name="obj" /> is a doubleList and its value is the same as this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            intListAccessor b = obj as intListAccessor;
            if (b == null)
                return false;
            return (this == b);
        }
        /// <summary>
        /// Return the hash code for this intList.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return HashHelper.HashBytes(this.CellPtr, this.length);
        }
    }
}
)::");

            return source;
        }
    }
}
