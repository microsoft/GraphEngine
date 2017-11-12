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
using System.Threading.Tasks;
using System.Diagnostics;

using Trinity;
using Trinity.Core.Lib;
using Trinity.Network.Messaging;
using Trinity.Network;
using System.Globalization;
using Trinity.DynamicCluster;

namespace Trinity.Storage
{
    public unsafe partial class DynamicMemoryCloud
    {
        public override unsafe void SendMessageToServer(int serverId, byte* buffer, int size)
        {
            if (serverId >= 0)
                base.SendMessageToServer(serverId, buffer, size);
            else
                temporaryRemoteStorageRepo[serverId].SendMessage(buffer, size);
        }

        public override unsafe void SendMessageToServer(int serverId, byte* buffer, int size, out TrinityResponse response)
        {
            if (serverId >= 0)
                base.SendMessageToServer(serverId, buffer, size, out response);
            else
                temporaryRemoteStorageRepo[serverId].SendMessage(buffer, size, out response);
        }

        #region Proxies
        /// <summary>
        /// Gets a list of Trinity proxy.
        /// </summary>
        public override IList<RemoteStorage> ProxyList
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Send a binary message to the specified Trinity proxy.
        /// </summary>
        /// <param name="proxyId">A 32-bit proxy id.</param>
        /// <param name="buffer">A binary message buffer.</param>
        /// <param name="size">The size of the message.</param>
        public override void SendMessageToProxy(int proxyId, byte* buffer, int size)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Send a binary message to the specified Trinity proxy.
        /// </summary>
        /// <param name="proxyId">A 32-bit proxy id.</param>
        /// <param name="buffer">A binary message buffer.</param>
        /// <param name="size">The size of the message.</param>
        /// <param name="response">The TrinityResponse object returned by the Trinity proxy.</param>
        public override void SendMessageToProxy(int proxyId, byte* buffer, int size, out TrinityResponse response)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
