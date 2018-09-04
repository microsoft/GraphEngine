// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Trinity.Utilities;

namespace Trinity
{
    internal static partial class TrinityC
    {
        internal const string AssemblyName = "Trinity";
        private static object s_initlock = new object();
        private static bool   s_initialized = false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe static void Init()
        {
            lock (s_initlock)
            {
                if (s_initialized) return;

                fixed (char* pAssemblyPath = AssemblyUtility.MyAssemblyPath)
                {
                    __INIT_TRINITY_C__(pAssemblyPath);
                }

                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    Win32.NativeAPI.timeBeginPeriod(1);
                }

                s_initialized = true;
            }
        }
        
        [DllImport(TrinityC.AssemblyName)]
        private static extern unsafe void __INIT_TRINITY_C__(char* pAssemblyPath);
    }
}
