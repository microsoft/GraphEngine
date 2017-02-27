// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace Trinity.Win32
{
    internal class NativeAPI
    {
        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr CreateMutex(IntPtr lpMutexAttributes, bool bInitialOwner, string lpName);
        #region Fine-grained sleep functions
        [DllImport("winmm.dll")]
        public static extern uint timeBeginPeriod(uint period);
        [DllImport("winmm.dll")]
        public static extern uint timeEndPeriod(uint period);
        #endregion
        #region Memory usage query methods and structures
        [StructLayout(LayoutKind.Sequential, Size = 40)]
        internal struct PROCESS_MEMORY_COUNTERS
        {

            public uint cb;
            public uint PageFaultCount;
            public IntPtr PeakWorkingSetSize;
            public IntPtr WorkingSetSize;
            public IntPtr QuotaPeakPagedPoolUsage;
            public IntPtr QuotaPagedPoolUsage;
            public IntPtr QuotaPeakNonPagedPoolUsage;
            public IntPtr QuotaNonPagedPoolUsage;
            public IntPtr PagefileUsage;
            public IntPtr PeakPagefileUsage;

        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MEMORYSTATUSEX
        {
            public int dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern internal bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

        [DllImport("psapi.dll", SetLastError = true)]
        static extern internal bool GetProcessMemoryInfo(IntPtr hProcess, out PROCESS_MEMORY_COUNTERS counters, long size);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentProcess();
        [DllImport("kernel32.dll")]
        public static extern bool QueryPerformanceCounter(out long value);
        #endregion
    }
}
