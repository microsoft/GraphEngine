// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.IO;
using System.Threading.Tasks;
using Trinity.Network;
using Trinity.Network.Client;
using Trinity.Network.Messaging;

namespace Trinity.Storage
{
    public partial class RemoteStorage : IStorage, IDisposable
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
                case TrinityErrorCode.E_MSG_OVERFLOW:
                    throw new MessageTooLongException("Response message too long.");
            }
        }

        /// <summary>
        /// Get a SynClient and apply the function. Intended for SendMessage calls.
        /// Will retry if the function returns neither E_SUCCESS nor E_RPC_EXCEPTION.
        /// </summary>
        private async Task _use_synclient(Func<SynClient, Task<TrinityErrorCode>> func)
        {
            TrinityErrorCode err = TrinityErrorCode.E_SUCCESS;
            for (int i = 0; i < m_send_retry; i++)
            {
                SynClient sc = GetClient();
                err = await func(sc);
                PutBackClient(sc);
                _error_check(err);
                if (err == TrinityErrorCode.E_SUCCESS) break;
            }
        }

        /// <summary>
        /// Get a SynClient and apply the function. Intended for SendMessage calls.
        /// Will retry if the function returns neither E_SUCCESS nor E_RPC_EXCEPTION.
        /// </summary>
        private async Task<TrinityResponse> _use_synclient(Func<SynClient, Task<(TrinityErrorCode ErrorCode, TrinityResponse Response)>> func)
        {
            TrinityErrorCode err = TrinityErrorCode.E_SUCCESS;
            for (int i = 0; i < m_send_retry; i++)
            {
                SynClient sc = GetClient();
                var result = await func(sc);
                err = result.ErrorCode;
                PutBackClient(sc);
                _error_check(err);
                if (err == TrinityErrorCode.E_SUCCESS)
                {
                    return result.Response;
                }
            }

            return null;
        }

        /// <inheritdoc/>
        public unsafe Task SendMessageAsync(byte* message, int size)
        {
            return _use_synclient(sc => sc.SendMessageAsync(message, size));
        }

        /// <inheritdoc/>
        public unsafe Task<TrinityResponse> SendRecvMessageAsync(byte* message, int size)
        {
            return _use_synclient(sc => sc.SendRecvMessageAsync(message, size));
        }

        /// <inheritdoc/>
        public unsafe Task SendMessageAsync(byte** message, int* sizes, int count)
        {
            return _use_synclient(sc => sc.SendMessageAsync(message, sizes, count));
        }

        /// <inheritdoc/>
        public unsafe Task<TrinityResponse> SendRecvMessageAsync(byte** message, int* sizes, int count)
        {
            return _use_synclient(sc => sc.SendRecvMessageAsync(message, sizes, count));
        }

        /// <inheritdoc />
        public virtual T GetCommunicationModule<T>() where T : CommunicationModule
        {
            return this.m_memorycloud?.GetCommunicationModule<T>();
        }
    }
}
