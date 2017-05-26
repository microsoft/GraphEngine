#include "common.h"
#include <string>
#include "SyntaxNode.h"

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        string* 
longListAccessor(
NTSL* node)
        {
            string* source = new string();
            
source->append(R"::(using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity;
using Trinity.Core.Lib;
using Trinity.Storage;
using Trinity.TSL;
using Trinity.TSL.Lib;
namespace )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(
{
    internal class CellIdCache
    {
        internal List<List<long>> LongList = new List<List<long>>();
        internal List<long> GetLongList(int index)
        {
            lock (LongList)
            {
                if (index < LongList.Count)
                {
                    return LongList[index];
                }
            }
            return null;
        }
        internal int AddLongList(out List<long> list)
        {
            lock (LongList)
            {
                list = new List<long>();
                LongList.Add(list);
                return LongList.Count - 1;
            }
        }
    }
    /// <summary>
    /// Represents a TSL long list corresponding List{long}.
    /// </summary>
    
    public unsafe class longListAccessor : IEnumerable<long>
    {
        internal byte* CellPtr;
        internal long? CellID;
        internal ResizeFunctionDelegate ResizeFunction;
        internal const int               c_idcache_count = 256;
        internal static Cel)::");
source->append(R"::(lIdCache[]    s_IdCache       = new CellIdCache[c_idcache_count];
        internal longListAccessor(byte* _CellPtr, ResizeFunctionDelegate func)
        {
            ResizeFunction = func;
            CellPtr = _CellPtr + sizeof(int);
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
                return length >> 3;
            }
        }
        /// <summary>
        /// Gets or sets the element at the specified index. 
        /// </summary>
        /// <param name="index">Given index</param>
        /// <returns>Corresponding element at the specified index</returns>
        public unsafe long this[int index]
        {
            get
            {
                return *(long*)(CellPtr + (index << 3));
            )::");
source->append(R"::(}
            set
            {
                *(long*)(CellPtr + (index << 3)) = value;
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
        public unsafe void ForEach(Action<long> action)
        {
            byte* targetPtr = CellPtr;
            byte* endPtr = CellPtr + length;
            while (targetPtr < endPtr)
            {
                action(*(long*)targetPtr);
                tar)::");
source->append(R"::(getPtr += 8;
            }
        }
        /// <summary>
        /// Performs the specified action on each elements
        /// </summary>
        /// <param name="action">A lambda expression which has two parameters. First indicates element in the List and second the index of this element.</param>
        public unsafe void ForEach(Action<long, int> action)
        {
            byte* targetPtr = CellPtr;
            byte* endPtr = CellPtr + length;
            for (int index = 0; targetPtr < endPtr; ++index)
            {
                action(*(long*)targetPtr, index);
                targetPtr += 8;
            }
        }
        internal unsafe struct _iterator
        {
            byte* targetPtr;
            byte* endPtr;
            internal _iterator(longListAccessor target)
            {
                targetPtr = target.CellPtr;
                endPtr = target.CellPtr + target.length;
            }
            internal bool good()
            {
                return (targetPtr < endPtr);
)::");
source->append(R"::(            }
            internal long current()
            {
                return *(long*)targetPtr;
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
        public IEnumerator<long> GetEnumerator()
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
        /// <param name="element">The object to be added to t)::");
source->append(R"::(he end of the List.</param>
        public unsafe void Add(long element)
        {
            int size = sizeof(long);
            this.CellPtr = this.ResizeFunction(this.CellPtr - sizeof(int), *(int*)(this.CellPtr - sizeof(int)) + sizeof(int), size);
            byte* targetPtr = this.CellPtr + (*(int*)this.CellPtr) + sizeof(int);
            *(int*)this.CellPtr += size;
            this.CellPtr += sizeof(int);
            *(long*)targetPtr = element;
        }
        internal unsafe void AddCellID(long cellId, long element)
        {
            var IdCache = s_IdCache[LocalMemoryStorage.GetTrunkId(cellId)];
            if (*(int*)(CellPtr - sizeof(int)) != 0)
            {
                IdCache.GetLongList(*(int*)CellPtr).Add(element);
            }
            else
            {
                CellPtr = this.ResizeFunction(this.CellPtr - sizeof(int), sizeof(int), sizeof(long));
                *(int*)CellPtr = sizeof(long);
                this.CellPtr += sizeof(int);
                List<long> lis)::");
source->append(R"::(t;
                *(int*)CellPtr = IdCache.AddLongList(out list);
                *(int*)(CellPtr + sizeof(int)) = -1;
                list.Add(element);
            }
        }
        internal unsafe void Stretch(long cellId)
        {
            if (*(int*)(CellPtr - sizeof(int)) != 0)
            {
                List<long> list = s_IdCache[LocalMemoryStorage.GetTrunkId(cellId)].GetLongList(*(int*)CellPtr);
                int size = (list.Count - 1) << 3;
                CellPtr = this.ResizeFunction(this.CellPtr - sizeof(int), sizeof(int) + sizeof(long), size);
                *(int*)CellPtr = list.Count << 3;
                long* lp = (long*)(CellPtr + sizeof(int));
                for (int i = 0; i < list.Count; i++)
                {
                    *lp++ = list[i];
                }
            }
        }
        /// <summary>
        /// Inserts an element into the List at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item shoul)::");
source->append(R"::(d be inserted.</param>
        /// <param name="element">The object to insert.</param>
        public unsafe void Insert(int index, long element)
        {
            if (index < 0 || index > Count) throw new IndexOutOfRangeException();
            int size = sizeof(long);
            byte* targetPtr = CellPtr + (index << 3);
            int offset = (int)(targetPtr - CellPtr);
            this.CellPtr = this.ResizeFunction(this.CellPtr - 4, offset + 4, size);
            *(int*)this.CellPtr += size;
            this.CellPtr += 4;
            targetPtr = this.CellPtr + offset;
            *(long*)targetPtr = element;
        }
        /// <summary>
        /// Inserts an element into the sorted List using the specified Comparison delegate.
        /// </summary>
        /// <param name="element">The element to insert.</param>
        /// <param name="comparison">The Comparison delegate.</param>
        public unsafe void Insert(long element, Comparison<long> comparison)
        {
            int size = size)::");
source->append(R"::(of(long);
            byte* targetPtr = CellPtr;
            byte* endPtr = CellPtr + length;
            while (targetPtr < endPtr)
            {
                if (comparison(*(long*)targetPtr, element) <= 0)
                {
                    targetPtr += sizeof(long);
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
            *(long*)targetPtr = element;
        }
        /// <summary>
        /// Removes the element at the specified index of the List.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public unsafe void RemoveAt(int index)
        {
            if (index < 0 || index >= Count) throw new IndexOutO)::");
source->append(R"::(fRangeException();
            byte* targetPtr = CellPtr + (index << 3);
            int offset = (int)(targetPtr - CellPtr);
            int size = -sizeof(long);
            this.CellPtr = this.ResizeFunction(this.CellPtr - 4, offset + 4, size);
            *(int*)this.CellPtr += size;
            this.CellPtr += 4;
        }
        /// <summary>
        /// Adds the elements of the specified collection to the end of the List
        /// </summary>
        /// <param name="list">The collection whose elements should be added to the end of the List. The collection itself cannot be null.</param>
        public unsafe void AddRange(List<long> list)
        {
            if (list == null) throw new ArgumentNullException("collection is null.");
            int delta = list.Count << 3;
            CellPtr = ResizeFunction(CellPtr - 4, *(int*)(CellPtr - 4) + 4, delta);
            long* cp = (long*)(CellPtr + *(int*)CellPtr + 4);
            for (int i = 0; i < list.Count; i++)
            {
                *cp )::");
source->append(R"::(= list[i];
                cp++;
            }
            *(int*)CellPtr += delta;
            this.CellPtr += 4;
        }
        /// <summary>
        /// Adds the elements of the specified collection to the end of the List
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the end of the List. The collection itself cannot be null.</param>
        public unsafe void AddRange(longListAccessor collection)
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
                fixed (byte* tmpcellptr =)::");
source->append(R"::( tmpcell)
                {
                    Memory.Copy(collection.CellPtr, tmpcellptr, delta);
                    CellPtr = ResizeFunction(CellPtr - 4, *(int*)(CellPtr - 4) + 4, delta);
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
        /// <returns>true if item is found in the List; ot)::");
source->append(R"::(herwise, false.</returns>
        public unsafe bool Contains(long item)
        {
            byte* targetPtr = CellPtr;
            byte* endPtr = CellPtr + length;
            while (targetPtr < endPtr)
            {
                if ((*(long*)targetPtr) == item)
                    return true;
                targetPtr += 8;
            }
            return false;
        }
        /// <summary>
        /// Determines whether the List contains elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The Predicate delegate that defines the conditions of the elements to search for.</param>
        /// <returns>true if the List contains one or more elements that match the conditions defined by the specified predicate; otherwise, false.</returns>
        public unsafe bool Exists(Predicate<long> match)
        {
            bool ret = false;
            ForEach(x =>
            {
                if (match(x)) ret = true;
            });
)::");
source->append(R"::(            return ret;
        }
        /// <summary>
        /// Copies the entire List to a compatible one-dimensional array, starting at the beginning of the ptr1 array.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from List. The Array must have zero-based indexing.</param>
        public unsafe void CopyTo(long[] array)
        {
            if (array == null) throw new ArgumentNullException("array is null.");
            if (array.Length < Count) throw new ArgumentException("The number of elements in the source List is greater than the number of elements that the destination array can contain.");
            fixed (long* lp = array)
            {
                Memory.Copy(CellPtr, 0, lp, 0, length);
            }
        }
        /// <summary>
        /// Copies the entire List to a compatible one-dimensional array, starting at the specified index of the ptr1 array.
        /// </summary>
        /// <param name="a)::");
source->append(R"::(rray">The one-dimensional Array that is the destination of the elements copied from List. The Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public unsafe void CopyTo(long[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException("array is null.");
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException("arrayIndex is less than 0.");
            if (array.Length - arrayIndex < Count) throw new ArgumentException("The number of elements in the source List is greater than the available space from arrayIndex to the end of the destination array.");
            fixed (long* lp = array)
            {
                Memory.Copy(CellPtr, 0, lp, arrayIndex, length);
            }
        }
        /// <summary>
        /// Copies a range of elements from the List to a compatible one-dimensional array, starting at the specified index of the ptr1 array.
       )::");
source->append(R"::( /// </summary>
        /// <param name="index">The zero-based index in the source List at which copying begins.</param>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from List. The Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>;
        /// <param name="count">The number of elements to copy.</param>
        public unsafe void CopyTo(int index, long[] array, int arrayIndex, int count)
        {
            if (array == null) throw new ArgumentNullException("array is null.");
            if (arrayIndex < 0 || index < 0 || count < 0) throw new ArgumentOutOfRangeException("arrayIndex is less than 0 or index is less than 0 or count is less than 0.");
            if (array.Length - arrayIndex < count) throw new ArgumentException("The number of elements from index to the end of the source List is greater than the available space from arrayIndex to the end of th)::");
source->append(R"::(e destination array. ");
            if (index + count > Count) throw new ArgumentException("Source list does not have enough elements to copy.");
            fixed (long* lp = array)
            {
                Memory.Copy(CellPtr, index, lp, arrayIndex, count);
            }
        }
        /// <summary>
        /// Inserts the elements of a collection into the List at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="collection">The collection whose elements should be inserted into the List. The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
        public unsafe void InsertRange(int index, List<long> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection is null.");
            if (index < 0) throw new ArgumentOutOfRangeException("index is less than 0.");
      )::");
source->append(R"::(      if (index > Count) throw new ArgumentOutOfRangeException("index is greater than Count.");
            longListAccessor tmpAccessor = collection;
            byte* targetPtr = CellPtr + (index << 3);
            int offset = (int)(targetPtr - CellPtr);
            CellPtr = ResizeFunction(CellPtr - 4, offset + 4, tmpAccessor.length);
            Memory.Copy(tmpAccessor.CellPtr, CellPtr + offset + 4, tmpAccessor.length);
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
            if (index > Count) throw new ArgumentOutOfRangeE)::");
source->append(R"::(xception("index is greater than Count.");
            if (index + count > Count) throw new ArgumentException("index and count do not denote a valid range of elements in the List.");
            byte* targetPtr = CellPtr + (index << 3);
            int offset = (int)(targetPtr - CellPtr);
            int size = -(count << 3);
            CellPtr = ResizeFunction(CellPtr - 4, offset + 4, size);
            *(int*)CellPtr += size;
            this.CellPtr += 4;
        }
        /// <summary>
        /// Implicitly converts a longList instance to a List{long} instance.
        /// </summary>
        /// <param name="accessor">The longList instance.</param>
        /// <returns>A List{long} instance.</returns>
        public unsafe static implicit operator List<long>(longListAccessor accessor)
        {
            if ((object)accessor == null) return null;
            List<long> list = new List<long>();
            accessor.ForEach(element => list.Add(element));
            return list;
        }
        /// )::");
source->append(R"::(<summary>
        /// Implicitly converts a List{long} instance to a longList instance.
        /// </summary>
        /// <param name="value">The List{long} instance.</param>
        /// <returns>A longList instance.</returns>
        public unsafe static implicit operator longListAccessor(List<long> value)
        {
            byte* targetPtr = null;
            if (value != null)
            {
                targetPtr += (value.Count << 3) + sizeof(int);
            }
            else
            {
                targetPtr += sizeof(int);
            }
            byte* tmpcellptr = BufferAllocator.AllocBuffer((int)targetPtr);
            targetPtr = tmpcellptr;
            if (value != null)
            {
                *(int*)targetPtr = (value.Count << 3);
                targetPtr += sizeof(int);
                for (int iterator_0 = 0; iterator_0 < value.Count; ++iterator_0)
                {
                    *(long*)targetPtr = value[iterator_0];
                    targetPtr += 8;
        )::");
source->append(R"::(        }
            }
            else
            {
                *(int*)targetPtr = 0;
                targetPtr += sizeof(int);
            }
            longListAccessor ret = new longListAccessor(tmpcellptr, null);
            ret.CellID = null;
            return ret;
        }
        /// <summary>
        /// Determines whether two specified longList have the same value.
        /// </summary>
        /// <param name="a">The first longList to compare, or null. </param>
        /// <param name="b">The second longList to compare, or null. </param>
        /// <returns>true if the value of <paramref name="a" /> is the same as the value of <paramref name="b" />; otherwise, false.</returns>
        public static bool operator ==(longListAccessor a, longListAccessor b)
        {
            if (ReferenceEquals(a, b))
              return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
              return false;
            if (a.CellPtr == b.CellPtr) return true;
       )::");
source->append(R"::(     if (a.length != b.length) return false;
            return Memory.Compare(a.CellPtr, b.CellPtr, a.length);
        }
        /// <summary>Determines whether two specified longList have different values.</summary>
        /// <returns>true if the value of <paramref name="a" /> is different from the value of <paramref name="b" />; otherwise, false.</returns>
        /// <param name="a">The first longList to compare, or null. </param>
        /// <param name="b">The second longList to compare, or null. </param>
        public static bool operator !=(longListAccessor a, longListAccessor b)
        {
            return !(a == b);
        }
        /// <summary>
        /// Determines whether this instance and a specified object have the same value.
        /// </summary>
        /// <param name="obj">The longList to compare to this instance.</param>
        /// <returns>true if <paramref name="obj" /> is a longList and its value is the same as this instance; otherwise, false.</returns>
        public overri)::");
source->append(R"::(de bool Equals(object obj)
        {
            longListAccessor b = obj as longListAccessor;
            if (b == null)
                return false;
            return (this == b);
        }
        /// <summary>
        /// Return the hash code for this longList.
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
