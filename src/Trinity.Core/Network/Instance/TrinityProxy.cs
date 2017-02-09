// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;

using Trinity;
using Trinity.Network;
using Trinity.Network.Sockets;
using Trinity.Diagnostics;

namespace Trinity.Network
{
    /// <summary>
    /// Represents a stock Trinity proxy.
    /// </summary>
    public class TrinityProxy : CommunicationInstance
    {
        internal sealed override RunningMode RunningMode
        {
            get { return Trinity.RunningMode.Proxy; }
        }
    }
}
