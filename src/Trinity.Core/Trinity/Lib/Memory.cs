// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Runtime;

namespace Trinity.Core.Lib
{
    /// <summary>
    /// Provides methods for memory management.
    /// </summary>
    public unsafe static partial class Memory
    {
        static int warmup = 0;

        /// <summary>
        /// Read the first byte of each committed memory pages.
        /// </summary>
        internal static void WarmUp(byte* buff, uint memory_head)
        {
            if (memory_head > 0)
            {
                uint pageCount = memory_head >> 12;

                if (memory_head > (pageCount << 12))
                    pageCount++;

                for (int i = 0; i < pageCount; i++)
                {
                    warmup += buff[i << 12];
                }
            }
        }

        /// <summary>
        /// Compare bytes in two buffers.
        /// </summary>
        /// <param name="buff1">First buffer.</param>
        /// <param name="buff2">Second buffer.</param>
        /// <param name="count">Number of bytes to compare.</param>
        /// <returns>true if buff1 identical to buff2, otherwise false.</returns>
        public static unsafe bool Compare(byte* buff1, byte* buff2, int count)
        {
            return CMemory.C_memcmp(buff2, buff1, (ulong)count) == 0;
        }

        /// <summary>
        /// Compare bytes in two buffers.
        /// </summary>
        /// <param name="buff1">First buffer.</param>
        /// <param name="buff2">Second buffer.</param>
        /// <returns>true if buff1 identical to buff2, otherwise false.</returns>
        public static unsafe bool Compare(byte[] buff1, byte[] buff2)
        {
            if (buff1.Length != buff2.Length)
                return false;

            fixed (byte* src_p = buff1)
            {
                fixed (byte* dest_p = buff2)
                {
                    return Compare(src_p, dest_p, buff1.Length);
                }
            }
        }


        /// <summary>
        /// Compare bytes in two buffers.
        /// </summary>
        /// <param name="buff1">First buffer.</param>
        /// <param name="buff2">Second buffer.</param>
        /// <param name="count">Number of bytes to compare.</param>
        /// <returns>true if buff1 identical to buff2, otherwise false.</returns>
        public static unsafe bool Compare(byte[] buff1, byte[] buff2, int count)
        {
            fixed (byte* src_p = buff1)
            {
                fixed (byte* dest_p = buff2)
                {
                    return Compare(src_p, dest_p, count);
                }
            }
        }

        /// <summary>
        /// Compare bytes in two buffers.
        /// </summary>
        /// <param name="buff1">First buffer.</param>
        /// <param name="buff2">Second buffer.</param>
        /// <returns>true if buff1 identical to buff2, otherwise false.</returns>
        public static unsafe bool Compare(byte* buff1, byte[] buff2)
        {
            fixed (byte* p = buff2)
            {
                return Compare(buff1, p, buff2.Length);
            }
        }

        /// <summary>
        /// Compare bytes in two buffers.
        /// </summary>
        /// <param name="buff1">First buffer.</param>
        /// <param name="buff2">Second buffer.</param>
        /// <returns>true if buff1 identical to buff2, otherwise false.</returns>
        public static unsafe bool Compare(byte[] buff1, byte* buff2)
        {
            fixed (byte* p = buff1)
            {
                return Compare(p, buff2, buff1.Length);
            }
        }

        /// <summary>
        /// Copies a specified number of bytes from a source buffer to a destination buffer.
        /// </summary>
        /// <param name="src">The source buffer.</param>
        /// <param name="dst">The destination buffer.</param>
        /// <param name="count">The number of bytes to copy.</param>
        public static unsafe void Copy(byte[] src, void* dst, int count)
        {
            fixed (byte* numRef = src)
            {
                CMemory.Copy(numRef, dst, count);
            }
        }

        /// <summary>
        /// Copies a specified number of bytes from a source buffer starting at a particular offset to a destination buffer starting at a particular offset.
        /// </summary>
        /// <param name="src">The source buffer.</param>
        /// <param name="srcOffset">The byte offset into ptr2.</param>
        /// <param name="dst">The destination buffer.</param>
        /// <param name="dstOffset">The byte offset into ptr1.</param>
        /// <param name="count">The number of bytes to copy.</param>
        public static unsafe void Copy(void* src, int srcOffset, void* dst, int dstOffset, int count)
        {
            CMemory.Copy((byte*)src + srcOffset, (byte*)dst + dstOffset, count);
        }

        /// <summary>
        /// Copies a specified number of bytes from a source buffer starting at a particular offset to a destination buffer.
        /// </summary>
        /// <param name="src">The source buffer.</param>
        /// <param name="srcOffset">The byte offset into ptr2.</param>
        /// <param name="dst">The destination buffer.</param>
        /// <param name="count">The number of bytes to copy.</param>
        public static unsafe void Copy(byte[] src, int srcOffset, void* dst, int count)
        {
            fixed (byte* numRef = src)
            {
                CMemory.Copy(numRef + srcOffset, dst, count);
            }
        }

        /// <summary>
        /// Copies a specified number of bytes from a source buffer starting at a particular offset to a destination buffer starting at a particular offset.
        /// </summary>
        /// <param name="src">The source buffer.</param>
        /// <param name="srcOffset">The byte offset into ptr2.</param>
        /// <param name="dst">The destination buffer.</param>
        /// <param name="dstOffset">The byte offset into ptr1.</param>
        /// <param name="count">The number of bytes to copy.</param>
        public static unsafe void Copy(byte[] src, int srcOffset, void* dst, int dstOffset, int count)
        {
            fixed (byte* numRef = src)
            {
                CMemory.Copy(numRef + srcOffset, (byte*)dst + dstOffset, count);
            }
        }

        /// <summary>
        /// Copies a specified number of bytes from a source buffer starting at a particular offset to a destination buffer starting at a particular offset.
        /// </summary>
        /// <param name="src">The source buffer.</param>
        /// <param name="srcOffset">The byte offset into ptr2.</param>
        /// <param name="dst">The destination buffer.</param>
        /// <param name="dstOffset">The byte offset into ptr1.</param>
        /// <param name="count">The number of bytes to copy.</param>
        public static unsafe void Copy(void* src, int srcOffset, byte[] dst, int dstOffset, int count)
        {
            fixed (byte* numRef = dst)
            {
                CMemory.Copy((byte*)src + srcOffset, numRef + dstOffset, count);
            }
        }

        /// <summary>
        /// Copies a specified number of bytes from a source buffer starting at a particular offset to a destination buffer starting at a particular offset.
        /// </summary>
        /// <param name="src">The source buffer.</param>
        /// <param name="srcOffset">The byte offset into ptr2.</param>
        /// <param name="dst">The destination buffer.</param>
        /// <param name="dstOffset">The byte offset into ptr1.</param>
        /// <param name="count">The number of bytes to copy.</param>
        public static unsafe void Copy(byte[] src, int srcOffset, byte[] dst, int dstOffset, int count)
        {
            fixed (byte* src_p = src)
            {
                fixed (byte* numRef = dst)
                {
                    CMemory.Copy(src_p + srcOffset, numRef + dstOffset, count);
                }
            }
        }

        /// <summary>
        /// Copies a specified number of bytes from a source buffer starting at a particular offset to a destination buffer starting at a particular offset.
        /// </summary>
        /// <param name="src">The source buffer.</param>
        /// <param name="srcOffset">The byte offset into ptr2.</param>
        /// <param name="dst">The destination buffer.</param>
        /// <param name="dstOffset">The byte offset into ptr1.</param>
        /// <param name="count">The number of bytes to copy.</param>
        public static unsafe void Copy(byte[] src, int srcOffset, long[] dst, int dstOffset, int count)
        {
            fixed (byte* src_p = src)
            {
                fixed (long* numRef = dst)
                {
                    CMemory.Copy(src_p + srcOffset, (byte*)numRef + dstOffset, count);
                }
            }
        }

        /// <summary>
        /// Copies a specified number of bytes from an array of 64-bit integer starting at a particular offset to a destination buffer starting at a particular offset.
        /// </summary>
        /// <param name="src">The source buffer.</param>
        /// <param name="srcOffset">The byte offset into ptr2.</param>
        /// <param name="dst">The destination buffer.</param>
        /// <param name="dstOffset">The byte offset into ptr1.</param>
        /// <param name="count">The number of bytes to copy.</param>
        public static unsafe void Copy(long[] src, int srcOffset, byte[] dst, int dstOffset, int count)
        {
            fixed (long* src_p = src)
            {
                fixed (byte* numRef = dst)
                {
                    CMemory.Copy((byte*)src_p + srcOffset, numRef + dstOffset, count);
                }
            }
        }

        /// <summary>
        /// Copies a specified number of bytes from an array of 64-bit double starting at a particular offset to a destination buffer.
        /// </summary>
        /// <param name="src">The source buffer.</param>
        /// <param name="srcOffset">The byte offset into source array of double values.</param>
        /// <param name="dst">The destination buffer.</param>
        /// <param name="count">The number of bytes to copy.</param>
        public static unsafe void Copy(double[] src, int srcOffset, void* dst, int count)
        {
            fixed (double* src_p = src)
            {
                CMemory.Copy(src_p + srcOffset, dst, count);
            }
        }

        /// <summary>
        /// Copies a specified number of bytes from a source buffer to an array of 64-bit double starting at a particular offset.
        /// </summary>
        /// <param name="src">The source buffer.</param>
        /// <param name="dst">The destination buffer.</param>
        /// <param name="dstOffset">The byte offset into the destination array of double values.</param>
        /// <param name="count">The number of bytes to copy.</param>
        public static unsafe void Copy(void* src, double[] dst, int dstOffset, int count)
        {
            fixed (double* dst_p = dst)
            {
                CMemory.Copy(src, dst_p + dstOffset, count);
            }
        }

        /// <summary>
        /// Copies a specified number of bytes from an array of 64-bit double starting at a particular offset to a destination buffer starting at a particular offset.
        /// </summary>
        /// <param name="src">The source array of double values.</param>
        /// <param name="srcOffset">The offset into the source array.</param>
        /// <param name="dst">The destination array of double values.</param>
        /// <param name="dstOffset">THe offset into the destination array.</param>
        /// <param name="count">Note: The number of double values.</param>
        public static void Copy(double[] src, int srcOffset, double[] dst, int dstOffset, int count)
        {
            fixed (double* src_p = src)
            {
                fixed (double* dst_p = dst)
                {
                    CMemory.Copy(src_p + srcOffset, dst_p + dstOffset, count << 3);
                }
            }
        }
    }
}
