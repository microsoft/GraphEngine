// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include <os/os.h>
#if defined(TRINITY_PLATFORM_WINDOWS)
#include "TrinitySocketServer.h"
#include <Trinity/Environment.h>
#include <Mstcpip.h>

namespace Trinity
{
    namespace Network
    {
        void ReceiveAsync(PerSocketContextObject * pContext, bool receivePrefix)
        {
            DWORD flags = 0; // None flag
            ResetOverlappedStruct(pContext->pOverlapped, SocketAsyncOperation::Receive);
            int wsaBufferCnt = receivePrefix ? 2 : 1;
            LPWSABUF pWSABufArray = receivePrefix ? &(pContext->wsaPrefixBuf) : &(pContext->wsaBodyBuf);
            int statusCode = WSARecv(pContext->socket, pWSABufArray /*A pointer to an array of WSABUF structures*/, wsaBufferCnt, NULL /*bytes_recvd*/, &flags, (LPWSAOVERLAPPED)(pContext->pOverlapped), NULL);

            if (SOCKET_ERROR == statusCode &&
                WSA_IO_PENDING != WSAGetLastError())
            {
                /// initial recv operation
                /// If an overlapped operation completes immediately,
                /// WSARecv returns a value of zero and the lpNumberOfBytesRecvd parameter is updated
                /// with the number of bytes received and the flag bits indicated by the lpFlags parameter are also updated.
                /// If the overlapped operation is successfully initiated and will complete later,
                /// WSARecv returns SOCKET_ERROR and indicates error code WSA_IO_PENDING.
                /// If any overlapped function fails with WSA_IO_PENDING or immediately succeeds,
                /// the completion event will always be signaled and the completion routine will be scheduled to run (if specified)
                ///  Diagnostics::WriteLine(Diagnostics::LogLevel::Info, "Client is disconnected.");
                CloseClientConnection(pContext, false);
            }
            // otherwise, the receive operation has completed immediately or it is pending
        }

        // return value indicates whether the whole message is received
        bool ProcessRecv(PerSocketContextObject * pContext, uint32_t bytesRecvd)
        {
            if (bytesRecvd == 0)
            {
                CloseClientConnection(pContext, false);
                return false;
            }

            // Message prefix is not yet completely received
            if (pContext->ReceivedPrefixBytes < UInt32_Contants::MessagePrefixLength)
            {
                if (pContext->ReceivedPrefixBytes + bytesRecvd >= UInt32_Contants::MessagePrefixLength)
                {
                    bytesRecvd -= (UInt32_Contants::MessagePrefixLength - pContext->ReceivedPrefixBytes);
                    pContext->ReceivedPrefixBytes = UInt32_Contants::MessagePrefixLength;

                    // Message prefix received into BodyLength. Calibrate RecvBuffer now.

                    if (pContext->WaitingHandshakeMessage && pContext->BodyLength != HANDSHAKE_MESSAGE_LENGTH)
                    {
                        Trinity::Diagnostics::WriteLine(Trinity::Diagnostics::LogLevel::Error, "ServerSocket: Incorrect client handshake sequence, Client = {0}", pContext);
                        CloseClientConnection(pContext, false);
                        return false;
                    }

                    if (pContext->BodyLength < 0)
                    {
                        Trinity::Diagnostics::WriteLine(Trinity::Diagnostics::LogLevel::Error, "ServerSocket: Incorrect message header received, Client = {0}", pContext);
                        CloseClientConnection(pContext, false);
                        return false;
                    }

                    if (pContext->BodyLength > pContext->RecvBufferLen)
                    {
                        char* new_buf = (char*)malloc(pContext->BodyLength);
                        if (NULL == new_buf)
                        {
                            Trinity::Diagnostics::FatalError("ServerSocket: Cannot allocate memory during message receiving.");
                            CloseClientConnection(pContext, false);
                            return false;
                        }
                        memcpy(new_buf, pContext->RecvBuffer, bytesRecvd);
                        free(pContext->RecvBuffer);
                        pContext->RecvBufferLen  = pContext->BodyLength;
                        pContext->RecvBuffer     = new_buf;
                    }
                }
                else
                {
                    pContext->ReceivedPrefixBytes += bytesRecvd;
                    pContext->wsaPrefixBuf.buf += bytesRecvd;
                    pContext->wsaPrefixBuf.len -= bytesRecvd;
                    ReceiveAsync(pContext, true);
                    return false;
                }
            }

            pContext->ReceivedMessageBodyBytes += bytesRecvd;
            if (pContext->ReceivedMessageBodyBytes < pContext->BodyLength)
            {
                // the whole message body is not yet completely received
                pContext->wsaBodyBuf.buf = pContext->RecvBuffer + pContext->ReceivedMessageBodyBytes;
                pContext->wsaBodyBuf.len = pContext->RecvBufferLen - pContext->ReceivedMessageBodyBytes;
                ReceiveAsync(pContext, false);
                return false;
            }
            else
            {
                // we have the complete message.
                pContext->Message = pContext->RecvBuffer;
                return true;
            }
        }
    }
}
#endif//platform