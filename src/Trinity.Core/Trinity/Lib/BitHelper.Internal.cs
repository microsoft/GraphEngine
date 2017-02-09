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
    public unsafe partial class BitHelper
    {
        internal static byte[] GetBytes(List<long> longList)
        {
            int size = 4 + longList.Count * 8;//the first 4 bytes as a int to save length
            byte[] buffer = new byte[size];
            fixed (byte* numRef = buffer)
            {
                byte* ptr = numRef;
                *((int*)(ptr)) = longList.Count;
                ptr += 4;
                for (int k = 0; k < longList.Count; ++k)
                {
                    *((long*)(ptr)) = longList[k];
                    ptr += 8;
                }
            }
            return buffer;
        }

        internal static List<long> GetLongList(byte[] buffer)
        {
            List<long> ret = null;
            fixed (byte* numRef = buffer)
            {
                byte* ptr = numRef;
                int length = *((int*)ptr);
                ptr += 4;
                ret = new List<long>(length);

                for (int k = 0; k < length; k++)
                {
                    ret.Add(*((long*)(ptr)));
                    ptr += 8;
                }
            }
            return ret;
        }

        internal static void WriteString(string str, MemoryStream ms)
        {
            byte[] str_bytes = GetBytes(str);
            int str_length = str_bytes.Length;
            ms.Write(BitConverter.GetBytes(str_length), 0, 4);
            ms.Write(str_bytes, 0, str_length);
        }

        internal static string ReadString(MemoryStream ms)
        {
            byte[] int_bytes = new byte[4];
            ms.Read(int_bytes, 0, 4);
            int str_size = BitHelper.ToInt32(int_bytes, 0);
            byte[] str_buff = new byte[str_size];
            ms.Read(str_buff, 0, str_size);
            return GetString(str_buff);
        }

        internal static string ReadString(Byte[] buffer, int offset)
        {
            int str_size = BitHelper.ToInt32(buffer, offset);
            offset += 4;
            return GetString(buffer, offset, str_size);
        }

        internal static string ReadString(Byte* buffer, int offset)
        {
            int str_size = *(int*)(buffer + offset);
            offset += 4;
            return GetString(buffer + offset, str_size);
        }
    }
}
