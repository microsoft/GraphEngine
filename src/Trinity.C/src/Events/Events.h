// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include "TrinityCommon.h"
#include "Trinity/Events.h"
#include "Network/Network.h"
#include <cstdint>
#include <cstdlib>
#include <thread>
#include <atomic>

namespace Trinity
{
    namespace Events
    {
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
