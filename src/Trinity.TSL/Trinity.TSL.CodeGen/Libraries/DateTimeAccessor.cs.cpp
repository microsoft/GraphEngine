#include "common.h"
#include <string>
#include "SyntaxNode.h"

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        string* 
DateTimeAccessor(
NTSL* node)
        {
            string* source = new string();
            
source->append(R"::(using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Represents a Trinity data type that corresponds .Net DateTime.
    /// </summary>
    public unsafe class DateTimeAccessor : IAccessor
    {
        internal byte* m_ptr;
        internal long CellId;
        /// <summary>
        ///     Converts the specified string representation of a date and time to its <see cref="Trinity.TSL.Lib.DateTimeAccessor"/>
        ///     equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        ///
        /// <param name="input">
        ///     A string containing a date and time to convert.
        /// </param>
        ///
        /// <param name="value">
        ///     When this method returns, contains the <see cref="Trinity.TSL.Lib.DateTimeAccessor"/> value equivalent to
        ///     the date and time contained in s, if the conversion succeeded, or <see cref="System.DateTime.MinValue"/>
        ///     if the conversion failed. The conversion fails if the s paramete)::");
source->append(R"::(r is null,
        ///     is an empty string (""), or does not contain a valid string representation
        ///     of a date and time. This parameter is passed uninitialized.
        /// </param>
        ///
        /// <returns>
        ///     true if the s parameter was converted successfully; otherwise, false.
        /// </returns>
        public static bool TryParse(string input, out DateTimeAccessor value)
        {
            DateTime val;
            if (DateTime.TryParse(input, null, DateTimeStyles.RoundtripKind, out val))
            {
                value = val;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }
        /// <summary>
        ///     Converts the specified string representation of a date and time to its <see cref="System.DateTime"/>
        ///     equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summar)::");
source->append(R"::(y>
        ///
        /// <param name="input">
        ///     A string containing a date and time to convert.
        /// </param>
        ///
        /// <param name="value">
        ///     When this method returns, contains the <see cref="System.DateTime"/> value equivalent to
        ///     the date and time contained in s, if the conversion succeeded, or <see cref="System.DateTime.MinValue"/>
        ///     if the conversion failed. The conversion fails if the s parameter is null,
        ///     is an empty string (""), or does not contain a valid string representation
        ///     of a date and time. This parameter is passed uninitialized.
        /// </param>
        ///
        /// <returns>
        ///     true if the s parameter was converted successfully; otherwise, false.
        /// </returns>
        public static bool TryParse(string input, out DateTime value)
        {
            return DateTime.TryParse(input, null, DateTimeStyles.RoundtripKind, out value);
      )::");
source->append(R"::(  }
        internal DateTimeAccessor(byte* _CellPtr)
        {
            m_ptr = _CellPtr;
        }
        internal int length
        {
            get
            {
                return sizeof(long);
            }
        }
        /// <summary>
        /// Serializes the current DateTime object to a 64-bit binary value that subsequently can be used to recreate the DateTime object.
        /// </summary>
        /// <returns>A 64-bit signed integer that encodes the .Net DateTime. </returns>
        public unsafe long ToBinary()
        {
            return *(long*)m_ptr;
        }
        #region IAccessor Implementation
        /// <summary>
        /// Returns an eight byte array that contains the value of this instance.
        /// </summary>
        /// <returns>An eight byte array.</returns>
        public unsafe byte[] ToByteArray()
        {
            byte[] ret = new byte[sizeof(long)];
            fixed (byte* ptr = ret)
            {
                Memory.Co)::");
source->append(R"::(py(m_ptr, ptr, length);
            }
            return ret;
        }
        /// <summary>
        /// Get the pointer to the underlying buffer.
        /// </summary>
        public unsafe byte* GetUnderlyingBufferPointer()
        {
            return m_ptr;
        }
        /// <summary>
        /// Get the length of the buffer.
        /// </summary>
        public unsafe int GetBufferLength()
        {
            return length;
        }
        /// <summary>
        /// The ResizeFunctionDelegate that should be called when this accessor is trying to resize itself.
        /// </summary>
        public ResizeFunctionDelegate ResizeFunction { get; set; }
        #endregion
        /// <summary>
        /// Converts the value of the current DateTime object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current DateTime object.</returns>
        public override unsafe string ToString()
        )::");
source->append(R"::({
            return DateTime.FromBinary(this.ToBinary()).ToString("o", CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Converts current DateTimeAccessor instance to DateTime.
        /// </summary>
        /// <returns>A DateTime value corresponding to current DateTimeAccessor.</returns>
        public unsafe DateTime ToDateTime()
        {
            return DateTime.FromBinary(this.ToBinary());
        }
        /// <summary>
        /// Converts current DateTimeAccessor instance to DateTime.
        /// </summary>
        /// <param name="value">A DateTimeAccessor instance.</param>
        /// <returns>A DateTime value corresponding to current DateTimeAccessor.</returns>
        public unsafe static implicit operator DateTime(DateTimeAccessor value)
        {
            return DateTime.FromBinary(value.ToBinary());
        }
        /// <summary>
        /// Converts a DateTime instance to DateTimeAccessor.
        /// </summary>
        /// <param name=")::");
source->append(R"::(value">A DateTime instance.</param>
        /// <returns>A DateTimeAccessor instance corresponding to the specified DateTime.</returns>
        public unsafe static implicit operator DateTimeAccessor(DateTime value)
        {
            byte* targetPtr = BufferAllocator.AllocBuffer(sizeof(long));
            if (value != null)
            {
                *(long*)targetPtr = value.ToBinary();
            }
            else
            {
                *(long*)targetPtr = 0;
            }
            DateTimeAccessor ret = new DateTimeAccessor(targetPtr);
            return ret;
        }
        /// <summary>
        /// Returns a value indicating whether two given DateTimeAccessor instances have the same value.
        /// </summary>
        /// <param name="a">The first DateTimeAccessor instance.</param>
        /// <param name="b">The second DateTimeAccessor instance.</param>
        /// <returns>true if two given DateTimeAccessor instances have the same value; otherwise, false.</re)::");
source->append(R"::(turns>
        public static bool operator ==(DateTimeAccessor a, DateTimeAccessor b)
        {
            if (ReferenceEquals(a, b))
              return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
              return false;
            if (a.m_ptr == b.m_ptr) return true;
            return (*(long*)a.m_ptr) == (*(long*)b.m_ptr);
        }
        /// <summary>
        /// Returns a value indicating whether two given DateTimeAccessor instances have the same value.
        /// </summary>
        /// <param name="a">The first DateTimeAccessor instance.</param>
        /// <param name="b">The second DateTimeAccessor instance.</param>
        /// <returns>true if two given DateTimeAccessor instances do not have the same value; otherwise, false.</returns>
        public static bool operator !=(DateTimeAccessor a, DateTimeAccessor b)
        {
            return !(a == b);
        }
        /// <summary>
        /// Returns a value indicating whether this ins)::");
source->append(R"::(tance is equal to a specified object.
        /// </summary>
        /// <param name="value">An object to compare to this instance.</param>
        /// <returns>true if value is an instance of DateTimeAccessor and equals the value of this instance; otherwise, false.</returns>
        public override bool Equals(object value)
        {
            if (value is DateTimeAccessor)
            {
                DateTimeAccessor date = (DateTimeAccessor)value;
                return this == date;
            }
            return false;
        }
        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return (*(long*)m_ptr).GetHashCode();
        }
    }
}
)::");

            return source;
        }
    }
}
