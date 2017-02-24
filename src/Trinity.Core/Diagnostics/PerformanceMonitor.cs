// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

using Trinity;
using Trinity.Win32;
using System.Globalization;

namespace Trinity.Diagnostics
{
    internal class PerformanceMonitor
    {
#if !CORECLR
        static PerformanceCounter TotalCPUCounter = new PerformanceCounter( "Processor", "% Processor Time", "_Total" );
        static PerformanceCounter TotalRAMCounter = new PerformanceCounter( "Memory", "Committed Bytes" );

        static PerformanceCounter TotalAvailableRAMCounter = new PerformanceCounter("Memory", "Available Bytes");

        static PerformanceCounter TrinityCPUCounter = new PerformanceCounter( "Process", "% Processor Time", Process.GetCurrentProcess().ProcessName );

        static PerformanceCounter TrinityRAMCounter = new PerformanceCounter( "Process", "Private Bytes", Process.GetCurrentProcess().ProcessName );

        public static int GetOptimalThreadCount()
        {
            return Environment.ProcessorCount + 4;
        }

        public static string GetTotalCpuUsage()
        {
            return TotalCPUCounter.NextValue().ToString( "00.00", CultureInfo.InvariantCulture ) + "%";
        }

        public static string GetTotalRAMUsage()
        {
            return ( TotalRAMCounter.NextValue() / 1024 ).ToString( "000,000,000", CultureInfo.InvariantCulture ) + "KB";
        }

        public static long AvailableMemoryBytes
        {
            get
            {
                return (long)TotalAvailableRAMCounter.NextValue();
            }
        }

        public static string GetTrinityCpuUsage()
        {
            return ( TrinityCPUCounter.NextValue() / Environment.ProcessorCount ).ToString( "00.00", CultureInfo.InvariantCulture ) + "%";
        }

        public static string GetTrinityRamUsage()
        {
            return ( TrinityRAMCounter.NextValue() / 1024 ).ToString( "000,000,000" , CultureInfo.InvariantCulture) + "KB";
        }

        public static unsafe long GetMemoryWorkingSet()
        {
            Trinity.Win32.NativeAPI.PROCESS_MEMORY_COUNTERS counter = new Trinity.Win32.NativeAPI.PROCESS_MEMORY_COUNTERS();
            NativeAPI.GetProcessMemoryInfo(NativeAPI.GetCurrentProcess(), out counter, sizeof(Trinity.Win32.NativeAPI.PROCESS_MEMORY_COUNTERS));
            return counter.WorkingSetSize.ToInt64();
        }
#else
        public static int GetOptimalThreadCount()
        {
            return Environment.ProcessorCount + 4;
        }

        public static string GetTotalCpuUsage()
        {
            throw new NotImplementedException();
        }

        public static string GetTotalRAMUsage()
        {
            throw new NotImplementedException();
        }

        public static long AvailableMemoryBytes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public static string GetTrinityCpuUsage()
        {
            throw new NotImplementedException();
        }

        public static string GetTrinityRamUsage()
        {
            throw new NotImplementedException();
        }

        public static unsafe long GetMemoryWorkingSet()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Trinity.Win32.NativeAPI.PROCESS_MEMORY_COUNTERS counter = new Trinity.Win32.NativeAPI.PROCESS_MEMORY_COUNTERS();
                NativeAPI.GetProcessMemoryInfo(NativeAPI.GetCurrentProcess(), out counter, sizeof(Trinity.Win32.NativeAPI.PROCESS_MEMORY_COUNTERS));
                return counter.WorkingSetSize.ToInt64();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
#endif
        public static void PrintServerInfo()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("*****************************************************");
            Console.WriteLine();
            Console.WriteLine("server machine name = " + Environment.MachineName);
            Console.WriteLine("StorageRoot: " + Trinity.TrinityConfig.StorageRoot);
            Console.WriteLine("Server Configuration Parameters:");
            Console.WriteLine("Server is started at: {0}", DateTime.Now);
            Console.WriteLine();
            Console.WriteLine("*****************************************************");
            Console.WriteLine();
        }

        public static void PerformanceMonitorThread()
        {
            Thread.Sleep(6000);
            while (true)
            {
                Thread.Sleep(1000);
                Console.SetCursorPosition(0, 18);
                Console.WriteLine("\r\n\r\n");
                Console.SetCursorPosition(0, 20);
                Console.WriteLine("TotalCPU \t TotalMem \t TrinityCPU \t TrinityMem \r");
                Console.SetCursorPosition(0, 21);
                Console.Write(PerformanceMonitor.GetTotalCpuUsage() + " \t\t ");
                Console.Write(PerformanceMonitor.GetTotalRAMUsage() + " \t ");
                Console.Write(PerformanceMonitor.GetTrinityCpuUsage() + " \t ");
                Console.Write(PerformanceMonitor.GetTrinityRamUsage() + "\r");
                Console.SetCursorPosition(0, 22);
            }
        }

        public delegate void MeasureFunction();
        static public void Measure(MeasureFunction mf, string name, bool echo = true)
        {
            if (echo)
            {
                Stopwatch sw = Stopwatch.StartNew();
                mf();
                sw.Stop();
                Console.WriteLine(name + "; Time = " + sw.ElapsedMilliseconds);
            }
            else
            {
                mf();
            }
        }
    }
}
