// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "Events.h"
#include "Network/Server/TrinityServer.h"
#include "Trinity/Diagnostics/Log.h"

class _handler_initializer
{
public:
    _handler_initializer()
    {
        (void)Trinity::Events::ResetMessageHandlers();
    }
};

static _handler_initializer s_hinit;

namespace Trinity
{
    namespace Events
    {
        constexpr size_t MAX_HANDLERS_COUNT = 65536;
        message_handler_t * s_message_handlers[MAX_HANDLERS_COUNT];
        std::atomic<size_t> g_threadpool_size;

        TrinityErrorCode platform_start_eventloop();
        TrinityErrorCode platform_stop_eventloop();

        void _default_handler(MessageBuff * buff)
        {
            buff->Buffer = (char *)malloc(sizeof(TrinityErrorCode));
            buff->BytesToSend = sizeof(TrinityErrorCode);
            *((TrinityErrorCode *)(buff->Buffer)) = TrinityErrorCode::E_RPC_EXCEPTION;
        }

        void _workerthread()
        {
            auto tid = std::this_thread::get_id();
            Diagnostics::WriteLine(Diagnostics::Debug, "EventLoop: Starting worker thread #{0}", tid);
            ++g_threadpool_size;
            do
            {
                void* _pContext = NULL;
                switch (AwaitRequest(_pContext))
                {
                case worktype_t::Shutdown:
                    break;
                case worktype_t::Receive:
                    MessageBuff* msg = (MessageBuff*)_pContext;
                    s_message_handlers[*(uint16_t *)msg->Buffer](msg);
                    Network::SendResponse(_pContext);
                    continue;
                }
            } while (false);
            Diagnostics::WriteLine(Diagnostics::Debug, "EventLoop: Stopping worker thread #{0}", tid);
            --g_threadpool_size;
        }

        TrinityErrorCode RegisterMessageHandler(uint16_t msg_type, message_handler_t * handler)
        {
            Diagnostics::WriteLine(Diagnostics::Debug, "EventLoop: Registering message handler, ID={0}, handler={1}", msg_type, (void*)handler);
            s_message_handlers[msg_type] = handler;
            return TrinityErrorCode::E_SUCCESS;
        }

        TrinityErrorCode ResetMessageHandlers()
        {
            Diagnostics::WriteLine(Diagnostics::Debug, "EventLoop: Resetting all message handlers");
            std::fill_n(s_message_handlers, MAX_HANDLERS_COUNT, &_default_handler);
            return TrinityErrorCode::E_SUCCESS;
        }

        TrinityErrorCode StartEventLoop()
        {
            g_threadpool_size = 0;
            auto errcode = platform_start_eventloop();
            if (errcode != TrinityErrorCode::E_SUCCESS)
            {
                return  errcode;
            }

            /// Without further profiling, the best overall maximum value to pick for the concurrency value is the number of processors.
            /// At the same time, a good rule of thumb is to have a minimum of twice as many threads in the thread pool as there are processors.
            int thread_count = std::thread::hardware_concurrency() * 2;

            try
            {
                for (auto i = 0; i < thread_count; ++i)
                {
                    std::thread(_workerthread).detach();
                }
            }
            catch(...)
            {
                Diagnostics::WriteLine(Diagnostics::Error, "EventLoop: failed to spawn worker threads");
                return TrinityErrorCode::E_FAILURE;
            }

            return TrinityErrorCode::E_SUCCESS;
        }

        TrinityErrorCode StopEventLoop()
        {
            return platform_stop_eventloop();
        }
    }
}

// exports
DLL_EXPORT TrinityErrorCode StartEventLoop() { return Trinity::Events::StartEventLoop(); }
DLL_EXPORT TrinityErrorCode StopEventLoop() { return Trinity::Events::StopEventLoop(); }
DLL_EXPORT TrinityErrorCode RegisterMessageHandler(uint16_t msgId, void * handler) { return Trinity::Events::RegisterMessageHandler(msgId, (Trinity::Events::message_handler_t *)handler); }
