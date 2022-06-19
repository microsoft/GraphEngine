// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.Utilities
{
    /// <summary>
    /// C-Style string routines
    /// </summary>
    internal class CString
    {
        /// <summary>
        /// Seek for a given byte, from inclusive start to exclusive end.
        /// User must guarantee that start is less than end, or it will crash.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="target"></param>
        /// <returns>If the given byte is found, the position is returned. Otherwise null is returned.</returns>
        public static unsafe byte* SeekForByte(byte* start, byte* end, byte target)
        {
            for (; start != end; ++start)
                if (*start == target)
                    return start;
            return null;
        }
        /// <summary>
        /// Same semantic with cstdlib
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static bool isNumber(byte x)
        {
            return (x > 47 && x < 58);
        }
        /// <summary>
        /// Dump integers from a literal buffer
        /// Note that SIGN IS IGNORED
        /// </summary>
        /// <param name="IntegerList"></param>
        /// <param name="bufPtr"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns>The count of parsed integers</returns>
        public static unsafe int DumpIntegers(long[] IntegerList, byte* bufPtr, int startIndex, int length)
        {
            int count = 0;
            IntegerList[0] = 0;
            bool nFlag = true;
            if (!isNumber(*(bufPtr + startIndex)))
                return 0;
            for (byte* p = bufPtr + startIndex, q = bufPtr + startIndex + length; p != q; ++p)
            {
                if (nFlag)
                {
                    IntegerList[count] = IntegerList[count] * 10 + *p - 48;
                    if (p + 1 != q && !isNumber(*(p + 1)))
                        nFlag = false;
                }
                else
                {
                    if (p + 1 != q && isNumber(*(p + 1)))
                    {
                        ++count;
                        IntegerList[count] = 0;
                        nFlag = true;
                    }
                }
            }
            return count + 1;
        }
        /// <summary>
        /// Try to parse an integer from a literal buffer, then move the pointer forward.
        /// If an integer cannot be parsed from the buffer, bufPtr will be pushed to end.
        /// Note that SIGN IS IGNORED
        /// </summary>
        /// <param name="bufPtr">Pointer to the buffer, note that it will be pushed forward</param>
        /// <param name="end"></param>
        /// <param name="result">The parsed result</param>
        /// <returns>True if the result is parsed, false if parsing fails.</returns>
        public static unsafe bool TryParseInteger(ref byte* bufPtr, byte* end, out int result)
        {
            while (bufPtr != end && !isNumber(*bufPtr))
            {
                ++bufPtr;
            }
            if (bufPtr == end)//cannot find an integer
            {
                result = -1;
                return false;
            }
            result = 0;
            while (bufPtr != end)
            {
                if (!isNumber(*bufPtr))
                    break;
                result = result * 10 + (*bufPtr - 48);
                ++bufPtr;
            }
            return true;
        }
    }
}
