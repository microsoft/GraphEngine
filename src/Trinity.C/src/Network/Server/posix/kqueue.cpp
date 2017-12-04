// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include <os/os.h>
#if ((!defined(__linux__)) && (defined(__unix__) || (defined(__APPLE__) && defined(__MACH__))))
#include "Network/ProtocolConstants.h"
#include "TrinitySocketServer.h"
#include <sys/event.h>

namespace Trinity
{
    namespace Network
    {
        int kqueue_fd;

        void AwaitRequest(void* &_pContext)
        {
            struct kevent kevt_out;
            while (true)
            {
                if (1 == kevent(kqueue_fd, NULL, 0, &kevt_out, 1, NULL))
                {
                    PerSocketContextObject* pContext = GetPerSocketContextObject(kevt_out.ident);
                    if ((kevt_out.flags & EV_EOF) || (kevt_out.flags & EV_ERROR))
                    {
                        struct kevent kevt;
                        EV_SET(&kevt, kevt_out.ident, EVFILT_READ, EV_DELETE, 0, 0, NULL);
                        if (-1 == kevent(kqueue_fd, &kevt, 1, NULL, 0, NULL))
                        {
                            fprintf(stderr, "cannot delete fd from kqueue");
                        }
                        ClientCloseConnection(kevt_out.ident, false);
                    }
                    else if (kevt_out.flags & EVFILT_READ)
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
                    /* Assume that kqueue is destroyed, and we're shutting down the server. */
                    _pContext = NULL;
                    break;
                }
            }
        }

        bool RearmFD(PerSocketContextObject* pContext)
        {
            int fd = pContext->fd;
            struct kevent kevt;
            EV_SET(&kevt, fd, EVFILT_READ, EV_ENABLE, 0, 0, NULL);
            return -1 != kevent(kqueue_fd, &kevt, 1, NULL, 0, NULL);
        }

        int InitializeEventMonitor()
        {
            kqueue_fd = kqueue();
            if (-1 == kqueue_fd)
            {
                return -1;
            }
            fprintf(stderr, "kqueue: %d\n", kqueue_fd);
            return 0;
        }

        int UninitializeEventMonitor()
        {
            //TODO
            close(kqueue_fd);
            return 0;
        }

        int EnterEventMonitor(PerSocketContextObject* pContext)
        {
            struct kevent kevt;
            EV_SET(&kevt, pContext->fd, EVFILT_READ, EV_ADD | EV_ENABLE | EV_DISPATCH, 0, 0, NULL);
            if (-1 == kevent(kqueue_fd, &kevt, 1, NULL, 0, NULL))
            {
                fprintf(stderr, "cannot register fd to kqueue\n");
                CloseClientConnection(pContext, false);
                return -1;
            }
            return 0;
        }
    }
}
#endif//platform
