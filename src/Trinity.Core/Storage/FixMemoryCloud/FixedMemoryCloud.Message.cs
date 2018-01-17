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
    public unsafe partial class FixedMemoryCloud
    {
        #region Internal
        internal bool _canSendAsynRspMessage()
        {
            // condition: 
            // 1. I am a server within this memory cloud
            // 2. I am a proxy within this memory cloud
            return (my_partition_id != -1 || my_proxy_id != -1);
        }

        #endregion

        #region Proxies
        private List<RemoteStorage> proxy_list = null;

        private object proxyList_init_lock = new object();


        /// <summary>
        /// Gets a list of Trinity proxy.
        /// </summary>
        public override IList<RemoteStorage> ProxyList
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
                                    new_proxy_list.Add(new RemoteStorage(instance, TrinityConfig.ClientMaxConn));
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
                if (!proxy.m_connected)
                    throw new Exception(string.Format(CultureInfo.InvariantCulture, "The proxy {0} is not connected.", proxyId));
                return proxy;
            }
        }


        private Random rand = new Random();
        internal RemoteStorage RandomProxy
        {
            get
            {
                lock (get_proxy_lock)
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

    }
}
