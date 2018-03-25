// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;

using Trinity;
using Trinity.Core.Lib;
using Trinity.Storage;
using Trinity.Network.Messaging;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Trinity.Diagnostics;
using Trinity.FaultTolerance;

namespace Trinity.Network.Client
{
    unsafe partial class SynClient : IDisposable
    {
        private bool DoSend(byte* buf, int len)
        {
            if (!sock_connected && !DoConnect(m_connect_retry)) // cannot connect
                return false;

            if (CNativeNetwork.ClientSend(socket, buf, len))
            {
                return true;
            }
            else
            {
                // ClientSend failed.
                sock_connected = false;
                return false;
            }
        }

        /// <summary>
        /// Send multiple buffers sequentially
        /// </summary>
        private bool DoSend(byte** bufs, int* lens, int cnt)
        {
            if (!sock_connected && !DoConnect(m_connect_retry)) // cannot connect
                return false;

            if (CNativeNetwork.ClientSendMulti(socket, bufs, lens, cnt))
            {
                return true;
            }
            else
            {
                // ClientSend failed.
                sock_connected = false;
                return false;
            }
        }

        /// <returns>
        /// E_SUCCESS:              Heartbeat success.
        /// E_FAILURE:              Could not connect to the remote IPE.
        /// E_NETWORK_RECV_FAILURE: Heartbeat sent, but AckPackage not received.
        /// E_NETWORK_SEND_FAILURE: Heartbeat could not be sent.
        /// E_RPC_EXCEPTION:        The remote handler throw an exception.
        /// </returns>
        internal TrinityErrorCode Heartbeat()
        {
            // Heartbeat() connects only once regardless of reconnect setting.
            if (!sock_connected && !DoConnect(1))  // cannot connect
                return TrinityErrorCode.E_FAILURE; // TODO error code for connection error

            if (CNativeNetwork.ClientSend(socket, HeartbeatBuffer, 8))
            {
                return WaitForAckPackage();
            }
            else
            {
                // ClientSend failed.
                FailureHandlerRegistry.MachineFailover(ipe);
                sock_connected = false;
                return TrinityErrorCode.E_NETWORK_SEND_FAILURE;
            }
        }

        /// <summary>
        /// Send multiple buffers sequentially, as a single message.
        /// </summary>
        /// <returns>
        /// E_SUCCESS:              SendMessage success.
        /// E_NETWORK_RECV_FAILURE: AckPackage cannot be received.
        /// E_NETWORK_SEND_FAILURE: Failed to send message.
        /// E_RPC_EXCEPTION:        The remote handler throw an exception.
        /// </returns>
        public TrinityErrorCode SendMessage(byte** message, int* sizes, int count)
        {
            if (DoSend(message, sizes, count))
            {
                return WaitForAckPackage();
            }
            else
            {
                sock_connected = false;
                return TrinityErrorCode.E_NETWORK_SEND_FAILURE;
            }
        }

        /// <summary>
        /// Send multiple buffers sequentially, as a single message.
        /// </summary>
        /// <returns>
        /// E_SUCCESS:              SendMessage success.
        /// E_NETWORK_RECV_FAILURE: Response cannot be received.
        /// E_NETWORK_SEND_FAILURE: Failed to send message.
        /// E_NOMEM:                Failed to allocate memory for response message.
        /// E_RPC_EXCEPTION:        The remote handler throw an exception.
        /// </returns>
        public TrinityErrorCode SendMessage(byte** message, int* sizes, int count, out TrinityResponse response)
        {
            response = null;
            if (DoSend(message, sizes, count))
            {
                byte* buf;
                int len;
                TrinityErrorCode recv_err = CNativeNetwork.ClientReceive(socket, out buf, out len);
                if (recv_err == TrinityErrorCode.E_SUCCESS)
                {
                    // will be freed by a message reader
                    response = new TrinityResponse(buf, len); 
                }
                else if (recv_err == TrinityErrorCode.E_NETWORK_RECV_FAILURE)
                {
                    // recv fail
                    sock_connected = false;
                }

                return recv_err;
            }
            else
            {
                // send fail
                sock_connected = false;
                return TrinityErrorCode.E_NETWORK_SEND_FAILURE;
            }
        }


        /// <returns>
        /// E_SUCCESS:              SendMessage success.
        /// E_NETWORK_RECV_FAILURE: AckPackage cannot be received.
        /// E_NETWORK_SEND_FAILURE: Failed to send message.
        /// E_RPC_EXCEPTION:        The remote handler throw an exception.
        /// </returns>
        public TrinityErrorCode SendMessage(byte* message, int size)
        {
            if (DoSend(message, size))
            {
                return WaitForAckPackage();
            }
            else
            {
                sock_connected = false;
                return TrinityErrorCode.E_NETWORK_SEND_FAILURE;
            }
        }

        /// <returns>
        /// E_SUCCESS:              SendMessage success.
        /// E_NETWORK_RECV_FAILURE: Response cannot be received.
        /// E_NETWORK_SEND_FAILURE: Failed to send message.
        /// E_NOMEM:                Failed to allocate memory for response message.
        /// E_RPC_EXCEPTION:        The remote handler throw an exception.
        /// E_MSG_OVERFLOW:         The response is too long to fit in a single TrinityResponse.
        /// </returns>
        public TrinityErrorCode SendMessage(byte* message, int size, out TrinityResponse response)
        {
            response = null;
            if (DoSend(message, size))
            {
                byte* buf;
                int len;
                TrinityErrorCode recv_err = CNativeNetwork.ClientReceive(socket, out buf, out len);
                if (recv_err == TrinityErrorCode.E_SUCCESS)
                {
                    // will be freed by a message reader
                    response = new TrinityResponse(buf, len); 
                }
                else if (recv_err == TrinityErrorCode.E_NETWORK_RECV_FAILURE)
                {
                    // recv fail
                    sock_connected = false;
                }

                return recv_err;
            }
            else
            {
                // send fail
                sock_connected = false;
                return TrinityErrorCode.E_NETWORK_SEND_FAILURE;
            }
        }

        /// <returns>
        /// E_SUCCESS:              WaitForAckPackage success.
        /// E_NETWORK_RECV_FAILURE: AckPackage cannot be received.
        /// E_RPC_EXCEPTION:        The remote handler throw an exception.
        /// </returns>
        private TrinityErrorCode WaitForAckPackage()
        {
            TrinityErrorCode ret = CNativeNetwork.WaitForAckPackage(socket);
            if (ret == TrinityErrorCode.E_NETWORK_RECV_FAILURE)
                sock_connected = false;
            return ret;
        }
    }
}
