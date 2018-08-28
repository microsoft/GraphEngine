// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include <os/os.h>
#if !defined(TRINITY_PLATFORM_WINDOWS)
#include <thread>
#include <map>
#include "os/platforms/posix.h"
#include "Trinity/Threading/TrinityLock.h"
#include "Trinity/Configuration/TrinityConfig.h"
#include "Trinity/Hash/NonCryptographicHash.h"
#include "Network/Server/posix/TrinitySocketServer.h"
#include "Network/SocketOptionsHelper.h"
#include "Network/ProtocolConstants.h"

static bool make_nonblocking(int fd)
{
    int status_flags = fcntl(fd, F_GETFL, 0);
    if (-1 == status_flags)
        return false;
    status_flags |= O_NONBLOCK;
    if (-1 == fcntl(fd, F_SETFL, status_flags))
        return false;
    return true;
}

namespace Trinity
{
    namespace Network
    {
        TrinityLock psco_spinlock; // psco = per socket context object
        std::map<int, PerSocketContextObject*> psco_map;
        std::atomic<size_t> g_threadpool_size;
        std::thread* socket_accept_thread = nullptr;
        int accept_sock; // accept socket

#pragma region PerSocketContextObject management
        void AddPerSocketContextObject(PerSocketContextObject * pContext)
        {
            psco_spinlock.lock();
            psco_map.insert(std::pair<int, PerSocketContextObject*>(pContext->fd, pContext));
            psco_spinlock.unlock();

        }
        void RemovePerSocketContextObject(int fd)
        {
            psco_spinlock.lock();
            psco_map.erase(fd);
            psco_spinlock.unlock();
        }

        PerSocketContextObject* GetPerSocketContextObject(int fd)
        {
            psco_spinlock.lock();
            PerSocketContextObject* pContext = psco_map[fd];
            psco_spinlock.unlock();
            return pContext;
        }

        PerSocketContextObject* AllocatePerSocketContextObject(int fd)
        {
            PerSocketContextObject* p = (PerSocketContextObject*)malloc(sizeof(PerSocketContextObject));

            p->RecvBuffer = (char*)malloc(UInt32_Contants::RecvBufferSize);
            p->RecvBufferLen = UInt32_Contants::RecvBufferSize;
            p->avg_RecvBufferLen = UInt32_Contants::RecvBufferSize;

            p->fd = fd;
            p->WaitingHandshakeMessage = TrinityConfig::Handshake();

            return p;
        }

        void FreePerSocketContextObject(PerSocketContextObject* p)
        {
            free(p->RecvBuffer);
            free(p);
        }

        void ResetContextObjects(PerSocketContextObject * pContext)
        {
            free(pContext->Message);
            pContext->Message = NULL;
            pContext->ReceivedMessageBodyBytes = 0;
            pContext->RemainingBytesToSend = 0;

            // Calculate average received message length with a sliding window.
            pContext->avg_RecvBufferLen = (uint32_t)(pContext->avg_RecvBufferLen * Float_Constants::AvgSlideWin_a + pContext->ReceivedMessageBodyBytes * Float_Constants::AvgSlideWin_b);
            // Make sure that average received message length is capped above default value.
            pContext->avg_RecvBufferLen = std::max(pContext->avg_RecvBufferLen, static_cast<uint32_t>(UInt32_Contants::RecvBufferSize));
            // If the average received message length drops below half of current recv buf len, adjust it.
            if (pContext->avg_RecvBufferLen < pContext->RecvBufferLen / Float_Constants::AvgSlideWin_r)
            {
                free(pContext->RecvBuffer);
                pContext->RecvBufferLen = pContext->avg_RecvBufferLen;
                pContext->RecvBuffer = (char*)malloc(pContext->RecvBufferLen);
            }
        }
#pragma endregion

        void SocketAcceptThreadProc()
        {
            char clientaddr[INET6_ADDRSTRLEN];
            while (true)
            {
                sockaddr addr;
                socklen_t addrlen = sizeof(sockaddr);
                int connected_sock_fd = accept4(accept_sock, &addr, &addrlen, SOCK_NONBLOCK);
                if (-1 == connected_sock_fd)
                {
                    /* Break the loop if listening socket is shut down. */
                    if (EINVAL == errno) { break; }
                    else { continue; } // XXX in the Windows side we shutdown the networking subsystem here
                }

                if (NULL == inet_ntop(addr.sa_family, addr.sa_data, clientaddr, INET6_ADDRSTRLEN))
                {
                    strcpy(clientaddr, "Unknown address");
                };
                Diagnostics::WriteLine(Diagnostics::LogLevel::Debug, "ServerSocket: Incomming connection from {0}", String(clientaddr));

                PerSocketContextObject * pContext = AllocatePerSocketContextObject(connected_sock_fd);
                AddPerSocketContextObject(pContext);
                EnterEventMonitor(pContext);
            }
        }

        int StartSocketServer(uint16_t port)
        {
            g_threadpool_size = 0;
            accept_sock = -1;
            struct addrinfo hints, *addrinfos, *addrinfop;
            memset(&hints, 0, sizeof(struct addrinfo));
            hints.ai_family   = AF_INET;
            hints.ai_socktype = SOCK_STREAM;
            hints.ai_flags    = AI_PASSIVE; // wildcard addresses
            char port_buf[6];
            sprintf(port_buf, "%u", port);
            int error_code = getaddrinfo(NULL, port_buf, &hints, &addrinfos);
            if (error_code != 0)
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "Cannot obtain local address info.");
                return -1;
            }

            for (addrinfop = addrinfos; addrinfop != NULL; addrinfop = addrinfop->ai_next)
            {
                accept_sock = socket(addrinfop->ai_family, addrinfop->ai_socktype,
                                     addrinfop->ai_protocol);
                // TODO bind on any address
                if (-1 == accept_sock)
                    continue;
                if (0 == bind(accept_sock, addrinfop->ai_addr, addrinfop->ai_addrlen))
                    break;
                close(accept_sock);
            }
            if (addrinfop == NULL)
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "Cannot bind the socket to an IP endpoint {0}.", port);
                ShutdownSocketServer();
                return -1;
            }
            //make_nonblocking(accept_sock);
            freeaddrinfo(addrinfos);

            if (-1 == listen(accept_sock, SOMAXCONN))
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "Cannot turn the socket into listening state.");
                ShutdownSocketServer();
                return -1;
            }

            if (0 != InitializeEventMonitor())
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "Cannot initialize socket server event monitor.");
                ShutdownSocketServer();
                return -1;
            }

            Diagnostics::WriteLine(Diagnostics::LogLevel::Info, "Listening endpoint :{0}", port);

            Diagnostics::WriteLine(Diagnostics::LogLevel::Info, "Waiting for client connection ...");

            socket_accept_thread = new std::thread(SocketAcceptThreadProc);

            return accept_sock;
        }

        int ShutdownSocketServer()
        {
            /* signal the accept thread that we are shutting down */
            shutdown(accept_sock, SHUT_RD);
            if (socket_accept_thread != nullptr)
            {
                socket_accept_thread->join();
                delete socket_accept_thread;
                socket_accept_thread = nullptr;
            }
            close(accept_sock);
            UninitializeEventMonitor();

            while (g_threadpool_size > 0) { usleep(100000); }
            /* Now, all WorkerThreadProc exit. Proceed to close all client sockets. */
            psco_spinlock.lock();
            std::vector<PerSocketContextObject*> psco_vec = std::vector<PerSocketContextObject*>(psco_map.size());
            for (auto& psco : psco_map) { psco_vec.push_back(psco.second); }
            psco_spinlock.unlock();

            for (auto& psco : psco_vec) { CloseClientConnection(psco, false); }

            return 0;
        }

        bool ProcessRecv(PerSocketContextObject* pContext)
        {
            int fd = pContext->fd;
            int32_t body_length;

            /* Receive header */
            uint32_t bytes_left = UInt32_Contants::MessagePrefixLength;
            uint32_t p = 0;
            while (bytes_left > 0)
            {
                ssize_t bytes_read = read(fd, ((char*)&body_length) + p, bytes_left);
                p += bytes_read;
                bytes_left -= bytes_read;
            }

            if (pContext->WaitingHandshakeMessage && body_length != HANDSHAKE_MESSAGE_LENGTH)
            {
                Trinity::Diagnostics::WriteLine(Trinity::Diagnostics::LogLevel::Error, "ServerSocket: Incorrect client handshake sequence, Client = {0}", pContext);
                CloseClientConnection(pContext, false);
                return false;
            }

            /**
             * Note: we cap the length to int32_t, because for a server's response, a negative
             * length is regarded as an error code. So we should keep the lengths of client/server
             * payloads unified.
             */
            if (body_length < 0)
            {
                Trinity::Diagnostics::WriteLine(Trinity::Diagnostics::LogLevel::Error, "ServerSocket: Incorrect message header received, Client = {0}", pContext);
                CloseClientConnection(pContext, false);
                return false;
            }

            if (((uint32_t)body_length) > pContext->RecvBufferLen)
            {
                pContext->RecvBuffer = (char*)realloc(pContext->RecvBuffer, body_length);
                pContext->RecvBufferLen = body_length;
                if (pContext->RecvBuffer == nullptr)
                {
                    Trinity::Diagnostics::FatalError("ServerSocket: Cannot allocate memory during message receiving.");
                    CloseClientConnection(pContext, false);
                    return false;
                }
            }

            /* Receive body */
            bytes_left = (uint32_t)body_length;
            p = 0;
            while (bytes_left > 0)
            {
                ssize_t bytes_read = read(fd, pContext->RecvBuffer + p, bytes_left);
                if (bytes_read == 0 || (bytes_read == -1 && EAGAIN != errno))
                {
                    Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "ServerSocket: Error reading from client socket {0}.", fd);
                    CloseClientConnection(pContext, false);
                    return false;
                }
                p += bytes_read;
                bytes_left -= bytes_read;
            }
            pContext->Message = pContext->RecvBuffer;
            pContext->ReceivedMessageBodyBytes = (uint32_t)body_length;

            /* Body received. Check handshake. */
            if (pContext->WaitingHandshakeMessage)
            {
                CheckHandshakeResult(pContext);
                // handshake check either posts handshake succeed, or close the connection,
                // so here we do not receive a real request.
                return false;
            }
            else
            {
                return true;
            }
        }

        void SendResponse(void* _pContext)
        {
            PerSocketContextObject * pContext = (PerSocketContextObject*)_pContext;
            char* buf = pContext->Message;
            do
            {
                ssize_t bytesSent = write(pContext->fd, buf, pContext->RemainingBytesToSend);
                if (-1 == bytesSent)
                {
                    Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "ServerSocket: Errors occur during network send: Error code = {0}, socket = {1}", errno, pContext->fd);
                    CloseClientConnection(pContext, false);
                }
                buf += bytesSent;
                pContext->RemainingBytesToSend -= bytesSent;
            } while (pContext->RemainingBytesToSend > 0);
            ResetContextObjects(pContext);
            RearmFD(pContext);
        }

        void CloseClientConnection(PerSocketContextObject* pContext, bool lingering)
        {
            RemovePerSocketContextObject(pContext->fd);
            //TODO lingering
            close(pContext->fd);
            FreePerSocketContextObject(pContext);
        }

    }
}
#endif
