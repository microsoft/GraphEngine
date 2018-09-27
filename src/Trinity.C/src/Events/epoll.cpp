// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "os/os.h"
#if defined(TRINITY_PLATFORM_LINUX)
#include "Network/Server/posix/TrinitySocketServer.h"
#include <sys/epoll.h>
#include <sys/eventfd.h>


namespace Trinity
{
    namespace Events
    {
        int epoll_fd;
        using Network::PerSocketContextObject;

        worktype_t AwaitRequest(void* &_pContext)
        {
            epoll_event ep_event;
            while (true)
            {
                switch (epoll_wait(epoll_fd, &ep_event, /*nr_events*/ 1, /*timeout*/ -1))
                {
                case 0:
                    break;
                case 1:
                    auto pContext = (PerSocketContextObject*)ep_event.data.ptr;
                    if (NULL == pContext)
                    {
                        /* Assume that epoll set is destroyed, and we're shutting down the server. */
                        _pContext = NULL;
                        /* Drain the evfd semaphore */
                        return worktype_t::Shutdown;
                    }

                    if ((ep_event.events & EPOLLERR) || (ep_event.events & EPOLLHUP))
                    {
                        Network::CloseClientConnection(pContext, false);
                        continue;
                    }
                    else if (ep_event.events & EPOLLIN)
                    {
                        if (Network::ProcessRecv(pContext))
                        {
                            _pContext = pContext;
                            return worktype_t::Receive;
                        }
                    }
                    break;
                case -1:
                default:
                    // error?
                    return worktype_t::Shutdown;
                }
            }
        }

        TrinityErrorCode platform_start_eventloop()
        {
            epoll_fd = epoll_create1(0);
            if (-1 == epoll_fd)
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "Cannot create epoll set: {0}", errno);
                return TrinityErrorCode::E_FAILURE;
            }
            return TrinityErrorCode::E_SUCCESS;
        }

        TrinityErrorCode platform_stop_eventloop()
        {
            int evfd = eventfd(0, EFD_NONBLOCK);
            epoll_event ep_event;
            ep_event.data.fd = evfd;
            ep_event.data.ptr = NULL;
            // use level-trigger so that everyone is woke up when the semaphore is not drained
            ep_event.events = EPOLLIN; 
            epoll_ctl(epoll_fd, EPOLL_CTL_ADD, evfd, &ep_event);

            uint32_t thread_cnt = g_threadpool_size;
            for (uint32_t i = 0; i < thread_cnt; ++i)
            {
                eventfd_write(evfd, 1);
            }
            while (g_threadpool_size > 0) { usleep(100000); }

            close(evfd);
            close(epoll_fd);

            return TrinityErrorCode::E_SUCCESS;
        }

        TrinityErrorCode EnterEventMonitor(PerSocketContextObject* pContext)
        {
            epoll_event ep_event;
            ep_event.data.ptr = pContext;
            ep_event.events = EPOLLIN | EPOLLET | EPOLLONESHOT;
            if (-1 == epoll_ctl(epoll_fd, EPOLL_CTL_ADD, pContext->fd, &ep_event))
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "Cannot register connected sock fd to epoll instance");
                CloseClientConnection(pContext, false);
                return TrinityErrorCode::E_FAILURE;
            }

            return TrinityErrorCode::E_SUCCESS;
        }

        bool RearmFD(PerSocketContextObject* pContext)
        {
            epoll_event ep_event;
            ep_event.data.fd = pContext->fd;
            ep_event.data.ptr = pContext;
            ep_event.events = EPOLLIN | EPOLLET | EPOLLONESHOT;
            return epoll_ctl(epoll_fd, EPOLL_CTL_MOD, pContext->fd, &ep_event) == 0;
        }
    }
}
#endif