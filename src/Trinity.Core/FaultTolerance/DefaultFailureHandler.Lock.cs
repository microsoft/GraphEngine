// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity.Diagnostics;
using System.Net;
using Trinity.Network.Messaging;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using System.Runtime;

namespace Trinity.FaultTolerance
{
#pragma warning disable 0420
    static partial class DefaultFailureHandler
    {
        private static volatile int spinlock = 0;

        private static bool TryGetLock()
        {
            if (Interlocked.CompareExchange(ref spinlock, 1, 0) == 0)
                return true;
            else
                return false;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        private static void UnLock()
        {
            spinlock = 0;
        }
    }
}
