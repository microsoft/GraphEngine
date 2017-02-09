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
using Trinity.Utilities;
using Trinity.Diagnostics;
using Trinity.Win32;
using Trinity.Core.Lib;
using Trinity.Daemon;
using Trinity.Network.Messaging;
using Trinity.FaultTolerance;
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;
using System.Collections.Concurrent;
using Trinity.Network.Client;
using Trinity.Configuration;

namespace Trinity.Storage
{
#pragma warning disable 0420
    internal partial class RemoteStorage : Storage, IDisposable
    {
        BlockingCollection<Network.Client.SynClient> ConnPool = new BlockingCollection<Network.Client.SynClient>(new ConcurrentQueue<Network.Client.SynClient>());

        private volatile bool disposed = false;
        internal bool connected = false;
        private int m_send_retry = NetworkConfig.Instance.ClientSendRetry;
        private int m_client_count = 0;
        private MemoryCloud memory_cloud;
        public int MyServerId;

        internal RemoteStorage(IPEndPoint ip_endpoint, int connPerServer)
        {
            for (int i = 0; i < connPerServer; i++)
            {
                ConnectIPEndPoint(ip_endpoint);
            }
        }

        internal RemoteStorage(AvailabilityGroup trinityServer, int connPerServer, MemoryCloud mc, int serverId, bool nonblocking)
        {
            this.memory_cloud = mc;
            this.MyServerId = serverId;

            var connect_async_task = Task.Factory.StartNew(() =>
            {
                for (int k = 0; k < connPerServer; k++) // make different server connections interleaved 
                {
                    for (int i = 0; i < trinityServer.Instances.Count; i++)
                    {
                        ConnectIPEndPoint(trinityServer.Instances[i].EndPoint);
                    }
                }
                BackgroundThread.AddBackgroundTask(new BackgroundTask(Heartbeat, TrinityConfig.HeartbeatInterval));
                mc.ReportServerConnectedEvent(serverId);
            });

            if (!nonblocking)
            {
                try { connect_async_task.Wait(); }
                catch (AggregateException ex) { ExceptionDispatchInfo.Capture(ex.InnerException).Throw(); }
            }
        }

        private void ConnectIPEndPoint(IPEndPoint ip_endpoint)
        {
            while (true)
            {
                try
                {
                    var client = new Network.Client.SynClient(ip_endpoint);
                    if (client.sock_connected)
                    {
                        ConnPool.Add(client);
                        ++m_client_count;
                        connected = true;
                        break;
                    }
                }
                catch (Exception)
                {
                    Log.WriteLine(LogLevel.Debug, "Cannot connect to {0}", ip_endpoint);
                    Thread.Sleep(100);
                }
            }
        }

        private int Heartbeat()
        {
            Network.Client.SynClient sc = GetClient();
            TrinityErrorCode eResult = sc.Heartbeat();
            PutBackClient(sc);

            if (TrinityErrorCode.E_SUCCESS == eResult)
            {
                if (!connected)
                {
                    connected = true;
                    memory_cloud.ReportServerConnectedEvent(MyServerId);
                }
            }
            else
            {
                if (connected)
                {
                    connected = false;
                    memory_cloud.ReportServerDisconnectedEvent(MyServerId);
                    InvalidateSynClients();
                }
            }

            return TrinityConfig.HeartbeatInterval;
        }

        /// <summary>
        /// Called when the heartbeat daemon reports a disconnection event.
        /// In this routine, we exhaustively take all clients, close them,
        /// and then put them back, so that they stay in disconnected state,
        /// and any send message action after the remote storage is up again
        /// will trigger a new connection, rather than reporting a send failure
        /// due to stale socket.
        /// 
        /// !Note concurrent calls to this routine causes deadlock. It should be
        /// only called by a single heartbeat daemon.
        /// 
        /// !Note this routine may cause connections to oscillate between connected/
        /// disconnected state with a transient network failure. Consider the following
        /// sequence:
        /// 1. daemon detects disconnect.
        /// 2. daemon calls InvalidateSynClients()
        /// 3. RemoteStorage back online.
        /// 4. Senders take SynClients and restore connection.
        /// 5. InvalidateSynClients() closes connections
        /// </summary>
        private void InvalidateSynClients()
        {
            List<SynClient> clients = new List<SynClient>();
            for (int i=0; i<m_client_count; ++i)
            {
                clients.Add(GetClient());
            }
            foreach (var client in clients)
            {
                client.Close();
                PutBackClient(client);
            }
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (ConnPool.Count != 0)
                {
                    foreach (Network.Client.SynClient client in ConnPool)
                    {
                        client.Dispose();
                    }
                }

                this.disposed = true;
            }
        }

        ~RemoteStorage()
        {
            Dispose(false);
        }

        private Network.Client.SynClient GetClient()
        {
            return ConnPool.Take();
        }

        private void PutBackClient(Network.Client.SynClient sc)
        {
            ConnPool.Add(sc);
        }

    }
}
