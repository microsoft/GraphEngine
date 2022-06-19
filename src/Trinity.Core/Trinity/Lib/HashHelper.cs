// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !CORECLR
using System.Runtime.ConstrainedExecution;
#endif
using System.Security;
using System.Security.Cryptography;

namespace Trinity.Core.Lib
{
    /// <summary>
    /// A helper class for various hash functions.
    /// </summary>
    public unsafe static class HashHelper
    {
        // Fields
        internal static readonly int[] primes = new int[] { 
        3, 7, 11, 0x11, 0x17, 0x1d, 0x25, 0x2f, 0x3b, 0x47, 0x59, 0x6b, 0x83, 0xa3, 0xc5, 0xef, 
        0x125, 0x161, 0x1af, 0x209, 0x277, 0x2f9, 0x397, 0x44f, 0x52f, 0x63d, 0x78b, 0x91d, 0xaf1, 0xd2b, 0xfd1, 0x12fd, 
        0x16cf, 0x1b65, 0x20e3, 0x2777, 0x2f6f, 0x38ff, 0x446f, 0x521f, 0x628d, 0x7655, 0x8e01, 0xaa6b, 0xcc89, 0xf583, 0x126a7, 0x1619b, 
        0x1a857, 0x1fd3b, 0x26315, 0x2dd67, 0x3701b, 0x42023, 0x4f361, 0x5f0ed, 0x72125, 0x88e31, 0xa443b, 0xc51eb, 0xec8c1, 0x11bdbf, 0x154a3f, 0x198c4f, 
        0x1ea867, 0x24ca19, 0x2c25c1, 0x34fa1b, 0x3f928f, 0x4c4987, 0x5b8b6f, 0x6dda89
     };

        internal static void InitDHT()
        {
        }

        internal static int GetServerIdByCellId(long cell_id)
        {
            return -1;
        }

        // Methods
        internal static int GetMinPrime()
        {
            return 3;
        }

#if !CORECLR
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
#endif
        internal static int GetPrime(int min)
        {
            if (min < 0)
            {
                throw new ArgumentException("Parameter cannot be less zero.");
            }
            for (int i = 0; i < primes.Length; i++)
            {
                int num2 = primes[i];
                if (num2 >= min)
                {
                    return num2;
                }
            }
            for (int j = min | 1; j < 0x7fffffff; j += 2)
            {
                if (IsPrime(j))
                {
                    return j;
                }
            }
            return min;
        }

        internal static int GetPrimeNoLargerThan(int max)
        {
            int value = max;
            int prime = 0;
            do
            {
                prime = GetPrime(value);
                value--;
            } while (prime > max);
            return prime;
        }


        internal static bool IsPrime(int candidate)
        {
            if ((candidate & 1) == 0)
            {
                return (candidate == 2);
            }
            int num = (int)Math.Sqrt((double)candidate);
            for (int i = 3; i <= num; i += 2)
            {
                if ((candidate % i) == 0)
                {
                    return false;
                }
            }
            return true;
        }

        internal unsafe static long HashString2Int64Strong(string s)
        {
            if (s != null && s.Length != 0)
            {
                byte[] bytes = BitHelper.GetBytes(s);
#if CORECLR
                SHA256 hash = SHA256.Create();
#else
                SHA256 hash = new SHA256CryptoServiceProvider();
#endif
                byte[] hashBytes = hash.ComputeHash(bytes);
                return BitHelper.ToInt64(hashBytes, 0) ^ BitHelper.ToInt64(hashBytes, 8) ^ BitHelper.ToInt64(hashBytes, 24);
            }
            return 0;
        }

        /// <summary>
        /// Gets a 64-bit signed integer hash code of the specified string.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns>A 64-bit signed integer hash code.</returns>
        public unsafe static long HashString2Int64(string value)
        {
            byte[] bytes = BitHelper.GetBytes(value);
            long hash;
            uint* p = (uint*)&hash;
            *p = HashBytesA(bytes);
            *(p + 1) = HashBytesB(bytes);
            return hash;
        }

        internal unsafe static long HashAsciiString2Int64(string value)
        {
            byte[] bytes = BitHelper.GetAsciiCharArray(value);
            long hash;
            uint* p = (uint*)&hash;
            *p = HashBytesA(bytes);
            *(p + 1) = HashBytesB(bytes);
            return hash;
        }

        /// <summary>
        /// Gets the hash code of the byte array.
        /// </summary>
        /// <param name="buffer">The array of bytes.</param>
        /// <returns>The hash code.</returns>
        public static int HashBytes(byte[] buffer)
        {
            return (int)HashBytesA(buffer);
        }

        /// <summary>
        /// Gets the hash code of the memory buffer.
        /// </summary>
        /// <param name="buffer">The memory buffer.</param>
        /// <param name="count">The number of bytes to access.</param>
        /// <returns>The hash code.</returns>
        public static int HashBytes(byte* buffer, int count)
        {
            return (int)HashBytesA(buffer, count);
        }

        internal unsafe static uint HashBytesA(byte[] key)
        {
            byte* e;
            byte* k;
            uint h;

            fixed (byte* kp = key)
            {
                k = kp;
                e = k + key.Length;
                for (h = 0; k < e; ++k)
                {
                    h *= 16777619;
                    h ^= *k;
                }
            }
            return h;
        }


        // FNV-1
        // see https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
        internal unsafe static uint HashBytesA(byte* kp, int count)
        {
            uint h;

            byte* k = kp;
            byte* e = k + count;
            for (h = 0; k < e; ++k)
            {
                h *= 16777619;
                h ^= *k;
            }
            return h;
        }

        // Phong's linear congruential hash
        // see https://en.wikipedia.org/wiki/Linear_congruential_generator
        internal unsafe static uint HashBytesB(byte[] key)
        {
            byte* e;
            byte* k;
            byte c;
            uint h;

            fixed (byte* kp = key)
            {
                k = kp;
                e = k + key.Length;
                for (h = 0; k != e; )
                {
                    c = *k++;
                    if (c == 0 && k > e)
                        break;
                    h = 0x63c63cd9 * h + 0x9c39c33d + c;
                }
            }
            return h;
        }

        internal unsafe static uint HashBytesB(byte* kp, int count)
        {
            byte c;
            uint h;

            byte* k = kp;
            byte* e = k + count;
            for (h = 0; k != e; )
            {
                c = *k++;
                if (c == 0 && k > e)
                    break;
                h = 0x63c63cd9 * h + 0x9c39c33d + c;
            }
            return h;
        }
    }
}
