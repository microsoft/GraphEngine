// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

using Trinity;
using Trinity.Network;
using Trinity.Network.Client;
using Trinity.Network.Messaging;
using System.Diagnostics;
using Trinity.Core.Lib;

namespace Trinity.Storage
{
    public unsafe partial class RemoteStorage : IStorage, IDisposable
    {
        /// <summary>
        /// Checks for possible errors that occur during message passing, 
        /// and throw exceptions accordingly.
        /// </summary>
        private static void _error_check(TrinityErrorCode err)
        {
            switch (err)
            {
                case TrinityErrorCode.E_NOMEM:
                    throw new OutOfMemoryException("Cannot allocate memory for response message.");
                case TrinityErrorCode.E_RPC_EXCEPTION:
                    throw new IOException("Remote message handler throws an exception.");
                case TrinityErrorCode.E_NETWORK_SEND_FAILURE:
                case TrinityErrorCode.E_NETWORK_RECV_FAILURE:
                    throw new IOException("Network errors occur.");
            }
        }

        /// <summary>
        /// Get a SynClient and apply the function. Intended for SendMessage calls.
        /// Will retry if the function returns neither E_SUCCESS nor E_RPC_EXCEPTION.
        /// </summary>
        private void _use_synclient(Func<SynClient, TrinityErrorCode> func)
        {
            TrinityErrorCode err = TrinityErrorCode.E_SUCCESS;
            for (int i = 0; i < m_send_retry; i++)
            {
                SynClient sc = GetClient();
                err = func(sc);
                PutBackClient(sc);
                if (err == TrinityErrorCode.E_SUCCESS || err == TrinityErrorCode.E_RPC_EXCEPTION)
                    break;
            }
            _error_check(err);
        }

        /// <inheritdoc/>
        public void SendMessage(byte* message, int size)
        {
            _use_synclient(sc => sc.SendMessage(message, size));
        }

        /// <inheritdoc/>
        public void SendMessage(byte* message, int size, out TrinityResponse response)
        {
            TrinityResponse _rsp = null;
            _use_synclient(sc => sc.SendMessage(message, size, out _rsp));
            response = _rsp;
        }

        /// <inheritdoc/>
        public void SendMessage(byte** message, int* sizes, int count)
        {
            _use_synclient(sc => sc.SendMessage(message, sizes, count));
        }

        /// <inheritdoc/>
        public void SendMessage(byte** message, int* sizes, int count, out TrinityResponse response)
        {
            TrinityResponse _rsp = null;
            _use_synclient(sc => sc.SendMessage(message, sizes, count, out _rsp));
            response = _rsp;
        }

        public T GetModule<T>() where T : CommunicationModule
        {
            return this.m_memorycloud?.GetCommunicationModule<T>();
        }
    }
}
