// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include "TrinityCommon.h"
#include <cstdint>
#include <cstdlib>
#include <thread>
#include <atomic>

#if defined(TRINITY_PLATFORM_WINDOWS)
#include <winsock2.h>
#endif

namespace Trinity
{
    namespace Network
    {
        struct sock_t;
    }
    namespace Events
    {
        enum worktype_t : uint32_t
        {
            Receive,
            Send,
            Shutdown,
            Compute,
            Continuation,
        };

        // Forward definitions
        struct message_t;
        struct work_t;

        typedef work_t*(message_handler_t)(message_t* pmessage);
        typedef work_t*(compute_handler_t)(void* pdata);
        typedef work_t*(continuation_handler_t)(void* pdata, work_t* pcompleted);

        // This is for data exchange between Events subsystem and message handlers.
        struct message_t
        {
            // RECV: allocated after receiving the message header
            // SEND: allocated by a message handler
            char*    buf;
            uint32_t len;
        };

        struct work_t
        {
#if defined(TRINITY_PLATFORM_WINDOWS)
            WSAOVERLAPPED Overlapped;
#endif
            /**
             * record the work type when issuing an async op,
             * e.g., Send, Recv, Wakeup, etc.
             */
            worktype_t type; // size: 4

            union
            {
                struct
                {
                    Network::sock_t* psock;
                    bool             bsock_incoming;
                };
                struct
                {
                    compute_handler_t* pcompute;
                    void*              pcompute_data;
                };
                struct
                {
                    //  executed when dependency is completed (or null)
                    continuation_handler_t* pcontinuation;
                    //  associated data for pcontinuation
                    void* pcontinuation_data;
                    //  when non-null, should be executed before pcontinuation
                    work_t* pdependency;
                };
            };

            //  when non-null, should be executed after the current work.
            work_t* pwait_chain;
        };

        extern std::atomic<size_t> g_threadpool_size;

        work_t* alloc_work(worktype_t work);
        void    free_work(work_t* p);
        void    reset_work(work_t* p, worktype_t work);

        TrinityErrorCode reset_handlers();
        TrinityErrorCode register_handler(uint16_t msgId, message_handler_t * handler);
        TrinityErrorCode start_eventloop();
        TrinityErrorCode stop_eventloop();
        TrinityErrorCode enter_eventloop(Network::sock_t *psock);
    }
}
