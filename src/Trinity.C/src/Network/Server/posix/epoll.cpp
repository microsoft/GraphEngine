// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include <os/os.h>
#if defined(__linux__)
#include "TrinitySocketServer.h"
#include <sys/epoll.h>


namespace Trinity
{
    namespace Network
    {
        int sock_fd;
        int epoll_fd;
        std::thread socket_accept_thread;

        void AwaitRequest(void* &_pContext)
        {
            epoll_event ep_event;
            while (true)
            {
                if (1 == epoll_wait(epoll_fd, &ep_event, 1, -1))
                {
                    PerSocketContextObjectSlim* pContext = GetPerSocketContextObject(ep_event.data.fd);
                    if ((ep_event.events & EPOLLERR) || (ep_event.events & EPOLLHUP))
                    {
                        CloseClientConnection(pContext, false);
                        continue;
                    }
                    else if (ep_event.events & EPOLLIN)
                    {
                        if(pContext->WaitingHandshakeMessage)
                        {
                            CheckHandshakeResult(pContext);
                            continue;
                        }
                        _pContext = pContext;
                        ProcessRecv(pContext);
                        break;
                    }
                }
            }
        }

        void SocketAcceptThreadProc()
        {
            while (true)
            {
                int connected_sock_fd = AcceptConnection(sock_fd);
                if (-1 == connected_sock_fd)
                    continue;
                PerSocketContextObjectSlim * pContext = AllocatePerSocketContextObjectSlim(connected_sock_fd);
                AddPerSocketContextObject(pContext);
                epoll_event ep_event;
                ep_event.data.fd = connected_sock_fd;
                ep_event.events = EPOLLIN | EPOLLET | EPOLLONESHOT;
                if (-1 == epoll_ctl(epoll_fd, EPOLL_CTL_ADD, connected_sock_fd, &ep_event))
                {
                    fprintf(stderr, "cannot register connected sock fd to epoll instance");
                    RemovePerSocketContextObject(connected_sock_fd);
                    FreePerSocketContextObjectSlim(pContext);
                    close(connected_sock_fd);
                }
            }
        }

        int ShutdownSocketServer()
        {
            // join after close fd?
            socket_accept_thread.join();
            close(sock_fd);
            close(epoll_fd);
            return 0;
        }

        bool RearmFD(int fd)
        {
            epoll_event ep_event;
            ep_event.data.fd = fd;
            ep_event.events = EPOLLIN | EPOLLET | EPOLLONESHOT;
            return epoll_ctl(epoll_fd, EPOLL_CTL_MOD, fd, &ep_event) == 0;
        }

        int InitializeEventMonitor(int sockfd)
        {
            sock_fd = sockfd;
            epoll_fd = epoll_create1(0);
            if (-1 == epoll_fd)
            {
                return -1;
            }
            printf("epoll_fd: %d\n", epoll_fd);
            socket_accept_thread = std::thread(SocketAcceptThreadProc);
            return true;
        }
    }
}
#endif//platform
