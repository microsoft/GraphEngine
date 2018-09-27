// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "os/os.h"
#if defined(TRINITY_PLATFORM_WINDOWS)

#include "Events.h"
#include "Trinity/Diagnostics/Log.h"
#include "Network/Server/iocp/Common.h"
#include "Network/Server/iocp/TrinitySocketServer.h"
#include <winsock2.h>

namespace Trinity
{
    namespace Events
    {
        HANDLE hIocp;
        using Network::OverlappedOpStruct;

        TrinityErrorCode platform_start_eventloop()
        {
            hIocp = CreateIoCompletionPort(INVALID_HANDLE_VALUE, NULL, NULL, 0);

            if (hIocp == NULL)
            {
                auto e = GetLastError();
                Diagnostics::WriteLine(Diagnostics::Error, "EventLoop: Failed to create IOCP handle. Error code = {0}", e);
                return TrinityErrorCode::E_FAILURE;
            }

            return TrinityErrorCode::E_SUCCESS;
        }

        TrinityErrorCode platform_stop_eventloop()
        {
            //  Post shutdown messages to IOCP
            for (uint64_t i=0, thread_pool_cnt = g_threadpool_size; i < thread_pool_cnt; ++i)
            {
                LPOVERLAPPED pOverlapped = (LPOVERLAPPED)Network::AllocateOverlappedOpStruct(worktype_t::Shutdown);
                PostQueuedCompletionStatus(hIocp, 0, NULL, pOverlapped);
            }

            while (g_threadpool_size) { Sleep(100); }

            //  Also bring down the IOCP.
            if (CloseHandle(hIocp))
            {
                hIocp = NULL;
                return TrinityErrorCode::E_SUCCESS;
            }
            else
            {
                auto errorcode = GetLastError();
                Diagnostics::WriteLine(Diagnostics::Error, "EventLoop: Failed to close IOCP handle. Error code = {0}", errorcode);
                return TrinityErrorCode::E_FAILURE;
            }
        }

        TrinityErrorCode AwaitIOCompletion(OUT void* &_pContext, OUT worktype_t &_opType, OUT uint32_t &_bytesTransferred)
        {
            DWORD bytesTransferred;
            Network::PerSocketContextObject* pContext;
            LPWSAOVERLAPPED pOverlapped;
            if (FALSE == GetQueuedCompletionStatus(hIocp, &bytesTransferred, (PULONG_PTR)&pContext, (LPOVERLAPPED *)&pOverlapped, INFINITE))
            {
                //Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "ServerSocket: Errors occur during GetQueuedCompletionStatus with error code: {0}.", GetLastError());

                if (pContext != NULL)
                {
                    Network::CloseClientConnection(pContext, false);
                }
                return TrinityErrorCode::E_FAILURE;
            }

            OverlappedOpStruct* pOverlappedStruct = (OverlappedOpStruct*)pOverlapped;
            _pContext = pContext;
            _opType = pOverlappedStruct->work;
            _bytesTransferred = bytesTransferred;

            if (_opType == worktype_t::Shutdown)
            {
                FreeOverlappedOpStruct(pOverlappedStruct);
                _pContext = NULL;
                return TrinityErrorCode::E_NETWORK_SHUTDOWN;
            }

            return TrinityErrorCode::E_SUCCESS;
        }

        // Return true if a message is received, and should be reported to the messaging system.
        bool ProcessIOCompletion(void* _pContext, uint32_t opType, uint32_t bytesTransferred)
        {
            Network::PerSocketContextObject* pContext = (Network::PerSocketContextObject*)_pContext;
            if (opType == (uint32_t)worktype_t::Receive)
            {
                bool ret = ProcessRecv(pContext, bytesTransferred);
                // If the handshake response is received, we examine the response in-place
                // here without reporting to the messaging system.
                if (ret && pContext->WaitingHandshakeMessage)
                {
                    CheckHandshakeResult(pContext);
                    return false;
                }
                else
                {
                    return ret;
                }
            }

            if (opType == (uint32_t)worktype_t::Send)
            {
                ProcessSend(pContext, bytesTransferred);
                return false;
            }
            Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "EventLoop: Async work type {0} is not recognized.", opType);
            return false;
        }

        worktype_t AwaitRequest(void* &_pContext)
        {
            worktype_t work;
            uint32_t bytesTransferred;
            while (true)
            {
                TrinityErrorCode eResult = AwaitIOCompletion(_pContext, work, bytesTransferred);
                if (TrinityErrorCode::E_SUCCESS == eResult && ProcessIOCompletion(_pContext, work, bytesTransferred))
                {
                    break;
                }
                if (TrinityErrorCode::E_NETWORK_SHUTDOWN == eResult)
                {
                    break;
                }
            }

            return work;
        }

        TrinityErrorCode EnterEventMonitor(Network::PerSocketContextObject* key)
        {
            if (NULL == CreateIoCompletionPort((HANDLE)key->socket, hIocp, (ULONG_PTR)key, 0))
            {
                auto err = GetLastError();
                Diagnostics::WriteLine(Diagnostics::Error, "EventLoop: Cannot associate IO handle to the completion port. Error code = {0}", err);
                return TrinityErrorCode::E_FAILURE;
            }
            return TrinityErrorCode::E_SUCCESS;
        }
    }
}

#endif