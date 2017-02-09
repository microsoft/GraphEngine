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

namespace Trinity.Core.Lib
{
    class CTrinityC
    {
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void TestEntry();

        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int GetLastError();
    }
}
