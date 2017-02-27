// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include <os/os.h>
#if defined(__linux__)
#include "TrinitySocketServer.h"
#include <sys/epoll.h>
#include <sys/eventfd.h>


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
                    PerSocketContextObject* pContext = (PerSocketContextObject*)ep_event.data.ptr;
                    if (NULL == pContext)
                    {
                        /* Assume that epoll set is destroyed, and we're shutting down the server. */
                        _pContext = NULL;
                        /* Drain the evfd semaphore */
                        break;
                    }

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
            }
        }

        bool RearmFD(PerSocketContextObject* pContext)
        {
            epoll_event ep_event;
            ep_event.data.fd = pContext->fd;
            ep_event.data.ptr = pContext;
            ep_event.events = EPOLLIN | EPOLLET | EPOLLONESHOT;
            return epoll_ctl(epoll_fd, EPOLL_CTL_MOD, pContext->fd, &ep_event) == 0;
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
            uint32_t thread_cnt = g_threadpool_size;
            int evfd = eventfd(0, EFD_NONBLOCK);
            epoll_event ep_event;
            ep_event.data.fd = evfd;
            ep_event.data.ptr = NULL;
            // use level-trigger so that everyone is woke up when the semaphore is not drained
            ep_event.events = EPOLLIN; 
            epoll_ctl(epoll_fd, EPOLL_CTL_ADD, evfd, &ep_event);

            for (uint32_t i = 0; i < thread_cnt; ++i)
            {
                eventfd_write(evfd, 1);
            }
            while (g_threadpool_size > 0) { usleep(100000); }

            close(evfd);
            close(epoll_fd);
            return 0;
        }

        int EnterEventMonitor(PerSocketContextObject* pContext)
        {
            epoll_event ep_event;
            ep_event.data.fd = pContext->fd;
            ep_event.data.ptr = pContext;
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
