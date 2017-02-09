// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;

using Trinity;
using Trinity.Network;
using Trinity.Network.Messaging;

namespace Trinity.Network.Messaging
{
    /// <summary>
    /// Specifies the length and offset constants for the binary network message header. 
    /// </summary>
    public static class TrinityProtocol
    {
        /// <summary>
        /// value = 4
        /// </summary>
        public const int SocketMsgHeader = 4;

        /// <summary>
        /// value = 4
        /// </summary>
        public const int TrinityMsgHeader = 4;

        /// <summary>
        /// value = 0
        /// </summary>
        public const int TrinityMsgTypeOffset = 0;

        /// <summary>
        /// value = 2
        /// </summary>
        public const int TrinityMsgIdOffset = 2;

        /// <summary>
        /// value = 8
        /// </summary>
        public const int MsgHeader = 8;

        /// <summary>
        /// value = 4
        /// </summary>
        public const int MsgTypeOffset = 4;

        /// <summary>
        /// value = 6
        /// </summary>
        public const int MsgIdOffset = 6;
    }
}
