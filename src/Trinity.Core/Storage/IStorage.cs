// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Trinity;
using Trinity.Network.Messaging;
using Trinity.TSL.Lib;
using Trinity.Diagnostics;
namespace Trinity.Storage
{
    /// <summary>
    /// Represents an abstract storage class. It defines a set of cell accessing and manipulation interfaces.
    /// </summary>
    public unsafe interface IStorage : IKeyValueStore, IMessagePassingEndpoint, IDisposable
    {
    }
}
