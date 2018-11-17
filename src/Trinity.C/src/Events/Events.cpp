// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "Events.h"
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

        work_t* _default_handler(message_t * buff)
        {
            buff->buf = (char *)malloc(sizeof(TrinityErrorCode));
            buff->len = sizeof(TrinityErrorCode);
            *(TrinityErrorCode *)buff->buf = TrinityErrorCode::E_RPC_EXCEPTION;
            return nullptr;
        }

        const char* _sock_err(TrinityErrorCode eresult, bool is_incoming)
        {
            switch (eresult)
            {
            case TrinityErrorCode::E_NOENTRY:
                return "Network: Empty response";
            case TrinityErrorCode::E_INIT_FAIL:
                if (is_incoming) { return "Network: Incorrect client handshake sequence."; }
                else { return "Network: Target host rejected handshake request message."; }
            case TrinityErrorCode::E_RPC_EXCEPTION:
                if (is_incoming) { return "Network: Incorrect client request header."; }
            case TrinityErrorCode::E_NOMEM:
                return "Network: Cannot allocate memory for sock_t.";
            case TrinityErrorCode::E_MSG_OVERFLOW:
                return "Network: Receiving more than a complete message.";
            }
            return nullptr;
        }

        // return true if further execution should be prevented
        bool _work_check_error(TrinityErrorCode eresult, work_t* pwork)
        {
            if (TrinityErrorCode::E_SUCCESS == eresult)
            {
                return false;
            }

            if (TrinityErrorCode::E_RETRY == eresult)
            {
                return true;
            }

            if (pwork->type == worktype_t::Receive || pwork->type == worktype_t::Send)
            {
                auto emsg = _sock_err(eresult, pwork->psock->is_incoming);
                if (emsg != nullptr)
                {
                    Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "{0} sock_t = {1}, thread id = {2}", emsg, pwork->psock, std::this_thread::get_id());
                }

                if (pwork->psock->is_incoming)
                {

                    close_conn(pwork->psock, false);
                }
                else
                {
                    // E_RPC_EXCEPTION is recoverable, do not close the connection
                    if (eresult != TrinityErrorCode::E_RPC_EXCEPTION)
                    {
                        close_conn(pwork->psock, false);
                    }
                    pwork->esock_op = eresult;
                    // outgoing conn. transmission failed.
                    // we should now wake up the wait chain, so
                    // that the failure is propagated.
                    return false;
                }
            }
            else if (pwork->type == worktype_t::Compute || pwork->type == worktype_t::Continuation)
            {
                // queue failed. cleanup.
                free_work(pwork);
            }

            return true;
        }

        // after each _post_work, call _work_check_error to clean up errors
        TrinityErrorCode _post_work(work_t* pwork)
        {
            switch (pwork->type)
            {
            case worktype_t::Send:
                return Network::send_message(pwork->psock);
            case worktype_t::Receive:
                return Network::recv_message(pwork->psock);
            case worktype_t::Compute: /* FALLTHROUGH */
            case worktype_t::Continuation:
            {
                // TODO calculate when to execute the work in-place
                // for now we just queue the work.
                auto eresult = platform_post_workitem(pwork);
                if (TrinityErrorCode::E_SUCCESS == eresult)
                {
                    // computation successfully queued
                    return TrinityErrorCode::E_RETRY;
                }
                else
                {
                    return eresult;
                }
            }
            default:
                return TrinityErrorCode::E_INVALID_ARGUMENTS;
            }
        }

        void _post_continuation(work_t* pcont, work_t* pwait)
        {
            assert(pcont->type == worktype_t::Continuation);
            assert(pcont->pcontinuation != nullptr);
            assert(pcont->pwait_chain == nullptr);

            // chain it together so that the execution order is:
            // ?dep -> continuation -> ?pwait
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
            }

            auto eresult = _post_work(pexecute);
            _work_check_error(eresult, pexecute);
            // execution underway (or failed, and then cleaned up).
            // go back to drive the eventloop...
        }

#define __try_post_continuation(C, W, E)         \
    pcont = (C);                                 \
    if (pcont != nullptr)                        \
    {                                            \
        _post_continuation(pcont, W);            \
        continue;                                \
    }                                            \
    else                                         \
    {                                            \
        E                                        \
    }                                            \

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
                while (TrinityErrorCode::E_SUCCESS != platform_poll(pwork, szwork)) { /* loop */ }
                work_t* pwait = pwork->pwait_chain;

                //  2. work processing
                switch (pwork->type)
                {
                case worktype_t::Shutdown:
                    assert(pwait == nullptr);
                    free_work(pwork);
                    return;
                case worktype_t::Receive:

                    psock = pwork->psock;
                    eresult = Network::process_recv(psock, szwork);

                    if (_work_check_error(eresult, pwork))
                    {
                        continue;
                    }

                    if (!psock->is_incoming)
                    {
                        assert(pwait != nullptr);
                        break;
                    }

                    /*  is_incoming is true  */
                    //  the definitive behavior after incoming recv is handshake/messageproc,
                    //  so it cannot have a wait chain.
                    assert(pwait == nullptr);
                    if (psock->wait_handshake)
                    {
                        eresult = Network::check_handshake(psock);
                    }
                    else
                    {
                        // switch to send mode
                        pwork->type = worktype_t::Send;
                        message_t* msg = (message_t*)psock;
                        __try_post_continuation(
                            s_message_handlers[*(uint16_t *)msg->buf](msg),
                            /* continue with sending the response */
                            pwork,
                            { eresult = _post_work(pwork); }
                        );
                    }

                    if (_work_check_error(eresult, pwork))
                    {
                        continue;
                    }

                    // breaks to continuation handling
                    break;
                case worktype_t::Send:

                    psock = pwork->psock;
                    eresult = Network::process_send(psock, szwork);

                    if (_work_check_error(eresult, pwork))
                    {
                        continue;
                    }

                    if (psock->is_incoming)
                    {
                        assert(pwait == nullptr);
                        pwork->type = worktype_t::Receive;
                        eresult = _post_work(pwork);
                    }
                    else /* is_outgoing */
                    {
                        // outgoing send finished.
                        // just wait for pwait_chain to run.
                        assert(pwait != nullptr);
                    }

                    if (_work_check_error(eresult, pwork))
                    {
                        continue;
                    }

                    // breaks to continuation handling
                    break;
                case worktype_t::Compute:
                    __try_post_continuation(
                        pwork->pcompute(pwork->pcompute_data),
                        pwork->pwait_chain,
                        {
                            // no new continuation posted.
                            // computation terminated.
                            eresult = TrinityErrorCode::E_SUCCESS;
                        }
                    );

                    break;
                case worktype_t::Continuation:
                    __try_post_continuation(
                        pwork->pcontinuation(pwork->pcontinuation_data, pwork->pdependency),
                        pwork->pwait_chain,
                        {
                            // no new continuation posted.
                            // computation terminated.
                            eresult = TrinityErrorCode::E_SUCCESS;
                        }
                    );

                    break;
                default:
                    Diagnostics::WriteLine(Diagnostics::Error, "EventLoop: Async work type {0} is not recognized.", pwork->type);
                    continue;
                }

                //  3. wait chain processing
                if (pwait != nullptr)
                {
                    eresult = _post_work(pwait);
                    _work_check_error(eresult, pwait);
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
            reset_work(p, work);
            return p;
        }

        void free_work(work_t* p)
        {
            if (p->pwait_chain != nullptr)
            {
                free_work(p->pwait_chain);
            }
            free(p);
        }

        void reset_work(work_t* p, worktype_t opType)
        {
#if defined(TRINITY_PLATFORM_WINDOWS)
            memset(p, 0, sizeof(WSAOVERLAPPED));
#endif
            p->type = opType;
            p->pwait_chain = nullptr;
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

DLL_EXPORT Trinity::Events::work_t* AllocContinuation(Trinity::Events::continuation_handler_t* pcontinuation, void* pdata, Trinity::Events::work_t* pdependency)
{
    auto pwork = Trinity::Events::alloc_work(Trinity::Events::worktype_t::Continuation);
    pwork->pcontinuation = pcontinuation;
    pwork->pcontinuation_data = pdata;
    pwork->pdependency = pdependency;
    return pwork;
}