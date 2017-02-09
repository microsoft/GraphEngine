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
using Trinity.Core.Lib;

namespace Trinity.FaultTolerance
{
    static partial class DefaultFailureHandler
    {
        internal unsafe static void FailureNotificationMessageHandler(ref SynReqRspArgs args)
        {
            int leaderCandidate = -1;
            int failureServerId = -1;

            {
                int* p = (int*)(args.Buffer + args.Offset);
                leaderCandidate = *p++;
                failureServerId = *p;
            }

            #region Debug
#if DEBUG
            Log.WriteLine(LogLevel.Info, "Received failure notification message from {0} who claim {1} failed.", leaderCandidate, failureServerId);
#endif
            #endregion

            TrinityMessage response = new TrinityMessage(34);

            {
                *(int*)response.Buffer = 30;
                long* p = (long*)(response.Buffer + TrinityMessage.Offset);
                var workload = SystemStatus.Workloads;
                *p++ = workload.AvailableMemoryBytes; // 8
                *p++ = workload.CellCount; // 8
                *p++ = workload.SliceSize; //8
                *(int*)p = workload.SliceCount; //4
            }

            args.Response = response;
        }
    }
}
