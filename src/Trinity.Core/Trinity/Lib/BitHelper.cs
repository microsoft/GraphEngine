// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;

namespace Trinity.Core.Lib
{
    /// <summary>
    /// A Helper class for manipulating blob related data structures.
    /// </summary>
    public unsafe partial class BitHelper
    {
        /// <summary>
        /// Returns the specified string as an array of bytes.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>An array of bytes.</returns>
        public static byte[] GetBytes(string value)
        {
            int length = value.Length;
            byte[] buffer = new byte[length << 1];
            fixed (byte* bp = buffer)
            {
                byte* ptr = bp;
                for (int k = 0; k < length; k++)
                {
                    *((char*)(ptr)) = value[k];
                    ptr += 2;
                }
            }
            return buffer;
        }

        internal static byte[] GetAsciiCharArray(string value)
        {
            int length = value.Length;
            byte[] buffer = new byte[length];
            fixed (byte* bp = buffer)
            {
                byte* ptr = bp;
                for (int k = 0; k < length; k++)
                {
                    *ptr = (byte)value[k];
                    ptr++;
                }
            }
            return buffer;
        }

        /// <summary>
        /// Returns the specified string as an array of bytes with a header containing the string buffer length.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>An array of bytes.</returns>
        public static byte[] GetBytesWithHeader(string value)
        {
            int length = value.Length;
            byte[] buffer = new byte[sizeof(int) + (length << 1)];
            fixed (byte* bp = buffer)
            {
                *(int*)bp = (length << 1);
                byte* ptr = bp + sizeof(int);
                for (int k = 0; k < length; k++)
                {
                    *((char*)(ptr)) = value[k];
                    ptr += 2;
                }
            }
            return buffer;
        }

        /// <summary>
        /// Write the specified value to the specified buffer starting from the given offset.
        /// </summary>
        /// <param name="value">The string value to write.</param>
        /// <param name="buffer">The buffer to write the string value in.</param>
        /// <param name="offset">The offset of the buffer to write; the offset is added (value.Length*2) after the writing.</param>
        public static void WriteString(string value, byte[] buffer, ref int offset)
        {
            int length = value.Length;
            fixed (byte* numRef = &(buffer[offset]))
            {
                byte* ptr = numRef;
                for (int k = 0; k < length; k++)
                {
                    *((short*)(ptr)) = (short)value[k];
                    ptr += 2;
                }
            }
            offset += (length << 1);
        }


        /// <summary>
        /// Write the specified value to the specified buffer.
        /// </summary>
        /// <param name="value">The string value to write.</param>
        /// <param name="buffer">The buffer to write the string value in.</param>
        public static void WriteString(string value, byte* buffer)
        {
            int length = value.Length;

            for (int k = 0; k < length; k++)
            {
                *((short*)(buffer)) = (short)value[k];
                buffer += 2;
            }
        }

        /// <summary>
        /// Gets the corresponding string from the specified buffer.
        /// </summary>
        /// <param name="buffer">The string buffer.</param>
        /// <returns>The corresponding string.</returns>
        public static string GetString(byte[] buffer)
        {
            int string_length = buffer.Length >> 1;
            char[] char_array = new char[string_length];
            fixed (byte* bp = buffer)
            {
                byte* ptr = bp;
                for (int k = 0; k < string_length; k++)
                {
                    char_array[k] = *((char*)(ptr));
                    ptr += 2;
                }
            }
            return new string(char_array);
        }

        /// <summary>
        /// Gets the corresponding string from the specified buffer starting from the given position.
        /// </summary>
        /// <param name="buffer">The string buffer.</param>
        /// <param name="startIndex">The zero-based byte offset into the buffer.</param>
        /// <param name="count">The number of bytes to convert.</param>
        /// <returns>The corresponding string.</returns>
        public static unsafe string GetString(byte[] buffer, int startIndex, int count)
        {
            fixed (byte* numRef = &(buffer[startIndex]))
            {
                return new string((char*)numRef, 0, count >> 1);
            }
        }

        /// <summary>
        ///  Gets the corresponding string from the specified buffer.
        /// </summary>
        /// <param name="buffer">The string buffer.</param>
        /// <param name="byteCount">The number of bytes to convert.</param>
        /// <returns>The corresponding string.</returns>
        public static unsafe string GetString(byte* buffer, int byteCount)
        {
            return new string((char*)buffer, 0, byteCount >> 1);
        }

        /// <summary>
        /// Gets the corresponding char array from the specified buffer starting from the given position.
        /// </summary>
        /// <param name="buffer">The string buffer.</param>
        /// <param name="startIndex">The zero-based byte offset into the buffer.</param>
        /// <param name="count">The number of bytes to convert.</param>
        /// <returns>The corresponding char array.</returns>
        public static unsafe char[] GetCharArray(byte[] buffer, int startIndex, int count)
        {
            int size = count >> 1;
            char[] array = new char[size];
            fixed (byte* numRef = &(buffer[startIndex]))
            {
                char* ptr = (char*)numRef;
                for (int k = 0; k < size; k++)
                {
                    array[k] = *ptr;
                    ptr++;
                }
            }
            return array;
        }

        /// <summary>
        /// Writes an IPEndPoint instance to the specified buffer.
        /// </summary>
        /// <param name="ipe">An IPEndPoint instance.</param>
        /// <param name="buffer">The buffer to write the ipe in.</param>
        public unsafe static void WriteIPEndPoint(IPEndPoint ipe, byte* buffer)
        {
            *(int*)buffer = BitHelper.ToInt32(ipe.Address.GetAddressBytes(), 0);
            *(int*)(buffer + 4) = ipe.Port;
        }

        /// <summary>
        /// Returns an IPAddress instance from eight bytes of the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="startIndex">A zero-based byte offset into the buffer.</param>
        /// <returns>An IPAddress instance.</returns>
        public static IPAddress ToIPAddress(byte* buffer, int startIndex)
        {
            Int64 m_address = (
                (
                ((buffer[startIndex + 3] << 0x18) | (buffer[startIndex + 2] << 0x10)) |
                (buffer[startIndex + 1] << 8)
                ) | buffer[startIndex]) & ((long)0xffffffffL);
            return new IPAddress(m_address);
        }

        /// <summary>
        /// Returns an IPEndPoint instance from eight bytes of the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>An IPEndPoint instance.</returns>
        public unsafe static IPEndPoint ToIPEndPoint(byte* buffer)
        {
            Int64 m_address = ((((*(buffer + 3) << 0x18) | (*(buffer + 2) << 0x10)) | (*(buffer + 1) << 8)) | *buffer) & ((long)0xffffffffL);
            return new IPEndPoint(m_address, *((int*)(buffer + 4)));
        }

        /// <summary>
        /// Returns a 32-bit signed integer converted from four bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="buffer">An array of bytes</param>
        /// <param name="startIndex">The starting position within the byte array.</param>
        /// <returns>A 32-bit signed integer formed by four bytes beginning at startIndex.</returns>
        public static unsafe Int32 ToInt32(byte[] buffer, int startIndex)
        {
            fixed (byte* p = &(buffer[startIndex]))
            {
                return *(((int*)p));
            }
        }

        /// <summary>
        /// Returns a Boolean value converted from one byte at a specified position in a byte array.
        /// </summary>
        /// <param name="buffer">An array of bytes.</param>
        /// <param name="startIndex">The starting position within the byte of array.</param>
        /// <returns>true if the byte at startIndex in the byte array is nonzero; otherwise, false.</returns>
        public static unsafe bool ToBoolean(byte[] buffer, int startIndex)
        {
            return (buffer[startIndex] != 0);
        }

        /// <summary>
        /// Returns a 16-bit unsigned integer converted from two bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="buffer">The array of bytes.</param>
        /// <param name="startIndex">The starting position within the byte array.</param>
        /// <returns>A 16-bit unsigned integer formed by two bytes beginning at startIndex.</returns>
        public static unsafe ushort ToUInt16(byte[] buffer, int startIndex)
        {
            fixed (byte* p = &(buffer[startIndex]))
            {
                return *(((ushort*)p));
            }
        }

        /// <summary>
        /// Returns a 16-bit signed integer converted from two bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="buffer">The array of bytes.</param>
        /// <param name="startIndex">The starting position within the byte array.</param>
        /// <returns>A 16-bit signed integer formed by two bytes beginning at startIndex.</returns>
        public static unsafe short ToInt16(byte[] buffer, int startIndex)
        {
            fixed (byte* p = &(buffer[startIndex]))
            {
                return *(((short*)p));
            }
        }

        /// <summary>
        /// Returns a 64-bit signed integer converted from eight bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="buffer">The array of bytes.</param>
        /// <param name="startIndex">The starting position within the byte array.</param>
        /// <returns>A 64-bit signed integer formed by eight bytes beginning at startIndex.</returns>
        public static unsafe Int64 ToInt64(byte[] buffer, int startIndex)
        {
            fixed (byte* p = &(buffer[startIndex]))
            {
                return *(((long*)p));
            }
        }

        /// <summary>
        /// Returns the specified decimal value as an array of bytes.
        /// </summary>
        /// <param name="value">The decimal number to convert.</param>
        /// <returns>An array of bytes with length 16.</returns>
        public static byte[] GetBytes(decimal value)
        {
            byte[] bytes = new byte[16];
            int[] int_array = Decimal.GetBits(value);
            for (int i = 0; i < 4; i++)
            {
                Memory.Copy(BitConverter.GetBytes(int_array[i]), 0, bytes, i << 2, 4);
            }
            return bytes;
        }

        /// <summary>
        /// Writes the specified decimal value to the specified buffer starting from the specified offset.
        /// </summary>
        /// <param name="value">The decimal value.</param>
        /// <param name="buffer">The memory buffer.</param>
        /// <param name="offset">The zero-based byte offset into the buffer; the offset is added 16 bytes after the writing.</param>
        public static void WriteDecimal(decimal value, byte[] buffer, ref int offset)
        {
            int[] int_array = Decimal.GetBits(value);
            for (int i = 0; i < 4; i++)
            {
                Memory.Copy(BitConverter.GetBytes(int_array[i]), 0, buffer, offset, 4);
                offset += 4;
            }
        }

        /// <summary>
        /// Returns a decimal value converted from 16 bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="buffer">An array of bytes.</param>
        /// <param name="startIndex">The starting position within the buffer.</param>
        /// <returns>A decimal value.</returns>
        public static decimal GetDecimal(byte[] buffer, int startIndex)
        {
            int[] int_array = new int[4];
            for (int i = 0; i < 4; i++)
            {
                int_array[i] = BitHelper.ToInt32(buffer, startIndex + (i << 2));
            }
            return new decimal(int_array);
        }

        internal static uint CountOneBits(uint* array, int size)
        {
            int i, j, lim;
            uint s8, x;

            uint oneBitCount = 0;
            for (i = 0; i < size; i = i + 31)
            {
                lim = Math.Min(size, i + 31);
                s8 = 0;
                for (j = i; j < lim; j++)
                {
                    x = array[j];
                    x = x - ((x >> 1) & 0x55555555);
                    x = (x & 0x33333333) + ((x >> 2) & 0x33333333);
                    x = (x + (x >> 4)) & 0x0F0F0F0F;
                    s8 = s8 + x;
                }
                x = (s8 & 0x00FF00FF) + ((s8 >> 8) & 0x00FF00FF);
                x = (x & 0x0000ffff) + (x >> 16);
                oneBitCount = oneBitCount + x;
            }
            return oneBitCount;
        }

        /// <summary>
        /// Returns the Hamming distance of the two specified ulong array buffer.
        /// </summary>
        /// <param name="arrayA">The first ulong array buffer.</param>
        /// <param name="arrayB">The second ulong array buffer.</param>
        /// <param name="size">The number of ulong elements to compare.</param>
        /// <returns>The hamming distance between the two ulong array.</returns>
        public static uint HammingDistance(ulong* arrayA, ulong* arrayB, int size)
        {
            ulong[] buffer = new ulong[size];
            fixed (ulong* lp = buffer)
            {
                for (int i = 0; i < size; i++)
                {
                    lp[i] = arrayA[i] ^ arrayB[i];
                }
                uint* ip = (uint*)lp;
                return CountOneBits(ip, size << 1);
            }
        }
    }
}
