// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "os/os.h"
#if defined(TRINITY_PLATFORM_LINUX)
#include "Network.h"
#include "Events/Events.h"
#include "SocketOptionsHelper.h"

#include <sys/epoll.h>
#include <sys/eventfd.h>

namespace Trinity
{
    namespace Events
    {
        extern int epoll_fd;
    }
    namespace Network
    {
        TrinityErrorCode recv_async(sock_t* p)
        {
            // rearm fd to listen EPOLLIN
            p->work->type = Events::worktype_t::Receive;
            epoll_event ep_event;
            ep_event.data.ptr = p->work;
            ep_event.events = EPOLLIN | EPOLLET | EPOLLONESHOT;
            if (-1 == epoll_ctl(Events::epoll_fd, EPOLL_CTL_MOD, p->socket, &ep_event))
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "Network: Errors occur during recv_async epoll_ctl. Error code = {0}", GetLastError());
                return TrinityErrorCode::E_NETWORK_RECV_FAILURE;
            }

            return TrinityErrorCode::E_RETRY;
        }

        TrinityErrorCode send_async(sock_t* p)
        {
            // rearm fd to listen EPOLLOUT
            p->work->type = Events::worktype_t::Send;
            epoll_event ep_event;
            ep_event.data.ptr = p->work;
            ep_event.events = EPOLLOUT | EPOLLET | EPOLLONESHOT;
            if (-1 == epoll_ctl(Events::epoll_fd, EPOLL_CTL_MOD, p->socket, &ep_event))
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "Network: Errors occur during send_async epoll_ctl. Error code = {0}", GetLastError());
                return TrinityErrorCode::E_NETWORK_SEND_FAILURE;
            }

            return TrinityErrorCode::E_RETRY;
        }
    }
}
#endif