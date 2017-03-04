using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity.TSL;
using Trinity.TSL.Lib;

/*MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")*/
namespace t_Namespace
{
    /// <summary>
    /// Represents a 256-bit enum type.
    /// </summary>
    [TARGET("NTSL")]
    public unsafe class EnumAccessor
    {
        internal byte* CellPtr;
        internal long? CellID;

        internal EnumAccessor(byte* _CellPtr)
        {
            CellPtr = _CellPtr;
        }

        internal int length
        {
            get
            {
                return sizeof(byte);
            }
        }

        /// <summary>
        /// Gets the byte value.
        /// </summary>
        /// <returns>A byte.</returns>
        public byte ToByte()
        {
            return *(byte*)CellPtr;
        }

        /// <summary>
        /// Gets the underlying blob.
        /// </summary>
        /// <returns>A byte array with size one.</returns>
        public byte[] ToByteArray()
        {
            return new byte[] { *(byte*)CellPtr };
        }

        /// <summary>
        /// Converts a EnumAccessor accessor to a byte value.
        /// </summary>
        /// <param name="accessor">A EnumAccessor accessor.</param>
        /// <returns>A byte.</returns>
        public static implicit operator byte(EnumAccessor accessor)
        {
            return *(byte*)accessor.CellPtr;
        }

        /// <summary>
        /// Converts a byte value to a EnumAccessor value.
        /// </summary>
        /// <param name="value">A byte value.</param>
        /// <returns>A EnumAccessor value.</returns>
        public static implicit operator EnumAccessor(byte value)
        {
            byte* ptr = BufferAllocator.AllocBuffer(sizeof(byte));
            *ptr = value;
            return new EnumAccessor(ptr);
        }

        /// <summary>
        /// Returns a value indicating whether two given EnumAccessor instances have the same value.
        /// </summary>
        /// <param name="a">A EnumAccessor instance.</param>
        /// <param name="b">Another EnumAccessor instance.</param>
        /// <returns>true if the two given EnumAccessor instances have the same value; otherwise, false.</returns>
        public static bool operator ==(EnumAccessor a, EnumAccessor b)
        {
            if (ReferenceEquals(a, b))
              return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
              return false;
            return *a.CellPtr == *b.CellPtr;
        }

        /// <summary>
        /// Returns a value indicating whether two given EnumAccessor instances have the same value.
        /// </summary>
        /// <param name="a">A EnumAccessor instance.</param>
        /// <param name="b">Another EnumAccessor instance.</param>
        /// <returns>true if the two given EnumAccessor instances do not have the same value; otherwise, false.</returns>
        public static bool operator !=(EnumAccessor a, EnumAccessor b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance or null.</param>       
        /// <returns>true if obj is an instance of EnumAccessor and equals the value of this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return ((obj is EnumAccessor) && ((*CellPtr) == (((EnumAccessor)obj).ToByte())));
        }

        /// <summary>
        /// Returns the hash code of the underlying value.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return (*CellPtr).GetHashCode();
        }
    }
}
