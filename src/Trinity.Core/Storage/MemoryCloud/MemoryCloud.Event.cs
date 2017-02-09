// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Storage
{
    public unsafe partial class MemoryCloud
    {
        /// <summary>
        /// An event that is triggered when a server is connected.
        /// </summary>
        public event ServerStatusEventHandler ServerConnected = delegate { };

        /// <summary>
        /// An event that is triggered when a server is disconnected.
        /// </summary>
        public event ServerStatusEventHandler ServerDisconnected = delegate { };

        /// <summary>
        /// Invokes a ServerConnected event.
        /// </summary>
        /// <param name="e">A event that indicates the server status is changed.</param>
        protected virtual void OnConnected(ServerStatusEventArgs e)
        {
            ServerConnected(this, e);
        }

        /// <summary>
        /// Invoked a ServerDisconnected event.
        /// </summary>
        /// <param name="e">A event that indicates the server status is changed.</param>
        protected virtual void OnDisconnected(ServerStatusEventArgs e)
        {
            ServerDisconnected(this, e);
        }

        internal void ReportServerConnectedEvent(int serverId)
        {
            ServerStatusEventArgs e = new ServerStatusEventArgs(serverId);
            OnConnected(e);
        }

        internal void ReportServerDisconnectedEvent(int serverId)
        {
            ServerStatusEventArgs e = new ServerStatusEventArgs(serverId);
            OnDisconnected(e);
        }
    }

    /// <summary>
    /// A delegates that represents a handler for ServerStatusEventArgs.
    /// </summary>
    /// <param name="sender">The object that triggers the current event.</param>
    /// <param name="e">An instance of ServerStatusEventArgs.</param>
    public delegate void ServerStatusEventHandler(object sender, ServerStatusEventArgs e);

    /// <summary>
    /// Represents an event class that is triggered when the server status is changed.
    /// </summary>
    public class ServerStatusEventArgs : EventArgs
    {
        private readonly int server_id;

        /// <summary>
        /// Constructs an instance of ServerStatusEventArgs.
        /// </summary>
        /// <param name="serverId">The id of the server whose status is changed.</param>
        public ServerStatusEventArgs(int serverId)
        {
            this.server_id = serverId;
        }

        /// <summary>
        /// The id of the server whose status is changed.
        /// </summary>
        public int ServerId
        {
            get
            {
                return server_id;
            }
        }
    }
}
