// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Trinity.Diagnostics;

namespace Trinity.FaultTolerance
{
    /// <summary>
    /// This registry is available for each process. 
    /// </summary>
    static class FailureHandlerRegistry
    {
        static FailureHandlerRegistry()
        {
            FailureHandlers = new List<MachineFailureHandler>();
            ConnectionFailureHandlers = new List<ConnectionFailureHandler>();
        }

        internal static List<MachineFailureHandler> FailureHandlers;
        internal static List<ConnectionFailureHandler> ConnectionFailureHandlers;

        internal static void RegisterMachineFailureHandler(MachineFailureHandler handler)
        {
            FailureHandlers.Add(handler);
        }

        internal static void RegisterConnectionFailureHandler(ConnectionFailureHandler handler)
        {
            ConnectionFailureHandlers.Add(handler);
        }

        internal static void MachineFailover(IPEndPoint failureMachineIPE)
        {
            //Log.WriteLine(LogLevel.Info, "The connection to {0} is unexpectedly closed.", failureMachineIPE.ToString());

            //LeaderElection.Elect(failureMachineIPE);

            //for (int i = 0; i < FailureHandlers.Count; i++)
            //{
            //    FailureHandlers[i](failureMachineIPE);
            //}
        }

        internal static void ConnectionFailureHandling(IPEndPoint connectionIPE)
        {
            //Log.WriteLine(LogLevel.Info, "The connection to {0} is temporarily dropped.", connectionIPE.ToString());
            //for (int i = 0; i < ConnectionFailureHandlers.Count; i++)
            //{
            //    ConnectionFailureHandlers[i](connectionIPE);
            //}
        }
    }
}
