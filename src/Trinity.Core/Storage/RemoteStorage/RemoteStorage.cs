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
    public partial class RemoteStorage : IStorage
    {
        BlockingCollection<Network.Client.SynClient> ConnPool = new BlockingCollection<Network.Client.SynClient>(new ConcurrentQueue<Network.Client.SynClient>());

        private volatile bool disposed = false;
        internal bool m_connected = false;
        private int m_send_retry = NetworkConfig.Instance.ClientSendRetry;
        private int m_client_count = 0;
        private MemoryCloud m_memorycloud = null;
        private BackgroundTask m_bgtask = null;

        /// <summary>
        /// Deprecated. Use PartitionId instead.
        /// </summary>
        [Obsolete]
        public int MyServerId { get { return PartitionId; } set { PartitionId = value; } }
        /// <summary>
        /// The partition id of this remote instance.
        /// </summary>
        public int PartitionId { get; protected set; }

        /// <summary>
        /// Client mode ctor
        /// </summary>
        internal RemoteStorage(ServerInfo server_info, int connPerServer) : this(new[] { server_info }, connPerServer, mc: null, partitionId: -1, nonblocking: false) { }

        protected internal RemoteStorage(IEnumerable<ServerInfo> servers, int connPerServer, MemoryCloud mc, int partitionId, bool nonblocking)
        {
            this.m_memorycloud = mc;
            this.PartitionId = partitionId;

            var connect_async_task = Task.Factory.StartNew(() =>
            {
                for (int k = 0; k < connPerServer; k++) // make different server connections interleaved 
                {
                    foreach(var s in servers)
                    {
                        Connect(s);
                    }
                }
                if (mc != null && partitionId != -1)
                {
                    m_bgtask = new BackgroundTask(Heartbeat, TrinityConfig.HeartbeatInterval);
                    BackgroundThread.AddBackgroundTask(m_bgtask);
                    mc.ReportServerConnectedEvent(this);
                }
            });

            if (!nonblocking)
            {
                try { connect_async_task.Wait(); }
                catch (AggregateException ex) { ExceptionDispatchInfo.Capture(ex.InnerException).Throw(); }
            }
        }

        private void Connect(ServerInfo server_info)
        {
            while (true)
            {
                try
                {
                    var ep = server_info.EndPoint;
                    var client = new Network.Client.SynClient(ep);
                    if (client.sock_connected)
                    {
                        ConnPool.Add(client);
                        ++m_client_count;
                        m_connected = true;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Debug, "RemoteStorage: Cannot connect to {0}:{1}: {2}", server_info.HostName, server_info.Port, ex.Message);
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
                if (!m_connected)
                {
                    m_connected = true;
                    m_memorycloud.ReportServerConnectedEvent(this);
                }
            }
            else
            {
                if (m_connected)
                {
                    m_connected = false;
                    m_memorycloud.ReportServerDisconnectedEvent(this);
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
            for (int i = 0; i < m_client_count; ++i)
            {
                clients.Add(GetClient());
            }
            foreach (var client in clients)
            {
                client.Close();
                PutBackClient(client);
            }
        }

        public void Dispose()
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
                BackgroundThread.RemoveBackgroundTask(m_bgtask);

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
