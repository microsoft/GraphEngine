// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Trinity
{
    /// <summary>
    /// Provides native file IO interfaces.
    /// </summary>
    internal unsafe class Stdio
    {
        static Stdio()
        {
            TrinityC.Init();
        }

        internal static readonly int EOF = -1;

        internal static errno_t _wfopen_s(out void* fp, string path, string mode)
        {
            fixed (char* ppath = path)
            fixed (char* pmode = mode)
            {
                return CStdio.C_wfopen_s(out fp, ppath, pmode);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong fread(void* buffer, ulong elementSize, ulong count, void* fp) { return CStdio.C_fread(buffer, elementSize, count, fp); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong fwrite(void* buffer, ulong elementSize, ulong count, void* fp) { return CStdio.C_fwrite(buffer, elementSize, count, fp); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int fflush(void* fp) { return CStdio.C_fflush(fp); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int fclose(void* fp) { return CStdio.C_fclose(fp); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int feof(void* fp) { return CStdio.C_feof(fp); }
    }

    internal unsafe class CStdio
    {
        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern errno_t C_wfopen_s(out void* fp, char* path, char* mode);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern ulong C_fread(void* buffer, ulong elementSize, ulong count, void* fp);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern ulong C_fwrite(void* buffer, ulong elementSize, ulong count, void* fp);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern int C_fflush(void* fp);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern int C_fclose(void* fp);

        [DllImport(TrinityC.AssemblyName)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern int C_feof(void* fp);
    }
}
