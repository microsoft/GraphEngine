// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Core.Lib;
using Trinity.FaultTolerance;
using Trinity.Network.Messaging;

namespace Trinity.Network.Client
{
    unsafe partial class SynClient : IDisposable
    {
        private delegate int AsyncOperation(UInt64 socket, byte* buf, int len, NativeOverlapped* overlapped);

        private class AsyncOperationClosure
        {
            private readonly SynClient _client;
            private readonly AsyncOperation _action;
            private byte* _buf;
            private int _len;
            public TaskCompletionSource<bool> _taskCompletionSource;

            public AsyncOperationClosure(SynClient client, byte* buf, int len, AsyncOperation action)
            {
                this._client = client;
                this._action = action;
                this._buf = buf;
                this._len = len;
                this._taskCompletionSource = new TaskCompletionSource<bool>();
            }

            private void Callback(uint errorCode, uint numBytes, NativeOverlapped* pOVERLAP)
            {
                this._client.threadPoolBoundHandle.FreeNativeOverlapped(pOVERLAP);

                bool succeeded = errorCode == CNativeNetwork.ERROR_SUCCESS;
                if (!succeeded) this._client.sock_connected = false;

                if (numBytes == this._len)
                {
                    this._taskCompletionSource.TrySetResult(succeeded);
                }
                else
                {
                    this._len -= (int)numBytes;
                    if (this._buf != null) this._buf += numBytes;

                    this.BeginInvoke();
                }
            }

            public Task<bool> BeginInvoke()
            {
                NativeOverlapped* overlapped = this._client.threadPoolBoundHandle.AllocateNativeOverlapped(this.Callback, null, null);

                int err = this._action(this._client.socket, this._buf, this._len, overlapped);
                if (err != CNativeNetwork.ERROR_SUCCESS &&
                    err != CNativeNetwork.ERROR_IO_PENDING)
                {
                    // error, complete the task with false
                    this._client.threadPoolBoundHandle.FreeNativeOverlapped(overlapped);
                    this._taskCompletionSource.TrySetResult(false);
                }

                return this._taskCompletionSource.Task;
            }
        }

        private Task<bool> DoSendAsync(byte* buf, int len)
        {
            if (!sock_connected && !DoConnect(m_connect_retry)) // cannot connect
                return Task.FromResult(false);

            AsyncOperationClosure send_op = new AsyncOperationClosure(this, buf, len, CNativeNetwork.BeginClientSend);
            return send_op.BeginInvoke();
        }

        private Task<bool> DoSendAsync(byte** bufs, int* lens, int cnt)
        {
            if (!sock_connected && !DoConnect(m_connect_retry)) // cannot connect
                return Task.FromResult(false);

            var taskCompletionSource = new TaskCompletionSource<bool>();
            NativeOverlapped* overlapped = threadPoolBoundHandle.AllocateNativeOverlapped(
                (errorCode, numBytes, pOVERLAP) =>
                {
                    threadPoolBoundHandle.FreeNativeOverlapped(pOVERLAP);
                    bool succeeded = errorCode == CNativeNetwork.ERROR_SUCCESS;
                    if (!succeeded) sock_connected = false;
                    taskCompletionSource.TrySetResult(succeeded);
                },
                null,
                null);

            int err = CNativeNetwork.BeginClientSendMulti(socket, bufs, lens, cnt, overlapped);
            if (err != CNativeNetwork.ERROR_SUCCESS &&
                err != CNativeNetwork.ERROR_IO_PENDING)
            {
                threadPoolBoundHandle.FreeNativeOverlapped(overlapped);
                taskCompletionSource.TrySetResult(false);
            }

            return taskCompletionSource.Task;
        }

        private Task<(TrinityErrorCode, TrinityResponse)> DoRecvAsync()
        {
            int data_len = 0;
            byte* data_buf = null;

            AsyncOperationClosure len_recv_op = new AsyncOperationClosure(
                this,
                intBuff,
                sizeof(int),
                CNativeNetwork.BeginClientReceive);
            return len_recv_op.BeginInvoke()
                              .ContinueWith(
                                t =>
                                {
                                    if (!t.Result)
                                    {
                                        return Task.FromResult(false);
                                    }

                                    data_len = Marshal.ReadInt32((IntPtr)intBuff);
                                    data_buf = (byte*)Memory.malloc((ulong)data_len);
                                    AsyncOperationClosure data_recv_op = new AsyncOperationClosure(
                                        this,
                                        data_buf,
                                        data_len,
                                        CNativeNetwork.BeginClientReceive);
                                    return data_recv_op.BeginInvoke();
                                })
                              .Unwrap()
                              .ContinueWith(
                                t =>
                                {
                                    if (!t.Result)
                                    {
                                        if (data_buf != null) Memory.free(data_buf);
                                        return (TrinityErrorCode.E_NETWORK_RECV_FAILURE, null);
                                    }

                                    return (TrinityErrorCode.E_SUCCESS, new TrinityResponse(data_buf, data_len));
                                },
                                TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <returns>
        /// E_SUCCESS:              Heartbeat success.
        /// E_FAILURE:              Could not connect to the remote IPE.
        /// E_NETWORK_RECV_FAILURE: Heartbeat sent, but AckPackage not received.
        /// E_NETWORK_SEND_FAILURE: Heartbeat could not be sent.
        /// E_RPC_EXCEPTION:        The remote handler throw an exception.
        /// </returns>
        internal Task<TrinityErrorCode> HeartbeatAsync()
        {
            // Heartbeat() connects only once regardless of reconnect setting.
            if (!sock_connected && !DoConnect(1)) // cannot connect
                return Task.FromResult(TrinityErrorCode.E_FAILURE); // TODO error code for connection error

            return DoSendAsync(HeartbeatBuffer, HeartbeatBufferLen).ContinueWith(
                t =>
                {
                    if (t.Result)
                    {
                        return WaitForAckPackageAsync();
                    }
                    else
                    {
                        // ClientSend failed.
                        FailureHandlerRegistry.MachineFailover(ipe);
                        sock_connected = false;
                        return Task.FromResult(TrinityErrorCode.E_NETWORK_SEND_FAILURE);
                    }
                }).Unwrap();
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
        public Task<TrinityErrorCode> SendMessageAsync(byte** message, int* sizes, int count)
        {
            return DoSendAsync(message, sizes, count).ContinueWith(
                t =>
                {
                    if (t.Result)
                    {
                        return WaitForAckPackageAsync();
                    }
                    else
                    {
                        sock_connected = false;
                        return Task.FromResult(TrinityErrorCode.E_NETWORK_SEND_FAILURE);
                    }
                }).Unwrap();
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
        public Task<(TrinityErrorCode ErrorCode, TrinityResponse Response)> SendRecvMessageAsync(byte** message, int* sizes, int count)
        {
            return DoSendAsync(message, sizes, count).ContinueWith(
                t =>
                {
                    if (t.Result)
                    {
                        return DoRecvAsync();
                    }
                    else
                    {
                        // send fail
                        sock_connected = false;
                        return Task.FromResult<(TrinityErrorCode, TrinityResponse)>((TrinityErrorCode.E_NETWORK_SEND_FAILURE, null));
                    }
                }).Unwrap();
        }

        /// <returns>
        /// E_SUCCESS:              SendMessage success.
        /// E_NETWORK_RECV_FAILURE: AckPackage cannot be received.
        /// E_NETWORK_SEND_FAILURE: Failed to send message.
        /// E_RPC_EXCEPTION:        The remote handler throw an exception.
        /// </returns>
        public Task<TrinityErrorCode> SendMessageAsync(byte* message, int size)
        {
            return DoSendAsync(message, size).ContinueWith(
                t =>
                {
                    if (t.Result)
                    {
                        return WaitForAckPackageAsync();
                    }
                    else
                    {
                        sock_connected = false;
                        return Task.FromResult(TrinityErrorCode.E_NETWORK_SEND_FAILURE);
                    }
                }).Unwrap();
        }

        /// <returns>
        /// E_SUCCESS:              SendMessage success.
        /// E_NETWORK_RECV_FAILURE: Response cannot be received.
        /// E_NETWORK_SEND_FAILURE: Failed to send message.
        /// E_NOMEM:                Failed to allocate memory for response message.
        /// E_RPC_EXCEPTION:        The remote handler throw an exception.
        /// E_MSG_OVERFLOW:         The response is too long to fit in a single TrinityResponse.
        /// </returns>
        public Task<(TrinityErrorCode ErrorCode, TrinityResponse Response)> SendRecvMessageAsync(byte* message, int size)
        {
            return DoSendAsync(message, size).ContinueWith(
                t =>
                {
                    if (t.Result)
                    {
                        return DoRecvAsync();
                    }
                    else
                    {
                        // send fail
                        sock_connected = false;
                        return Task.FromResult<(TrinityErrorCode, TrinityResponse)>((TrinityErrorCode.E_NETWORK_SEND_FAILURE, null));
                    }
                }).Unwrap();
        }

        /// <returns>
        /// E_SUCCESS:              WaitForAckPackage success.
        /// E_NETWORK_RECV_FAILURE: AckPackage cannot be received.
        /// E_RPC_EXCEPTION:        The remote handler throw an exception.
        /// </returns>
        private Task<TrinityErrorCode> WaitForAckPackageAsync()
        {
            AsyncOperationClosure wait_op = new AsyncOperationClosure(
                this,
                intBuff,
                sizeof(int),
                CNativeNetwork.BeginClientReceive);
            return wait_op.BeginInvoke()
                          .ContinueWith(
                            t =>
                            {
                                if (!t.Result)
                                {
                                    return TrinityErrorCode.E_NETWORK_RECV_FAILURE;
                                }

                                return (TrinityErrorCode)Marshal.ReadInt32((IntPtr)intBuff);
                            },
                            TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}
