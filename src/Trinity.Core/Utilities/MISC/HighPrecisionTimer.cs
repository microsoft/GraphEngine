// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;

namespace Trinity.Utilities
{
    class HighPrecisionTimer
    {
         [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(
            out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(
            out long lpFrequency);

        private long startTime, stopTime;
        private long freq;

        public HighPrecisionTimer()
        {
            startTime = 0;
            stopTime  = 0;

            if (QueryPerformanceFrequency(out freq) == false)
            {
                throw new Win32Exception();
            }
        }

        public void Start()
        {
            Thread.Sleep(0);

            QueryPerformanceCounter(out startTime);
        }

        // stop
        public void Stop()
        {
            QueryPerformanceCounter(out stopTime);
        }

        //return seconds
        public double Duration
        {
            get
            {
                return (double)(stopTime - startTime) / (double) freq;
            }
        }
    }
}
