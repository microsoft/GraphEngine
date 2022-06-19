// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
#if !CORECLR
using System.Runtime.ConstrainedExecution;
#endif
using System.Threading;
using System.Runtime;

namespace Trinity.Core.Lib
{
#pragma warning disable 0420
    class XRandom : Random
    {
#region spin lock
        private volatile int spinlock = 0;

        private void GetLock()
        {
            if (Interlocked.CompareExchange(ref spinlock, 1, 0) != 0)
            {
                while (true)
                {
                    if (spinlock == 0 && Interlocked.CompareExchange(ref spinlock, 1, 0) == 0)
                        return;
                    Thread.Sleep(0);
                }
            }
        }

#if !CORECLR
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
#endif
        private void UnLock()
        {
            spinlock = 0;
        }
#endregion

        public XRandom() : base() { }
        public XRandom(int seed) : base(seed) { }
        public byte NextByte()
        {
            return (byte)(this.Sample() * 256);
        }
        public unsafe long NextInt64()
        {
            byte[] bytes = new byte[8];
            GetLock();
            NextBytes(bytes);
            UnLock();
            fixed (byte* p = bytes)
            {
                return *(long*)p;
            }
        }

        public override double NextDouble()
        {
            GetLock();
            double value = Sample();
            UnLock();
            return value;
        }

        public override int Next(int minValue, int maxValue)
        {
            GetLock();
            int value = base.Next(minValue, maxValue);
            UnLock();
            return value;
        }

        public double NextDoubleUnsafe()
        {
            return Sample();
        }
    }

    /// <summary>
    /// A utility class for generating 64-bit cell ids.
    /// </summary>
    public unsafe static class CellIdFactory
    {
        static CellIdFactory()
        {
            rand = new XRandom(Environment.TickCount);
        }

        static XRandom rand;
        private static long sn = 0;

        /// <summary>
        /// Generates a new random 64-bit cell id.
        /// </summary>
        /// <returns>A new 64-bit cell id.</returns>
        /// <remarks>This is a thread-safe method that you can call to get a new cell id.</remarks>
        public static long NewCellId()
        {
            return rand.NextInt64();
        }

        /// <summary>
        /// Generates a sequentially incremented 64-bit cell id.
        /// </summary>
        /// <returns>A new 64-bit cell id.</returns>
        /// <remarks>This is a thread-safe method that you can call to get a new cell id.</remarks> 
        public static long NextSequentialCellId()
        {
            return Interlocked.Increment(ref sn);
        }

        internal static long Guid2CellId(Guid guid)
        {
            byte[] guidByteArray = guid.ToByteArray();
            guidByteArray[8] = guidByteArray[0];
            return BitHelper.ToInt64(guidByteArray, 8);
        }

        internal static long Guid2CellId(Guid guid, ushort cell_type)
        {
            byte[] guidByteArray = guid.ToByteArray();
            fixed (byte* p = guidByteArray)
            {
                *(p + 8) = *p;
                *(ushort*)(p + 14) = cell_type;
                return *(long*)(p + 8);
            }
        }

        internal static long MID2CellId(string mid, ushort cellType = 0)
        {
            long id = HashHelper.HashString2Int64(mid);
            ushort* sp = (ushort*)&id;
            *(sp + 2) = (ushort)(*(sp + 2) ^ *(sp + 3));
            *(sp + 3) = cellType;
            return id;
        }

        internal static ushort GetCellType(long cellId)
        {
            byte* p = (byte*)&cellId;
            return *(ushort*)(p + 6);
        }

        internal static long CellId2CellId(long cellId, ushort cell_type)
        {
            byte* p = (byte*)&cellId;
            *(ushort*)(p + 6) = cell_type;
            return cellId;
        }

        internal static ushort ExtractCellType(long cellId)
        {
            byte* p = (byte*)&cellId;
            return *(ushort*)(p + 6);
        }
    }
}
