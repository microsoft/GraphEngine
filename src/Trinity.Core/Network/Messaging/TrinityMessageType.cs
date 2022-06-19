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
    /**! Note, we assign SYNC, SYNC_WITH_RSP, ASYNC, ASYNC_WITH_RSP to first four values,
     *   So that in a comm module we use the type as an offset to locate the offset slot.
     */
    public enum TrinityMessageType : ushort
    {
        /// <summary>
        /// Synchronous message.
        /// </summary>
        SYNC = 0,
        /// <summary>
        /// Synchronous message with response.
        /// </summary>
        SYNC_WITH_RSP = 1,
        /// <summary>
        /// Asynchronous message.
        /// </summary>
        ASYNC = 2,
        /// <summary>
        /// Asynchronous message with response.
        /// </summary>
        ASYNC_WITH_RSP = 3,
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
        /// <summary>
        /// Upper bound message type -- do not use
        /// </summary>
        MESSAGE_TYPE_MAX,
    }
}
