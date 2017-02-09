// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Trinity.Diagnostics;

namespace Trinity
{
    internal unsafe static class CTrinityConfig
    {
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int GetStorageCapacityProfile();

        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetStorageCapacityProfile(int capacityProfile);

        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetStorageRoot(byte* buff, int length);
        
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool CReadOnly();
        
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void CSetReadOnly(bool value);
        
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int CTrunkCount();
        
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int CSetTrunkCount(int value);
        
        
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int CLargeObjectThreshold();
        

        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void CSetLargeObjectThreshold(int value);
        

        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void CSetGCDefragInterval(int value);
        
    }
}
