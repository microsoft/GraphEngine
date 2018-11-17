// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "Events.h"
#include "Network/Network.h"
#include "Trinity/Diagnostics/Log.h"

class _handler_initializer
{
public:
    _handler_initializer()
    {
        (void)Trinity::Events::reset_handlers();
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

        work_t* _default_handler(message_t * buff)
        {
            buff->buf = (char *)malloc(sizeof(TrinityErrorCode));
            buff->len = sizeof(TrinityErrorCode);
            *(TrinityErrorCode *)buff->buf = TrinityErrorCode::E_RPC_EXCEPTION;
            return nullptr;
        }

        void _eventloop()
        {
            uint32_t          szwork;
            work_t*           pwork;
            Network::sock_t*  psock;

            while (true)
            {
                pwork = poll_work(OUT szwork);
                psock = pwork->psock;
                switch (pwork->type)
                {
                case worktype_t::Shutdown:
                    free_work(pwork);
                    return;
                case worktype_t::Receive:

                    if (!Network::process_recv(psock, szwork))
                    {
                        break;
                    }

                    if (psock->wait_handshake)
                    {
                        Network::check_handshake(psock);
                    }
                    else
                    {
                        message_t* msg = (message_t*)psock;
                        s_message_handlers[*(uint16_t *)msg->buf](msg);
                        Network::send_rsp(psock);
                    }

                    break;
                case worktype_t::Send:
                    if (Network::process_send(psock, szwork))
                    {
                        Network::reset_socket(psock);
                        Network::recv_async(psock);
                    }
                    else
                    {
                        Network::send_async(psock);
                    }
                    break;
                default:
                    Diagnostics::WriteLine(Diagnostics::Error, "EventLoop: Async work type {0} is not recognized.", pwork->type);
                    break;
                }
            }
        }

        void _workerthread()
        {
            auto tid = std::this_thread::get_id();
            Diagnostics::WriteLine(Diagnostics::Debug, "EventLoop: Starting worker thread #{0}", tid);
            ++g_threadpool_size;

            _eventloop();

            Diagnostics::WriteLine(Diagnostics::Debug, "EventLoop: Stopping worker thread #{0}", tid);
            --g_threadpool_size;
        }

        work_t* alloc_work(worktype_t work)
        {
            work_t* p = (work_t*)malloc(sizeof(work_t));
#if defined(TRINITY_PLATFORM_WINDOWS)
            memset(p, 0, sizeof(WSAOVERLAPPED));
#endif
            p->type = work;
            return p;
        }

        void free_work(work_t* p)
        {
            free(p);
        }

        void reset_work(work_t* p, worktype_t opType)
        {
#if defined(TRINITY_PLATFORM_WINDOWS)
            memset(p, 0, sizeof(WSAOVERLAPPED));
#endif
            p->type = opType;
        }


        TrinityErrorCode register_handler(uint16_t msg_type, message_handler_t * handler)
        {
            Diagnostics::WriteLine(Diagnostics::Debug, "EventLoop: Registering message handler, ID={0}, handler={1}", msg_type, (void*)handler);
            s_message_handlers[msg_type] = handler;
            return TrinityErrorCode::E_SUCCESS;
        }

        TrinityErrorCode reset_handlers()
        {
            Diagnostics::WriteLine(Diagnostics::Debug, "EventLoop: Resetting all message handlers");
            std::fill_n(s_message_handlers, MAX_HANDLERS_COUNT, &_default_handler);
            return TrinityErrorCode::E_SUCCESS;
        }

        TrinityErrorCode start_eventloop()
        {
            Diagnostics::WriteLine(Diagnostics::Info, "EventLoop: Starting.");

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
            catch (...)
            {
                Diagnostics::WriteLine(Diagnostics::Error, "EventLoop: failed to spawn worker threads");
                return TrinityErrorCode::E_FAILURE;
            }

            return TrinityErrorCode::E_SUCCESS;
        }

        TrinityErrorCode stop_eventloop()
        {
            Diagnostics::WriteLine(Diagnostics::Info, "EventLoop: Stopping.");
            return platform_stop_eventloop();
        }
    }
}

// exports
DLL_EXPORT TrinityErrorCode StartEventLoop() { return Trinity::Events::start_eventloop(); }
DLL_EXPORT TrinityErrorCode StopEventLoop() { return Trinity::Events::stop_eventloop(); }
DLL_EXPORT TrinityErrorCode RegisterMessageHandler(uint16_t msgId, void * handler) { return Trinity::Events::register_handler(msgId, (Trinity::Events::message_handler_t *)handler); }
DLL_EXPORT void LocalSendMessage(Trinity::Events::message_t* message) { Trinity::Events::s_message_handlers[*(uint16_t*)message->buf](message); }
