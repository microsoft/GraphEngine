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
        int epoll_fd;

        void AwaitRequest(void* &_pContext)
        {
            epoll_event ep_event;
            while (true)
            {
                if (1 == epoll_wait(epoll_fd, &ep_event, /*nr_events*/ 1, /*timeout*/ -1))
                {
                    PerSocketContextObject* pContext = GetPerSocketContextObject(ep_event.data.fd);
                    if ((ep_event.events & EPOLLERR) || (ep_event.events & EPOLLHUP))
                    {
                        CloseClientConnection(pContext, false);
                        continue;
                    }
                    else if (ep_event.events & EPOLLIN)
                    {
                        if (ProcessRecv(pContext))
                        {
                            _pContext = pContext;
                            break;
                        }
                    }
                }
                else
                {
                    /* Assume that epoll set is destroyed, and we're shutting down the server. */
                    _pContext = NULL;
                    break;
                }
            }
        }

        bool RearmFD(int fd)
        {
            epoll_event ep_event;
            ep_event.data.fd = fd;
            ep_event.events = EPOLLIN | EPOLLET | EPOLLONESHOT;
            return epoll_ctl(epoll_fd, EPOLL_CTL_MOD, fd, &ep_event) == 0;
        }

        int InitializeEventMonitor()
        {
            epoll_fd = epoll_create1(0);
            if (-1 == epoll_fd)
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "Cannot create epoll set: {0}", errno);
                return -1;
            }
            return 0;
        }

        int UninitializeEventMonitor()
        {
            //TODO
            close(epoll_fd);
            return 0;
        }

        int EnterEventMonitor(PerSocketContextObject* pContext)
        {
            epoll_event ep_event;
            ep_event.data.fd = pContext->fd;
            ep_event.events = EPOLLIN | EPOLLET | EPOLLONESHOT;
            if (-1 == epoll_ctl(epoll_fd, EPOLL_CTL_ADD, pContext->fd, &ep_event))
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "Cannot register connected sock fd to epoll instance");
                CloseClientConnection(pContext, false);
                return -1;
            }

            return 0;
        }
    }
}
#endif//platform
