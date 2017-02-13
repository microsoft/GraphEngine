// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include <os/os.h>
#ifndef TRINITY_PLATFORM_WINDOWS
#include <thread>
#include <map>
#include <os/platforms/posix.h>
#include <Threading/TrinitySpinlock.h>
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
        TrinitySpinlock psco_spinlock; // psco = per socket context object
        std::map<int, PerSocketContextObjectSlim*> psco_map;

        void AddPerSocketContextObject(PerSocketContextObjectSlim * pContext)
        {
            psco_spinlock.lock();
            psco_map.insert(std::pair<int, PerSocketContextObjectSlim*>(pContext->fd, pContext));
            psco_spinlock.unlock();

        }
        void RemovePerSocketContextObject(int fd)
        {
            psco_spinlock.lock();
            psco_map.erase(fd);
            psco_spinlock.unlock();
        }

        PerSocketContextObjectSlim* GetPerSocketContextObject(int fd)
        {
            psco_spinlock.lock();
            PerSocketContextObjectSlim* pContext = psco_map[fd];
            psco_spinlock.unlock();
            return pContext;
        }

        int StartSocketServer(uint16_t port)
        {
            int sock_fd = -1;
            struct addrinfo hints, *addrinfos, *addrinfop;
            memset(&hints, 0, sizeof(struct addrinfo));
            hints.ai_family = AF_INET; 
            hints.ai_socktype = SOCK_STREAM;
            hints.ai_flags = AI_PASSIVE; // wildcard addresses
            char port_buf[6];
            sprintf(port_buf, "%u", port);
            int error_code = getaddrinfo(NULL, port_buf, &hints, &addrinfos);
            if (error_code != 0)
            {
                return -1;
            }

            for (addrinfop = addrinfos; addrinfop != NULL; addrinfop = addrinfop->ai_next)
            {
                sock_fd = socket(addrinfop->ai_family, addrinfop->ai_socktype,
                    addrinfop->ai_protocol);
                if (-1 == sock_fd)
                    continue;
                if (0 == bind(sock_fd, addrinfop->ai_addr, addrinfop->ai_addrlen))
                    break;
                close(sock_fd);
            }
            if (addrinfop == NULL)
            {
                return -1;
            }
            printf("sock_fd: %d\n", sock_fd);
            //make_nonblocking(sock_fd);
            freeaddrinfo(addrinfos);

            if (-1 == listen(sock_fd, SOMAXCONN))
            {
                printf("listen failed\n");
                return -1;
            }

            fprintf(stderr, "listen succeed\n");

            if (-1 == InitializeEventMonitor(sock_fd))
            {
                close(sock_fd);
                return -1;
            }

            return sock_fd;
        }

        //TODO stop server
        //TODO close connections on stop

        int AcceptConnection(int sock_fd)
        {
            fprintf(stderr, "waiting for connection ...\n");
            sockaddr addr;
            socklen_t addrlen = sizeof(sockaddr);
            return accept4(sock_fd, &addr, &addrlen, SOCK_NONBLOCK);
        }


        void WorkerThreadProc(int tid)
        {
            fprintf(stderr, "%d\n", tid);
            while (true)
            {
                void* _pContext;
                AwaitRequest(_pContext);
                PerSocketContextObjectSlim* pContext = (PerSocketContextObjectSlim*)_pContext;
                MessageHandler((MessageBuff*)pContext);
                SendResponse(pContext);
            }
        }

        bool ProcessRecv(PerSocketContextObjectSlim* pContext)
        {
            fprintf(stderr, "process recv on socket fd %d ...\n", pContext->fd);
            int fd = pContext->fd;
            uint32_t body_length;

            uint32_t bytes_left = UInt32_Contants::MessagePrefixLength;
            uint32_t p = 0;
            while (bytes_left > 0)
            {
                ssize_t bytes_read = read(fd, ((char*)&body_length) + p, bytes_left);
                p += bytes_read;
                bytes_left -= bytes_read;
            }

            char * buf = pContext->RecvBuffer;
            if (body_length > pContext->RecvBufferLen)
            {
                pContext->RecvBuffer = (char*)realloc(pContext->RecvBuffer, body_length);
                pContext->RecvBufferLen = body_length;
            }

            bytes_left = body_length;
            p = 0;
            while (bytes_left > 0)
            {
                ssize_t bytes_read = read(fd, buf + p, bytes_left);
                if (bytes_read == 0 || (bytes_read == -1 && EAGAIN != errno))
                {
                    // errors occurred
                    CloseClientConnection(pContext, false);
                    return false;
                }
                p += bytes_read;
                bytes_left -= bytes_read;
            }
            pContext->Message = buf;
            pContext->ReceivedMessageBodyBytes = body_length;
            return true;
        }

        void SendResponse(void* _pContext)
        {
            PerSocketContextObjectSlim * pContext = (PerSocketContextObjectSlim*)_pContext;
            RearmFD(pContext->fd);
            fprintf(stderr, "send response, length: %d\n", pContext->RemainingBytesToSend);
            write(pContext->fd, pContext->Message, pContext->RemainingBytesToSend);
            ResetContextObjects(pContext);
        }
    }
}
#endif
