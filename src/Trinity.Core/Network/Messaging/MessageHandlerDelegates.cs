// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System.Threading.Tasks;

namespace Trinity.Network.Messaging
{
    /// <summary>
    /// Represents a message handler for a synchronous protocol without response.
    /// </summary>
    /// <param name="args">A synchronous request object.</param>
    public delegate Task SynReqHandler(SynReqArgs args);

    /// <summary>
    /// Represents a message handler for a synchronous protocol with response.
    /// </summary>
    /// <param name="args">A synchronous request object.</param>
    public delegate Task SynReqRspHandler(SynReqRspArgs args);

    /// <summary>
    /// Represents a message handler for an asynchronous protocol without response.
    /// </summary>
    /// <param name="args">An asynchronous request object.</param>
    public delegate Task AsyncReqHandler(AsynReqArgs args);

    /// <summary>
    /// Represents a message handler for a asynchronous protocol with response.
    /// </summary>
    /// <param name="args">A synchronous request object.</param>
    public delegate Task AsyncReqRspHandler(AsynReqRspArgs args);

}
