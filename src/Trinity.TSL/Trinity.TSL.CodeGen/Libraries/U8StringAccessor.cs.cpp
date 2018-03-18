#include "common.h"
#include <string>
#include "SyntaxNode.h"

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        string* 
U8StringAccessor(
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
using Trinity.Storage;
namespace )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(
{
    /// <summary>
    /// Represents a TSL string corresponding to a string instance.
    /// </summary>
    public unsafe class U8StringAccessor : IAccessor
    {
        internal byte* m_ptr;
        internal long  CellId;
        internal U8StringAccessor(byte* _CellPtr, ResizeFunctionDelegate func)
        {
            m_ptr = _CellPtr;
            ResizeFunction = func;
            m_ptr += 4;
        }
        /// <summary>
        /// Gets the number of bytes in the current U8String object.
        /// </summary>
        public int Length
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
            byte[] ret = new byte[Length];
            fixed (b)::");
source->append(R"::(yte* retptr = ret)
            {
                Memory.Copy(m_ptr, retptr, Length);
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
            return Length + sizeof(int);
        }
        /// <summary>
        /// The ResizeFunctionDelegate that should be called when this accessor is trying to resize itself.
        /// </summary>
        public ResizeFunctionDelegate ResizeFunction { get; set; }
        #endregion
        /// <summary>
        /// Returns this instance of String
        /// </summary>
        /// <returns>The current string.</returns>
        public unsafe override string ToString()
        {
          )::");
source->append(R"::(  int len = this.Length;
            byte[] content = new byte[len];
            fixed (byte* pcontent = content)
            {
                Memory.Copy(this.m_ptr, pcontent, len);
            }
            return Encoding.UTF8.GetString(content);
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
       )::");
source->append(R"::(      */
            return ToString().Contains(substring);
        }
        /// <summary>
        /// Implicitly converts a StringAccessor instance to a string instance.
        /// </summary>
        /// <param name="accessor">The StringAccessor instance.</param>
        /// <returns>A string instance.</returns>
        public unsafe static implicit operator string(U8StringAccessor accessor)
        {
            if ((object)accessor == null) return null;
            return accessor.ToString();
        }
        /// <summary>
        /// Implicitly converts a string instance to a U8String instance.
        /// </summary>
        /// <param name="value">The string instance.</param>
        /// <returns>The StringAccessor instance.</returns>
        public unsafe static implicit operator U8StringAccessor(string value)
        {
            if (value == null) value = "";
            byte[] content = Encoding.UTF8.GetBytes(value);
            int    len     = content.Length;
            )::");
source->append(R"::(byte* targetPtr = BufferAllocator.AllocBuffer(sizeof(int) + len);
            *(int*)targetPtr = len;
            Memory.Copy(content, targetPtr + sizeof(int), len);
            U8StringAccessor ret = new U8StringAccessor(targetPtr, null);
            return ret;
        }
        /// <summary>
        /// Determines whether two specified StringAccessor have the same value.
        /// </summary>
        /// <param name="a">The first StringAccessor to compare, or null. </param>
        /// <param name="b">The second StringAccessor to compare, or null. </param>
        /// <returns>true if the value of <paramref name="a" /> is the same as the value of <paramref name="b" />; otherwise, false.</returns>
        public static bool operator ==(U8StringAccessor a, U8StringAccessor b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;
            if (a.m_ptr == b.m_ptr) ret)::");
source->append(R"::(urn true;
            return (a.ToString() == b.ToString());
        }
        /// <summary>
        /// Determines whether the specified StringAccessor and string have the same value.
        /// </summary>
        /// <param name="a">The StringAccessor to compare, or null. </param>
        /// <param name="b">The string to compare, or null. </param>
        /// <returns>true if the value of <paramref name="a" /> is the same as the value of <paramref name="b" />; otherwise, false.</returns>
        public static bool operator ==(U8StringAccessor a, string b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;
            return a.ToString() == b;
        }
        /// <summary>Determines whether the specified StringAccessor and string have different values.</summary>
        /// <returns>true if the value of <paramref name="a" /> is different from the value of <param)::");
source->append(R"::(ref name="b" />; otherwise, false.</returns>
        /// <param name="a">The StringAccessor to compare, or null. </param>
        /// <param name="b">The String to compare, or null. </param>
        public static bool operator !=(U8StringAccessor a, string b)
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
            if ((object)b ==)::");
source->append(R"::( null)
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
            return HashHelper.HashBytes(this.m_ptr, this.Length);
        }
        /// <summary>Determines whether the two specified StringAccessor have different values.</summary>
        /// <returns>true if the value of <paramref name="a" /> is different from the value of <paramref name="b" />; otherwise, false.</returns>
        /// <param name="a">The first StringAccessor to compare, or null. </param>
        /// <param name="b">The second StringAccessor to compare, or null. </param>
        publ)::");
source->append(R"::(ic static bool operator !=(U8StringAccessor a, U8StringAccessor b)
        {
            return !(a == b);
        }
    }
}
)::");

            return source;
        }
    }
}
