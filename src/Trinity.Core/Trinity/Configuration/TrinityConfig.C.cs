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
using Trinity.Diagnostics;

namespace Trinity
{
    internal unsafe static class CTrinityConfig
    {
#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern int GetStorageCapacityProfile();

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern void SetStorageCapacityProfile(int capacityProfile);

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern void SetStorageRoot(byte* buff, int length);

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern bool CReadOnly();

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern void CSetReadOnly(bool value);

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern int CTrunkCount();

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern int CSetTrunkCount(int value);


#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern int CLargeObjectThreshold();


#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern void CSetLargeObjectThreshold(int value);


#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern void CSetGCDefragInterval(int value);

        [DllImport(TrinityC.AssemblyName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        internal static extern unsafe void CLogInitializeLogger(string logDir);

        [DllImport(TrinityC.AssemblyName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        internal static extern void CLogSetLogLevel(LogLevel level);

        [DllImport(TrinityC.AssemblyName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        internal static extern void CLogSetEchoOnConsole(bool is_set);

        [DllImport(TrinityC.AssemblyName)]
        internal static extern bool CHandshake();
        [DllImport(TrinityC.AssemblyName)]
        internal static extern void CSetHandshake(bool handshake);
        [DllImport(TrinityC.AssemblyName)]
        internal static extern bool CClientDisableSendBuffer();
        [DllImport(TrinityC.AssemblyName)]
        internal static extern void CSetClientDisableSendBuffer(bool disable_sndbuffer);
    }
}
