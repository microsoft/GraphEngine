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

namespace Trinity.Storage
{
    public unsafe partial class MemoryCloud
    {
        #region Internal
        internal void SendMessageToServer(int serverId, TrinityMessage msg)
        {
            StorageTable[serverId].SendMessage(msg);
        }

        internal void SendMessageToServer(int serverId, TrinityMessage msg, out TrinityResponse response)
        {
            StorageTable[serverId].SendMessage(msg, out response);
        }

        internal void SendMessageToServer(int serverId, byte[] message, int offset, int size)
        {
            fixed (byte* p = message)
            {
                StorageTable[serverId].SendMessage(p + offset, size);
            }
        }
        internal void SendMessageToServer(int serverId, byte[] message, int offset, int size, out TrinityResponse response)
        {
            fixed (byte* p = message)
            {
                StorageTable[serverId].SendMessage(p + offset, size, out response);
            }
        }

        internal bool _canSendAsynRspMessage()
        {
            // condition: 
            // 1. I am a server within this memory cloud
            // 2. I am a proxy within this memory cloud
            return (my_server_id != -1 || my_proxy_id != -1);
        }

        #endregion

        #region Proxies
        private List<RemoteStorage> proxy_list = null;

        private object proxyList_init_lock = new object();


        /// <summary>
        /// Gets a list of Trinity proxy.
        /// </summary>
        internal List<RemoteStorage> ProxyList
        {
            get
            {
                if (proxy_list == null)
                {
                    lock (proxyList_init_lock)
                    {
                        if (proxy_list == null)
                        {
                            var new_proxy_list = new List<RemoteStorage>();

                            for (int i = 0; i < TrinityConfig.Proxies.Count; i++)
                            {
                                foreach (var instance in TrinityConfig.Proxies[i].Instances)
                                {
                                    new_proxy_list.Add(new RemoteStorage(instance.EndPoint, TrinityConfig.ClientMaxConn));
                                }
                            }

                            proxy_list = new_proxy_list;

                            CheckProxySignatures(proxy_list);
                        }
                    }
                }
                return proxy_list;
            }
        }

        object get_proxy_lock = new object();
        internal RemoteStorage GetProxy(int proxyId)
        {
            lock (get_proxy_lock)
            {
                var proxy = ProxyList[proxyId];
                if (!proxy.connected)
                    throw new Exception(string.Format(CultureInfo.InvariantCulture, "The proxy {0} is not connected.", proxyId));
                return proxy;
            }
        }


        private Random rand = new Random();
        internal RemoteStorage RandomProxy
        {
            get
            {
                lock (rand)
                {
                    try
                    {
                        return ProxyList[rand.Next(0, Global.ProxyCount)];
                    }
                    catch (Exception)
                    {
                        throw new Exception("No proxy is specified.");
                    }
                }
            }
        }

        #endregion

        #region Public
        /// <summary>
        /// Send a binary message to the specified Trinity server.
        /// </summary>
        /// <param name="serverId">A 32-bit server id.</param>
        /// <param name="buffer">A binary message buffer.</param>
        /// <param name="size">The size of the message.</param>
        public void SendMessageToServer(int serverId, byte* buffer, int size)
        {
            StorageTable[serverId].SendMessage(buffer, size);
        }

        /// <summary>
        /// Send a binary message to the specified Trinity server.
        /// </summary>
        /// <param name="serverId">A 32-bit server id.</param>
        /// <param name="buffer">A binary message buffer.</param>
        /// <param name="size">The size of the message.</param>
        /// <param name="response">The TrinityResponse object returned by the Trinity server.</param>
        public void SendMessageToServer(int serverId, byte* buffer, int size, out TrinityResponse response)
        {
            StorageTable[serverId].SendMessage(buffer, size, out response);
        }

        /// <summary>
        /// Send a binary message to the specified Trinity proxy.
        /// </summary>
        /// <param name="proxyId">A 32-bit proxy id.</param>
        /// <param name="buffer">A binary message buffer.</param>
        /// <param name="size">The size of the message.</param>
        public void SendMessageToProxy(int proxyId, byte* buffer, int size)
        {
            GetProxy(proxyId).SendMessage(buffer, size);
        }

        /// <summary>
        /// Send a binary message to the specified Trinity proxy.
        /// </summary>
        /// <param name="proxyId">A 32-bit proxy id.</param>
        /// <param name="buffer">A binary message buffer.</param>
        /// <param name="size">The size of the message.</param>
        /// <param name="response">The TrinityResponse object returned by the Trinity proxy.</param>
        public void SendMessageToProxy(int proxyId, byte* buffer, int size, out TrinityResponse response)
        {
            GetProxy(proxyId).SendMessage(buffer, size, out response);
        }


        public void SendMessageToServer(int serverId, byte** buffers, int* sizes, int count)
        {
            StorageTable[serverId].SendMessage(buffers, sizes, count);
        }

        public void SendMessageToServer(int serverId, byte** buffers, int* sizes, int count, out TrinityResponse response)
        {
            StorageTable[serverId].SendMessage(buffers, sizes, count, out response);
        }

        public void SendMessageToProxy(int proxyId, byte** buffers, int* sizes, int count)
        {
            GetProxy(proxyId).SendMessage(buffers, sizes, count);
        }

        public void SendMessageToProxy(int proxyId, byte** buffers, int* sizes, int count, out TrinityResponse response)
        {
            GetProxy(proxyId).SendMessage(buffers, sizes, count, out response);
        }
        #endregion
    }
}
