using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.TSL
{
    /// <summary>
    /// Specifies the Trinity TSL Types.
    /// </summary>
    public enum TSLType
    {
        /// <summary>
        /// Unknown TSL type.
        /// </summary>
        Unknown,
        /// <summary>
        /// Byte.
        /// </summary>
        ByteType,
        /// <summary>
        /// Signed Byte.
        /// </summary>
        SByteType,
        /// <summary>
        /// Char
        /// </summary>
        CharType,
        /// <summary>
        /// Boolean
        /// </summary>
        BoolType,
        /// <summary>
        /// 16-bit signed integer
        /// </summary>
        ShortType,
        /// <summary>
        /// 16-bit unsigned integer
        /// </summary>
        UShortType,
        /// <summary>
        /// 32-bit signed integer
        /// </summary>
        IntType,//Int32
        /// <summary>
        /// 32-bit unsigned integer
        /// </summary>
        UIntType,
        /// <summary>
        /// 64-bit signed integer
        /// </summary>
        LongType,//Int64
        /// <summary>
        /// 64-bit unsigned integer
        /// </summary>
        ULongType,
        /// <summary>
        /// 32-bit floating-point number
        /// </summary>
        FloatType,
        /// <summary>
        /// double-precision floating-point number
        /// </summary>
        DoubleType,
        /// <summary>
        /// Decimal
        /// </summary>
        DecimalType,
        /// <summary>
        /// List
        /// </summary>
        ListType,
        /// <summary>
        /// String
        /// </summary>
        StringType,
        /// <summary>
        /// Bit array
        /// </summary>
        BitArrayType,
        /// <summary>
        /// Bit list
        /// </summary>
        BitListType,
        /// <summary>
        /// DateTime
        /// </summary>
        DateTimeType,
        /// <summary>
        /// Guid
        /// </summary>
        GuidType
    }
}
