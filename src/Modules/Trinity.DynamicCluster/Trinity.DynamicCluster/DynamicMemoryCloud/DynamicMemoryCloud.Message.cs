// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using Trinity.Network.Messaging;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Storage
{
    public partial class DynamicMemoryCloud
    {
        public override unsafe void SendMessageToServer(int serverId, byte* buffer, int size)
        {
            base.SendMessageToServer(serverId, buffer, size);
        }

        public override unsafe void SendMessageToServer(int serverId, byte* buffer, int size, out TrinityResponse response)
        {
            base.SendMessageToServer(serverId, buffer, size, out response);
        }

        public override unsafe void SendMessageToServer(int serverId, byte** buffers, int* sizes, int count)
        {
            base.SendMessageToServer(serverId, buffers, sizes, count);
        }

        public override unsafe void SendMessageToServer(int serverId, byte** buffers, int* sizes, int count, out TrinityResponse response)
        {
            base.SendMessageToServer(serverId, buffers, sizes, count, out response);
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
        public override unsafe void SendMessageToProxy(int proxyId, byte* buffer, int size)
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
        public override unsafe void SendMessageToProxy(int proxyId, byte* buffer, int size, out TrinityResponse response)
        {
            throw new NotImplementedException();
        }

        public override unsafe void SendMessageToProxy(int proxyId, byte** buffers, int* sizes, int count)
        {
            throw new NotImplementedException();
        }

        public override unsafe void SendMessageToProxy(int proxyId, byte** buffers, int* sizes, int count, out TrinityResponse response)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
