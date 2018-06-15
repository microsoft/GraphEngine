// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <stdint.h>
#include <cstdlib>
#include <thread>
#include <algorithm>
#include <atomic>
#include "TrinityCommon.h"
#include "Network/Network.h"
#include "Network/ProtocolConstants.h"

namespace Trinity
{
    namespace Network
    {
        typedef struct // This is for data exchange between unmanaged and managed worlds
        {
            char* Buffer; // allocate after receiving the message header
            uint32_t BytesReceived;
            uint32_t BytesToSend;
        }MessageBuff;

		typedef void(message_handler_t)(MessageBuff *);
		
		constexpr size_t MAX_HANDLERS_COUNT = 65535;
		static message_handler_t * handlers[MAX_HANDLERS_COUNT];
		extern std::atomic_size_t handlers_count;

		bool register_message_handler (uint16_t msgId, message_handler_t * handler);

		void dispatch_message(MessageBuff *);

        // Should be defined in an implementation.
        extern std::atomic<size_t> g_threadpool_size;
        // Should be defined in an implementation.
        struct PerSocketContextObject;

        int StartSocketServer(uint16_t port);

        int ShutdownSocketServer();

        void AwaitRequest(void* &pContext);

        void SendResponse(void* _pContext);

        inline void EnterSocketServerThreadPool()
        {
            ++g_threadpool_size;
        }

        inline void ExitSocketServerThreadPool()
        {
            --g_threadpool_size;
        }

        void WorkerThreadProc(int tid);

        void MessageHandler(MessageBuff * msg);

        void CheckHandshakeResult(PerSocketContextObject* pContext);

        bool StartWorkerThreadPool(); //

#pragma region Test purpose only
        int TrinityServerTestEntry();
#pragma endregion
    }
}
