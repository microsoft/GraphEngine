using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Trinity
{
    [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit)]
    public unsafe struct PointerHelper
    {
        static public unsafe PointerHelper New(byte* ptr)
        {
            PointerHelper ret = new PointerHelper();
            ret.bp = ptr;
            return ret;
        }
        static public unsafe PointerHelper New(short* ptr)
        {
            PointerHelper ret = new PointerHelper();
            ret.sp = ptr;
            return ret;
        }
        static public unsafe PointerHelper New(int* ptr)
        {
            PointerHelper ret = new PointerHelper();
            ret.ip = ptr;
            return ret;
        }
        static public unsafe PointerHelper New(long* ptr)
        {
            PointerHelper ret = new PointerHelper();
            ret.lp = ptr;
            return ret;
        }
        [System.Runtime.InteropServices.FieldOffset(0)]
        public byte* bp;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public short* sp;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public int* ip;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public long* lp;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public double* dp;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public float* fp;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public char* cp;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe byte GetByte(byte** buffers, int* sizes, int offset)
        {
            while (offset >= *sizes)
            {
                offset -= *sizes;
                ++buffers;
                ++sizes;
            }
            return buffers[0][offset];
        }

        /// <summary>
        /// Caller guarantees that the ushort target value does not span across two buffers.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Add(byte** buffers, int* sizes, int offset, ushort value)
        {
            while (offset >= *sizes)
            {
                offset -= *sizes;
                ++buffers;
                ++sizes;
            }
            *(ushort*)(buffers[0] + offset) += value;
        }
    }

}
