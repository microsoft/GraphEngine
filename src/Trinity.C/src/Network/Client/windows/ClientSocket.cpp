// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include <os/os.h>
#if defined(TRINITY_PLATFORM_WINDOWS)
#include  "ClientSocket.h"
#include "Network/Server/iocp/TrinitySocketServer.h"
#include "Network/SocketOptionsHelper.h"
#include "Trinity/Configuration/TrinityConfig.h"
#include <thread>

namespace Trinity
{
    namespace Network
    {
        uint64_t CreateClientSocket()
        {
            InitializeNetwork();

            SOCKET clientsocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);

            if (INVALID_SOCKET == clientsocket)
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "ClientSocket: Cannot create a client socket.");
                closesocket(clientsocket);
                return INVALID_SOCKET;
            }

            // The client side already have heartbeat packages. Don't enable keepalive messages.
            return SetSocketOptions(clientsocket, /*enable_keepalive:*/ false, /*disable_sendbuf:*/TrinityConfig::ClientDisableSendBuffer());
        }

        bool ClientSocketConnect(uint64_t clientsocket, uint32_t ip, uint16_t port)
        {
            SOCKADDR_IN ipe;
            ipe.sin_family = AF_INET;
            ipe.sin_port = htons(port); // converts a u_short from host to network byte order (big-endian)
            ipe.sin_addr.S_un.S_addr = ip;

            if (SOCKET_ERROR == connect((SOCKET)clientsocket, (sockaddr*)&ipe, sizeof(SOCKADDR_IN)))
            {
                //Diagnostics::WriteLine(Diagnostics::LogLevel::Warning, "Cannot connect to an IP endpoint.");
                closesocket((SOCKET)clientsocket);
                return false;
            }
            
            if (TrinityConfig::Handshake())
            {
                return ClientSocketHandshake(clientsocket);
            }
            else
            {
                return true;
            }
        }

        bool ClientSend(uint64_t socket, char* buf, int32_t len)
        {
            do
            {
                int32_t bytesSent = send((SOCKET)socket, buf, len, 0);
                if (SOCKET_ERROR == bytesSent)
                {
                    int error_code = WSAGetLastError();
                    Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "ClientSocket: Errors occur during network send: {0} bytes to send. Error code = {1}, Thread Id = {2}", len, error_code, std::this_thread::get_id());

                    if (INVALID_SOCKET == socket)
                    {
                        Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "ClientSocket: Provided socket for send is invalid.");
                    }

                    closesocket((SOCKET)socket);
                    return false;
                }
                buf += bytesSent;
                len -= bytesSent;
            } while (len > 0);
            return true;
        }

        bool ClientSendMulti(uint64_t socket, char** bufs, int32_t* lens, int32_t cnt)
        {
            WSABUF* lpBuffers = (WSABUF*)_alloca(sizeof(WSABUF) * cnt);
            int32_t total_len = 0;
            DWORD   bytesSent;
            for (int i=0; i < cnt; ++i)
            {
                lpBuffers[i].buf = bufs[i];
                lpBuffers[i].len = lens[i];
                total_len += lens[i];
            }
            do
            {
                int eResult = WSASend((SOCKET)socket, lpBuffers, cnt, &bytesSent,
                    /*flags, completion object, completion routine*/0, NULL, NULL);

                if (SOCKET_ERROR == eResult)
                {
                    int error_code = WSAGetLastError();
                    Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "ClientSocket: Errors occur during network send: {0} bytes to send. Error code = {1}, Thread Id = {2}", total_len, error_code, std::this_thread::get_id() );

                    if (INVALID_SOCKET == socket)
                    {
                        Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "ClientSocket: Provided socket for send is invalid.");
                    }

                    closesocket((SOCKET)socket);
                    return false;
                }
                total_len -= bytesSent;
                // skip sent lpBuffers
                while (bytesSent >= lpBuffers->len)
                {
                    bytesSent -= lpBuffers->len;
                    ++lpBuffers;
                    --cnt;
                }
                // adjust current lpBuffer
                lpBuffers->buf += bytesSent;
                lpBuffers->len -= bytesSent;
            } while (total_len > 0);
            return true;
        }

        bool _do_recv(SOCKET socket, char* buf, int32_t len)
        {
            while (len)
            {
                int32_t bytesRecvd = recv(socket, buf, len, 0);
                if (SOCKET_ERROR == bytesRecvd || 0 == bytesRecvd)
                {
                    int error_code = WSAGetLastError();
                    Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "ClientSocket: Errors occur during network recv: Error code = {0}, Thread Id={1}", error_code, std::this_thread::get_id());
                    closesocket((SOCKET)socket);
                    return false;
                }
                buf += bytesRecvd;
                len -= bytesRecvd;
            }

            return true;
        }

        TrinityErrorCode ClientReceive(uint64_t _socket, OUT char* & buf, OUT int32_t & len)
        {
            SOCKET socket = (SOCKET)_socket;
            if (!_do_recv(socket, (char*)&len, sizeof(int32_t)))
            {
                return TrinityErrorCode::E_NETWORK_RECV_FAILURE;
            }

            if (len < 0) return (TrinityErrorCode)len;

            buf = (char*)malloc(len); // will be freed by a C# message reader.
            if (NULL == buf)
            {
                Trinity::Diagnostics::FatalError("ClientSocket: Cannot allocate memory in network Receive.");
                return TrinityErrorCode::E_NOMEM;
            }

            if (!_do_recv(socket, buf, len))
            {
                return TrinityErrorCode::E_NETWORK_RECV_FAILURE;
            }

            return TrinityErrorCode::E_SUCCESS;
        }

        TrinityErrorCode WaitForAckPackage(uint64_t _socket)
        {
            int32_t buf;
            char *recvbuf = (char*)&buf;
            SOCKET socket = (SOCKET)_socket;
            if (!_do_recv(socket, recvbuf, sizeof(int32_t)))
            {
                return TrinityErrorCode::E_NETWORK_RECV_FAILURE;
            }
            else
            {
                return (TrinityErrorCode)buf;
            }
        }

        void CloseClientSocket(uint64_t socket)
        {
            closesocket((SOCKET)socket);
        }

        uint32_t IP2UInt32(const char* ip)
        {
            return inet_addr(ip);
        }
    }
}
#endif
