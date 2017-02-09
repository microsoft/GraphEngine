// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.Network.Messaging
{
    internal enum RequestType : ushort
    {
        #region Asynchronous messages
        Shutdown = 0,
        BulkMessage,
        #endregion

        #region Synchronous message without response
        P2PBarrier = 0,
        Heartbeat,
        ReportProxy,
        LoadStorage,
        SaveStorage,
        ResetStorage,
        #endregion

        #region Synchronous message with response
        SaveCell = 0,
        RemoveCell,
        Contains,
        GetCellType,

        AddCell,
        UpdateCell,
        LoadCell,
        LoadCellWithType,

        Sampling,
        RegisterServerModule,
        RegisterCellDescriptorModule,

        QueryMemoryWorkingSet,
        EchoPing,

        FailureNotification,
        GetCommunicationSchema,
        GetCommunicationModuleOffsets,
        #endregion
    }
}
