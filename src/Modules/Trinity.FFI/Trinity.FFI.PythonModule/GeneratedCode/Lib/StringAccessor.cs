#pragma warning disable 162,168,649,660,661,1522
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity.Core.Lib;
using Trinity.TSL;
using Trinity.TSL.Lib;
using Trinity.Storage;
namespace CellAssembly
{
    /// <summary>
    /// Represents a TSL string corresponding to a string instance.
    /// </summary>
    public unsafe class StringAccessor : IAccessor, IEnumerable<char>
    {
        internal byte* CellPtr;
        internal long? CellID;
        internal StringAccessor(byte* _CellPtr, ResizeFunctionDelegate func)
        {
            CellPtr = _CellPtr;
            ResizeFunction = func;
            CellPtr += 4;
        }
        private int length
        {
            get
            {
                return *(int*)(CellPtr - 4);
            }
        }
        /// <summary>
        /// Gets the number of characters in the current String object.
        /// </summary>
        public int Length
        {
            get
            {
                return length >> 1;
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
                Memory.Copy(CellPtr, retptr, length);
                return ret;
            }
        }
        /// <summary>
        /// Get the pointer to the underlying buffer.
        /// </summary>
        public unsafe byte* GetUnderlyingBufferPointer()
        {
            return CellPtr - sizeof(int);
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
        /// Gets the Char object at a specified position in the current String object.
        /// </summary>
        /// <param name="index">A position in the current string. </param>
        /// <returns>The object at position index.</returns>
        public unsafe char this[int index]
        {
            get
            {
                return *(char*)(CellPtr + (index << 1));
            }
        }
        /// <summary>
        /// Returns this instance of String
        /// </summary>
        /// <returns>The current string.</returns>
        public unsafe override string ToString()
        {
            return Trinity.Core.Lib.BitHelper.GetString(CellPtr, *(int*)(CellPtr - 4));
        }
        /// <summary>
        /// Returns a value indicating whether the given substring occurs within the string.
        /// </summary>
        /// <param name="substring">The string to seek.</param>
        /// <returns>true if the value parameter occurs within this string, or if value is 
        ///          the empty string (""); otherwise, false.
        /// </returns>
        public unsafe bool Contains(string substring)
        {
            /*
             *  @note   Relying on .NET's implementation of string search
             *          seems to be a better idea than grinding out our own
             *          version, especially because that we're dealing with
             *          UTF-16 NLS style strings.
             */
            return ToString().Contains(substring);
        }
        /// <summary>
        /// Performs the specified action on each char
        /// </summary>
        /// <param name="action">A lambda expression which has one parameter indicates char in string</param>
        public unsafe void ForEach(Action<char> action)
        {
            byte* targetPtr = CellPtr;
            byte* endPtr = CellPtr + length;
            while (targetPtr < endPtr)
            {
                action(*(char*)targetPtr);
                targetPtr += 2;
            }
        }
        /// <summary>
        /// Performs the specified action on each char
        /// </summary>
        /// <param name="action">A lambda expression which has two parameters. First indicates char in the string and second the index of this char.</param>
        public unsafe void ForEach(Action<char, int> action)
        {
            byte* targetPtr = CellPtr;
            byte* endPtr = CellPtr + length;
            for (int index = 0; targetPtr < endPtr; ++index)
            {
                action(*(char*)targetPtr, index);
                targetPtr += 2;
            }
        }
        internal unsafe struct _iterator
        {
            byte* targetPtr;
            byte* endPtr;
            internal _iterator(StringAccessor target)
            {
                targetPtr = target.CellPtr;
                endPtr    = target.CellPtr + target.length;
            }
            internal bool good()
            {
                return (targetPtr < endPtr);
            }
            internal char current()
            {
                return *(char*)targetPtr;
            }
            internal void move_next()
            {
                targetPtr += 2;
            }
        }
        /// <summary>
        /// Returns an enumerator that iterate through current string.
        /// </summary>
        /// <returns>
        /// An IEnumerator object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<char> GetEnumerator()
        {
            _iterator it = new _iterator(this);
            while(it.good())
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
        /// Implicitly converts a StringAccessor instance to a string instance.
        /// </summary>
        /// <param name="accessor">The StringAccessor instance.</param>
        /// <returns>A string instance.</returns>
        public unsafe static implicit operator string(StringAccessor accessor)
        {
            if ((object)accessor == null) return null;
            return new string((char*)accessor.CellPtr, 0, accessor.length >> 1);
        }
        /// <summary>
        /// Implicitly converts a string instance to a BlogString instance.
        /// </summary>
        /// <param name="value">The string instance.</param>
        /// <returns>The StringAccessor instance.</returns>
        public unsafe static implicit operator StringAccessor(string value)
        {
            byte* targetPtr = null;
            if (value != null)
            {
                targetPtr += (value.Length << 1) + sizeof(int);
            }
            else
            {
                targetPtr += sizeof(int);
            }
            byte* tmpcellptr = BufferAllocator.AllocBuffer((int)targetPtr);
            targetPtr = tmpcellptr;
            if (value != null)
            {
                *(int*)targetPtr = (value.Length << 1);
                targetPtr += sizeof(int);
                fixed(char* pstr = value)
                {
                    Memory.Copy(pstr, targetPtr, value.Length << 1);
                }
            }
            else
            {
                *(int*)targetPtr = 0;
            }
            StringAccessor ret = new StringAccessor(tmpcellptr, null);
            ret.CellID = null;
            return ret;
        }
        /// <summary>
        /// Determines whether two specified StringAccessor have the same value.
        /// </summary>
        /// <param name="a">The first StringAccessor to compare, or null. </param>
        /// <param name="b">The second StringAccessor to compare, or null. </param>
        /// <returns>true if the value of <paramref name="a" /> is the same as the value of <paramref name="b" />; otherwise, false.</returns>
        public static bool operator ==(StringAccessor a, StringAccessor b)
        {
            if (ReferenceEquals(a, b))
              return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
              return false;
            if (a.CellPtr == b.CellPtr) return true;
            return (a.ToString() == b.ToString());
        }
        /// <summary>
        /// Determines whether the specified StringAccessor and string have the same value.
        /// </summary>
        /// <param name="a">The StringAccessor to compare, or null. </param>
        /// <param name="b">The string to compare, or null. </param>
        /// <returns>true if the value of <paramref name="a" /> is the same as the value of <paramref name="b" />; otherwise, false.</returns>
        public static bool operator ==(StringAccessor a, string b)
        {
            if (ReferenceEquals(a, b))
              return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
              return false;
            return a.ToString() == b;
        }
        /// <summary>Determines whether the specified StringAccessor and string have different values.</summary>
        /// <returns>true if the value of <paramref name="a" /> is different from the value of <paramref name="b" />; otherwise, false.</returns>
        /// <param name="a">The StringAccessor to compare, or null. </param>
        /// <param name="b">The String to compare, or null. </param>
        public static bool operator !=(StringAccessor a, string b)
        {
            if ((object)a == null && (object)b == null) return false;
            if (((object)a == null) || ((object)b == null)) return true;
            return a.ToString() != b;
        }
        /// <summary>
        /// Determines whether this instance and a specified object have the same value.
        /// </summary>
        /// <param name="obj">The StringAccessor to compare to this instance.</param>
        /// <returns>true if obj is a StringAccessor and its value is the same as this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            StringAccessor b = obj as StringAccessor;
            if ((object)b == null)
            {
                string s = obj as string;
                if ((object)s == null)
                {
                    return false;
                }
                return this.ToString() == s;
            }
            return this.ToString() == b.ToString();
        }
        /// <summary>Returns the hash code for this StringAccessor.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override unsafe int GetHashCode()
        {
            char* strPtr = (char*)CellPtr;
            int n1 = 0x15051505;
            int n2 = n1;
            int* intPtr = (int*)strPtr;
            for (int i = (length >> 1); i > 0; i -= 4)
            {
                n1 = (((n1 << 5) + n1) + (n1 >> 0x1b)) ^ intPtr[0];
                if (i <= 2)
                {
                    break;
                }
                n2 = (((n2 << 5) + n2) + (n2 >> 0x1b)) ^ intPtr[1];
                intPtr += 2;
            }
            return (n1 + (n2 * 0x5d588b65));
        }
        /// <summary>Determines whether the two specified StringAccessor have different values.</summary>
        /// <returns>true if the value of <paramref name="a" /> is different from the value of <paramref name="b" />; otherwise, false.</returns>
        /// <param name="a">The first StringAccessor to compare, or null. </param>
        /// <param name="b">The second StringAccessor to compare, or null. </param>
        public static bool operator !=(StringAccessor a, StringAccessor b)
        {
            return !(a == b);
        }
    }
}

#pragma warning restore 162,168,649,660,661,1522
