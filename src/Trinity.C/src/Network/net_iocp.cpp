// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include <os/os.h>
#if defined(TRINITY_PLATFORM_WINDOWS)
#include "Trinity/Diagnostics/Log.h"
#include "Trinity/Environment.h"
#include "Trinity/Threading/TrinityLock.h"
#include "Network/Network.h"
#include "Network/SocketOptionsHelper.h"
#include <atomic>

namespace Trinity
{
    namespace Network
    {
        SOCKET socket;

        DWORD WINAPI SocketAcceptThreadProc(LPVOID lpParameter)
        {
            /// Start accepting new connections from clients
            SOCKADDR accept_sockaddr;
            INT      accept_sockaddr_len = sizeof(accept_sockaddr);
            DWORD    accept_sockaddr_strbuf_len = 1024;
            LPWSTR   accept_sockaddr_strbuf     = (LPWSTR)malloc(accept_sockaddr_strbuf_len * sizeof(WCHAR));
            while (true)
            {
                accept_sockaddr_len = sizeof(SOCKADDR);
                SOCKET acceptSocket = WSAAccept(socket, &accept_sockaddr, &accept_sockaddr_len, NULL, 0);

                if (INVALID_SOCKET == acceptSocket)
                {
                    int errorCode = WSAGetLastError();
                    free(accept_sockaddr_strbuf);
                    if (WSAEINTR == errorCode)
                    {
                        // the accept call is interrupted. assume the server has been shutdown.
                        return 0;
                    }
                    Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "ServerSocket: Cannot accept incoming connection, error code {0}.", errorCode);
                    continue;
                }

                accept_sockaddr_strbuf_len = 1024;
                if (WSAAddressToString(&accept_sockaddr, accept_sockaddr_len, NULL, accept_sockaddr_strbuf, &accept_sockaddr_strbuf_len))
                {
                    lstrcpynW(accept_sockaddr_strbuf, L"<Unknown address>", 1024);
                }
                Diagnostics::WriteLine(Diagnostics::LogLevel::Debug, "ServerSocket: Incomming connection from {0}", String(accept_sockaddr_strbuf));

                acceptSocket = (SOCKET)SetSocketOptions(acceptSocket, /*enable_keepalive:*/true, /*disable_sendbuf:*/true);
                if (INVALID_SOCKET == acceptSocket)
                {
                    continue;
                }

                // Add it to the client socket set
                sock_t * p = alloc_incoming_socket(acceptSocket);

                // associate the accept socket with the previously created IOCP
                if (TrinityErrorCode::E_SUCCESS != Events::enter_eventloop(p))
                {
                    close_incoming_conn(p, false);
                    continue;
                }

                // post the first async receive
                recv_async(p);
            }
        }

        int start_socket_server(uint16_t port)
        {
            if (!initialize_network())
                return -1;

            socket = WSASocket(AF_INET, SOCK_STREAM, IPPROTO_IP, NULL, 0, WSA_FLAG_OVERLAPPED);

            if (INVALID_SOCKET == socket)
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Fatal, "Cannot create a WSA socket.");
                WSACleanup();
                return -1;
            }

            SOCKADDR_IN ipe;
            ipe.sin_family = AF_INET;
            ipe.sin_port = htons(port); // converts a u_short from host to network byte order (big-endian)
            ipe.sin_addr.S_un.S_addr = INADDR_ANY;
            if (SOCKET_ERROR == bind(socket, (sockaddr*)&ipe, sizeof(SOCKADDR_IN)))
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Fatal, "Cannot bind the socket to an IP endpoint {0}.", port);
                shutdown_socket_server();
                return -1;
            }

            if (SOCKET_ERROR == listen(socket, SOMAXCONN))
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Fatal, "Cannot turn the socket into listening state.");
                shutdown_socket_server();
                return -1;
            }

            socket = (SOCKET)SetSocketOptions((uint64_t)socket, /*enable_keepalive:*/true, /*disable_sendbuf:*/true);
            if (INVALID_SOCKET == socket)
            {
                shutdown_socket_server();
                return -1;
            }

            Diagnostics::WriteLine(Diagnostics::LogLevel::Info, "Listening endpoint :{0}", port);

            Diagnostics::WriteLine(Diagnostics::LogLevel::Info, "Waiting for client connection ...");

            DWORD threadId = 0;
            if (NULL == CreateThread(NULL, 0, SocketAcceptThreadProc, NULL, 0, &threadId))
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Fatal, "Cannot create socket accept threads.");
                shutdown_socket_server();
                return -1;
            }

            return (int)socket;
        }

        int shutdown_socket_server()
        {
            //  Close the listening socket so that no new connections are coming in.
            //  The accept thread will be interrupted, and quit after this call.
            closesocket(socket);

            //  Proceed to close all existing client sockets.
            close_all_incoming_conn();

            return 0;
        }

        TrinityErrorCode recv_async(sock_t* p)
        {
            // calibrate WSA buffer according to msg_buf & pending_len
            p->wsa_buf.buf = p->msg_buf;
            p->wsa_buf.len = p->pending_len;

            DWORD flags = 0; // None flag
            Events::reset_work(p->work, Events::worktype_t::Receive);
            int statusCode = WSARecv(p->socket, &p->wsa_buf /*A pointer to an array of WSABUF structures*/, 1, NULL /*bytes_recvd*/, &flags, (LPWSAOVERLAPPED)(p->work), NULL);

            if (SOCKET_ERROR == statusCode &&
                WSA_IO_PENDING != WSAGetLastError())
            {
                /// If an overlapped operation completes immediately,
                /// WSARecv returns a value of zero and the lpNumberOfBytesRecvd parameter is updated
                /// with the number of bytes received and the flag bits indicated by the lpFlags parameter are also updated.
                /// If the overlapped operation is successfully initiated and will complete later,
                /// WSARecv returns SOCKET_ERROR and indicates error code WSA_IO_PENDING.
                /// If any overlapped function fails with WSA_IO_PENDING or immediately succeeds,
                /// the completion event will always be signaled and the completion routine will be scheduled to run (if specified)
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "Network: Errors occur during WSARecv. Error code = {0}, sock_t = {1}", WSAGetLastError(), p);
                return TrinityErrorCode::E_NETWORK_RECV_FAILURE;
            }
            // otherwise, the receive operation has completed immediately or it is pending
            return TrinityErrorCode::E_RETRY;
        }

        TrinityErrorCode send_async(sock_t * p)
        {
            // calibrate WSA buffer according to msg_buf & pending_len
            p->wsa_buf.buf = p->msg_buf;
            p->wsa_buf.len = p->pending_len;

            Events::reset_work(p->work, Events::worktype_t::Send);
            int32_t statusCode = WSASend(p->socket, &p->wsa_buf, /*buffer_cnt*/1, /*bytes_sent*/NULL, /*flags*/0, (LPWSAOVERLAPPED)(p->work), /*callback*/NULL);
            if (SOCKET_ERROR == statusCode &&
                WSA_IO_PENDING != WSAGetLastError())
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "Network: Errors occur during WSASend. Error code = {0}, sock_t = {1}", WSAGetLastError(), p);
                return TrinityErrorCode::E_NETWORK_SEND_FAILURE;
            }
            return TrinityErrorCode::E_RETRY;
        }
    }
}

#endif
