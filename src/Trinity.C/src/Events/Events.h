// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <stdint.h>
#include <cstdlib>
#include <thread>
#include <atomic>
#include "TrinityCommon.h"

namespace Trinity
{
    namespace Network
    {
        struct PerSocketContextObject;
    }
    namespace Events
    {
        typedef struct // This is for data exchange between unmanaged and managed worlds
        {
            char* Buffer; // allocate after receiving the message header
            uint32_t BytesReceived;
            uint32_t BytesToSend;
        }MessageBuff;

        enum worktype_t : uint32_t
        {
            None,
            Receive,
            Send,
            Shutdown,
        };

        typedef void(message_handler_t)(MessageBuff *);

        extern std::atomic<size_t> g_threadpool_size;

        worktype_t AwaitRequest(void* &pContext);
        TrinityErrorCode ResetMessageHandlers();
        TrinityErrorCode RegisterMessageHandler(uint16_t msgId, message_handler_t * handler);
        TrinityErrorCode StartEventLoop();
        TrinityErrorCode StopEventLoop();
        TrinityErrorCode EnterEventMonitor(Network::PerSocketContextObject* key);
    }
}