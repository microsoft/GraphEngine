// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "os/os.h"
#if defined(TRINITY_PLATFORM_LINUX)
#include "Network/Network.h"
#include "Events/Events.h"
#include "Trinity/Diagnostics/Log.h"

#include <sys/epoll.h>
#include <sys/eventfd.h>

#include <queue>
#include <mutex>

namespace Trinity
{
    namespace Events
    {
        int epoll_fd;

        class epoll_compute_queue_t
        {
            work_t* pmaster_work;
            std::mutex lock;
            std::queue<work_t*> work_queue;
            int evfd;
            epoll_event epevent;

        public:
            epoll_compute_queue_t()
            {
                pmaster_work = alloc_work(worktype_t::Compute);
                pmaster_work->pcompute_data = this;

                // use semaphore semantic (each read decrements the counter by 1)
                evfd = eventfd(0, EFD_NONBLOCK | EFD_SEMAPHORE);
                epevent.data.ptr = pmaster_work;
                epevent.events = EPOLLET | EPOLLONESHOT;
                epoll_ctl(epoll_fd, EPOLL_CTL_ADD, evfd, &epevent);
            }

            void enqueue(work_t* pwork)
            {
                {
                    std::lock_guard<std::mutex> g(lock);
                    work_queue.push(pwork);
                }
                eventfd_write(evfd, 1);
            }

            work_t* dequeue()
            {
                work_t* pwork;
                {
                    std::lock_guard<std::mutex> g(lock);
                    pwork = work_queue.front();
                    work_queue.pop();
                }
                //  consume the signal
                eventfd_t _unused;
                eventfd_read(evfd, &_unused);
                //  rearm the queue
                epoll_ctl(epoll_fd, EPOLL_CTL_MOD, evfd, &epevent);

                return pwork;
            }

            ~epoll_compute_queue_t()
            {
                while (work_queue.size())
                {
                    //TODO finish the work?
                    free_work(work_queue.front());
                    work_queue.pop();
                }
                close(evfd);
                free_work(pmaster_work);
            }
        };

        using Network::sock_t;
        std::vector<std::unique_ptr<epoll_compute_queue_t>> compute_queues;
        size_t compute_queue_idx = 0;

        TrinityErrorCode platform_poll(OUT work_t* &pwork, OUT uint32_t &szwork)
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
                    /* do not drain the evfd semaphore */
                    break;
                }

                if (pwork->type == worktype_t::Compute)
                {
                    pwork = reinterpret_cast<epoll_compute_queue_t*>(pwork->pcompute_data)->dequeue();
                    return TrinityErrorCode::E_SUCCESS;
                }

                psock = pwork->psock;

                if ((ep_event.events & EPOLLERR) || (ep_event.events & EPOLLHUP))
                {
                    Network::close_conn(psock, false);
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

        TrinityErrorCode platform_start_eventloop()
        {
            epoll_fd = epoll_create1(0);
            if (-1 == epoll_fd)
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "Cannot create epoll set: {0}", errno);
                return TrinityErrorCode::E_FAILURE;
            }

            for (int i=0; i < std::thread::hardware_concurrency(); ++i)
            {
                compute_queues.push_back(std::make_unique<epoll_compute_queue_t>());
            }

            return TrinityErrorCode::E_SUCCESS;
        }

        TrinityErrorCode platform_stop_eventloop()
        {
            int evfd = eventfd(0, EFD_NONBLOCK);
            epoll_event ep_event;
            ep_event.data.ptr = NULL;
            // use level-trigger so that everyone is woke up when the semaphore is not drained
            ep_event.events = EPOLLIN;
            epoll_ctl(epoll_fd, EPOLL_CTL_ADD, evfd, &ep_event);

            eventfd_write(evfd, g_threadpool_size);
            while (g_threadpool_size > 0) { usleep(100000); }

            compute_queues.clear();

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
                Network::close_conn(pContext, false);
                return TrinityErrorCode::E_FAILURE;
            }

            return TrinityErrorCode::E_SUCCESS;
        }

        TrinityErrorCode platform_post_workitem(work_t* pwork)
        {
            auto idx = (compute_queue_idx++) % compute_queues.size();
            compute_queues[idx]->enqueue(pwork);
            return TrinityErrorCode::E_SUCCESS;
        }
    }
}
#endif