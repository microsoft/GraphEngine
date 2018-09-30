// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "os/os.h"
#if defined(TRINITY_PLATFORM_LINUX)
#include "Network/Network.h"
#include "Trinity/Diagnostics/Log.h"

#include <sys/epoll.h>
#include <sys/eventfd.h>


namespace Trinity
{
    namespace Events
    {
        int epoll_fd;
        using Network::sock_t;

        TrinityErrorCode await_epoll(OUT work_t* &pwork, OUT uint32_t &szwork)
        {
            epoll_event ep_event;
            sock_t* psock;
            int epoll_n = epoll_wait(epoll_fd, &ep_event, /*nr_events*/ 1, /*timeout*/ -1);

            switch (epoll_n)
            {
            case 0:
                return TrinityErrorCode::E_FAILURE;
            case 1:
                pwork = (work_t*)ep_event.data.ptr;
                /**
                 * Receiving nullptr means pulsing evfd, 
                 * and we're shutting down the server. 
                 */
                if (nullptr == pwork)
                {
                    /* Drain the evfd semaphore */
                    break;
                }

                psock = pwork->psock;

                if ((ep_event.events & EPOLLERR) || (ep_event.events & EPOLLHUP))
                {
                    Network::close_client_conn(psock, false);
                    return TrinityErrorCode::E_FAILURE;
                }
                else if ((ep_event.events & EPOLLIN) && pwork->type == worktype_t::Receive)
                {
                    szwork = read(psock->socket, psock->msg_buf, psock->pending_len);
                    return TrinityErrorCode::E_SUCCESS;
                }
                else if ((ep_event.events & EPOLLOUT) && pwork->type == worktype_t::Send)
                {
                    szwork = write(psock->socket, psock->msg_buf, psock->pending_len);
                    return TrinityErrorCode::E_SUCCESS;
                }
                else
                {
                    /**
                     * The ET event type does not match what we expect.
                     * e.g. EPOLLIIN when we are doing worktype_t::Send.
                     * In this case, we simply ignore this event -- further
                     * such events are disabled because of EPOLLONESHOT,
                     * and will be enabled again when we rearm the socket
                     * with a matching event type.
                     */
                    return TrinityErrorCode::E_FAILURE;
                }
            }
            // FALLTHROUGH default: report shutdown event
            pwork = alloc_work(worktype_t::Shutdown);
            return TrinityErrorCode::E_SUCCESS;
        }

        work_t* poll_work(OUT uint32_t& szwork)
        {
            work_t* ret;
            while (true)
            {
                if (TrinityErrorCode::E_SUCCESS == await_epoll(ret, szwork))
                {
                    return ret;
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

        TrinityErrorCode enter_eventloop(sock_t* pContext)
        {
            epoll_event ep_event;
            ep_event.data.ptr = pContext;
            ep_event.events = EPOLLET | EPOLLONESHOT;
            if (-1 == epoll_ctl(epoll_fd, EPOLL_CTL_ADD, pContext->socket, &ep_event))
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "Cannot register connected sock fd to epoll instance");
                Network::close_client_conn(pContext, false);
                return TrinityErrorCode::E_FAILURE;
            }

            return TrinityErrorCode::E_SUCCESS;
        }
    }
}
#endif