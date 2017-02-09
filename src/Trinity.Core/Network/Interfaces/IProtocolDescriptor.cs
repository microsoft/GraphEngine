// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Network.Messaging;

namespace Trinity.Network
{
    /// <summary>
    /// Provides metadata associated with a protocol defined in TSL.
    /// </summary>
    public interface IProtocolDescriptor
    {
        /// <summary>
        /// The name of the protocol.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// The request signature.
        /// </summary>
        string RequestSignature { get; }
        /// <summary>
        /// The response signature.
        /// </summary>
        string ResponseSignature { get; }
        /// <summary>
        /// The type of the protocol.
        /// </summary>
        TrinityMessageType Type { get; }
    }
}
