// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Network.Messaging
{
    /// <summary>
    /// Represents a collection of network message types. 
    /// </summary>
    public enum TrinityMessageType : byte
    {
        /// <summary>
        /// Synchronous message.
        /// </summary>
        SYNC,
        /// <summary>
        /// Synchronous message with response.
        /// </summary>
        SYNC_WITH_RSP,
        /// <summary>
        /// Asynchronous message.
        /// </summary>
        ASYNC,
        /// <summary>
        /// Preserved synchronous message.
        /// </summary>
        PRESERVED_SYNC,
        /// <summary>
        /// Preserved synchronous message with response.
        /// </summary>
        PRESERVED_SYNC_WITH_RSP,
        /// <summary>
        /// Preserved asynchronous message.
        /// </summary>
        PRESERVED_ASYNC,
    }
}
