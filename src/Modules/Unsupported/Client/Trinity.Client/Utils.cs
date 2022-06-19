using System;
using System.Collections.Generic;
using System.Text;
using Trinity.Core.Lib;

namespace Trinity.Client
{
    internal static class Utils
    {
        internal static unsafe void _copy(byte* p, byte** message, int* sizes, int count)
        {
            int* pend = sizes + count;
            int size;
            while (sizes != pend)
            {
                size = *sizes;
                Memory.memcpy(p, *message, (ulong)size);
                ++sizes;
                ++message;
                p += size;
            }
        }

        internal static unsafe int _sum(int* sizes, int count)
        {
            int size = 0;
            int* pend = sizes + count;
            while (sizes != pend)
            {
                size += *sizes;
                ++sizes;
            }
            return size;
        }
    }
}
