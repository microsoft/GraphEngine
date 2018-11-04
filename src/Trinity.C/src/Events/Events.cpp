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
        TrinityErrorCode platform_post_workitem(work_t*);
        TrinityErrorCode platform_poll(work_t*&, uint32_t&);

        void _default_handler(message_t * buff)
        {
            buff->buf = (char *)malloc(sizeof(TrinityErrorCode));
            buff->len = sizeof(TrinityErrorCode);
            *(TrinityErrorCode *)buff->buf = TrinityErrorCode::E_RPC_EXCEPTION;
        }

        const char* _sock_incoming_err(TrinityErrorCode eresult)
        {
            switch (eresult)
            {
            case TrinityErrorCode::E_NOENTRY:
                return "Network: Empty response, sock_t = {0}";
            case TrinityErrorCode::E_INIT_FAIL:
                return "Network: Incorrect client handshake sequence, sock_t = {0}";
            case TrinityErrorCode::E_RPC_EXCEPTION:
                return "Network: Incorrect client request header. sock_t = {0}";
            case TrinityErrorCode::E_NOMEM:
                return "Network: Cannot allocate memory during message receiving. sock_t = {0}";
            case TrinityErrorCode::E_MSG_OVERFLOW:
                return "Network: Receiving more than a complete message. sock_t = {0}";
            default:
                return nullptr;
            }
        }

        bool _sock_check_error(TrinityErrorCode eresult, Network::sock_t* psock)
        {
            if (TrinityErrorCode::E_SUCCESS == eresult)
            {
                return false;
            }

            if (TrinityErrorCode::E_RETRY == eresult)
            {
                return true;
            }

            if (psock->is_incoming)
            {
                auto emsg = _sock_incoming_err(eresult);
                if (emsg != nullptr)
                {
                    Diagnostics::WriteLine(Diagnostics::LogLevel::Error, emsg, psock);
                }

                close_incoming_conn(psock, false);
            }
            else
            {

            }

            return true;
        }

        void _post_continuation(work_t* pcont, work_t* pwait)
        {
            assert(pcont->pcontinuation != nullptr);

            // chain it together so that the execution order is:
            // ?dep -> continuation -> send_rsp(psock)
            pcont->pwait_chain = pwait;
            work_t* pexecute;

            if (pcont->pdependency != nullptr)
            {
                // post pdependency. the continuation will
                // wait for the dependency to complete.
                pcont->pdependency->pwait_chain = pcont;
                pexecute = pcont->pdependency;
            }
            else
            {
                // directly post the continuation.
                pexecute = pcont;
                platform_post_workitem(pcont);
            }

            switch (pexecute->type)
            {
            case worktype_t::Send:
                send_rsp(pexecute->psock);
                break;
            }
        }

        void _eventloop()
        {
            uint32_t          szwork;
            work_t*           pwork;
            work_t*           pcont;
            Network::sock_t*  psock;
            TrinityErrorCode  eresult;

            while (true)
            {
                //  1. work polling
                while (TrinityErrorCode::E_SUCCESS != platform_poll(pwork, szwork))
                {
                    // loop
                }

                //  2. work processing
                switch (pwork->type)
                {
                case worktype_t::Shutdown:
                    assert(pwork->pcontinuation == nullptr);
                    free_work(pwork);
                    return;
                case worktype_t::Receive:

                    psock = pwork->psock;
                    eresult = Network::process_recv(psock, szwork);

                    if (_sock_check_error(eresult, psock))
                    {
                        continue;
                    }

                    if (psock->is_incoming)
                    {
                        //  the definitive behavior after incoming recv is handshake/messageproc,
                        //  so it cannot have a continuation.
                        assert(pwork->pcontinuation == nullptr);
                        if (psock->wait_handshake)
                        {
                            eresult = Network::check_handshake(psock);
                        }
                        else
                        {
                            message_t* msg = (message_t*)psock;
                            pcont = s_message_handlers[*(uint16_t *)msg->buf](msg);
                            if (pcont != nullptr)
                            {
                                // continue with sending the response
                                pwork->type = worktype_t::Send;
                                _post_continuation(pcont, pwork);
                                continue;
                            }
                            else
                            {
                                eresult = Network::send_rsp(psock);
                            }
                        }
                    }
                    else /* is_outgoing */
                    {

                    }

                    if (_sock_check_error(eresult, psock))
                    {
                        continue;
                    }

                    // breaks to continuation handling
                    break;
                case worktype_t::Send:

                    psock = pwork->psock;
                    eresult = Network::process_send(psock, szwork);

                    if (_sock_check_error(eresult, psock))
                    {
                        continue;
                    }

                    if (psock->is_incoming)
                    {
                        assert(pwork->pcontinuation == nullptr);
                        Network::reset_incoming_socket(psock);
                        eresult = Network::recv_async(psock);
                    }
                    else /* is_outgoing */
                    {

                    }

                    if (_sock_check_error(eresult, psock))
                    {
                        continue;
                    }

                    // breaks to continuation handling
                    break;
                case worktype_t::Compute:

                    pcont = pwork->pcompute(pwork->pcompute_data);
                    if (pcont != nullptr)
                    {
                        _post_continuation(pcont, pwork->pwait_chain);
                        continue;
                    }

                    // breaks to continuation handling
                    break;
                case worktype_t::Continuation:
                    pcont = pwork->pcontinuation(pwork->pcontinuation_data, pwork->pdependency);
                    if (pcont != nullptr)
                    {
                        _post_continuation(pcont, pwork->pwait_chain);
                        continue;
                    }

                    // breaks to continuation handling
                    break;
                default:
                    Diagnostics::WriteLine(Diagnostics::Error, "EventLoop: Async work type {0} is not recognized.", pwork->type);
                    continue;
                }

                //  3. wait chain processing
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
            reset_work(p, work);
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
            p->pcontinuation = nullptr;
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
DLL_EXPORT TrinityErrorCode StartEventLoop()
{
    return Trinity::Events::start_eventloop();
}

DLL_EXPORT TrinityErrorCode StopEventLoop()
{
    return Trinity::Events::stop_eventloop();
}

DLL_EXPORT TrinityErrorCode RegisterMessageHandler(uint16_t msgId, void * handler)
{
    return Trinity::Events::register_handler(msgId, (Trinity::Events::message_handler_t *)handler);
}

DLL_EXPORT void LocalSendMessage(Trinity::Events::message_t* message)
{
    Trinity::Events::s_message_handlers[*(uint16_t*)message->buf](message);
}

DLL_EXPORT TrinityErrorCode PostCompute(Trinity::Events::compute_handler_t* pcompute, void* pdata)
{
    auto pwork = Trinity::Events::alloc_work(Trinity::Events::worktype_t::Compute);
    pwork->pcompute      = pcompute;
    pwork->pcompute_data = pdata;

    return Trinity::Events::platform_post_workitem(pwork);
}
