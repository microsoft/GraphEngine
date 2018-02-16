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
    /// Represents a message handler for a synchronous protocol without response.
    /// </summary>
    /// <param name="args">A synchronous request object.</param>
    public delegate void SynReqHandler(SynReqArgs args);

    /// <summary>
    /// Represents a message handler for a synchronous protocol with response.
    /// </summary>
    /// <param name="args">A synchronous request object.</param>
    public delegate void SynReqRspHandler(SynReqRspArgs args);

    /// <summary>
    /// Represents a message handler for an asynchronous protocol without response.
    /// </summary>
    /// <param name="args">An asynchronous request object.</param>
    public delegate void AsyncReqHandler(AsynReqArgs args);

    /// <summary>
    /// Represents a message handler for a asynchronous protocol with response.
    /// </summary>
    /// <param name="args">A synchronous request object.</param>
    public delegate void AsyncReqRspHandler(AsynReqRspArgs args);

}
