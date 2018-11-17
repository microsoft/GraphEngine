// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "os/os.h"
#if defined(TRINITY_PLATFORM_WINDOWS)

#include "Events.h"
#include "Trinity/Diagnostics/Log.h"
#include "Network/Network.h"

namespace Trinity
{
    namespace Events
    {
        HANDLE hIocp;

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
            for (uint64_t i=0, n = g_threadpool_size; i < n; ++i)
            {
                auto p = alloc_work(worktype_t::Shutdown);
                PostQueuedCompletionStatus(hIocp, 0, NULL, (LPOVERLAPPED)p);
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

        TrinityErrorCode platform_poll(OUT work_t* &pwork, OUT uint32_t &bytesTransferred)
        {
            Network::sock_t* psock;
            if (FALSE == GetQueuedCompletionStatus(hIocp, (LPDWORD)&bytesTransferred, (PULONG_PTR)&psock, (LPOVERLAPPED *)&pwork, INFINITE))
            {
                if (psock != NULL)
                {
                    Network::close_conn(psock, false);
                }
                return TrinityErrorCode::E_FAILURE;
            }

            return TrinityErrorCode::E_SUCCESS;
        }

        TrinityErrorCode enter_eventloop(Network::sock_t* key)
        {
            if (NULL == CreateIoCompletionPort((HANDLE)key->socket, hIocp, (ULONG_PTR)key, 0))
            {
                auto err = GetLastError();
                Diagnostics::WriteLine(Diagnostics::Error, "EventLoop: Cannot associate IO handle to the completion port. Error code = {0}", err);
                return TrinityErrorCode::E_FAILURE;
            }
            return TrinityErrorCode::E_SUCCESS;
        }

        TrinityErrorCode platform_post_workitem(work_t* pwork)
        {
            auto hr = PostQueuedCompletionStatus(hIocp, 0, NULL, (LPOVERLAPPED)pwork);
            if (hr == FALSE)
            {
                auto err = GetLastError();
                Diagnostics::WriteLine(Diagnostics::Error, "EventLoop: Cannot post compute workload to the completion port. Error code = {0}", err);
                return TrinityErrorCode::E_FAILURE;
            }

            return TrinityErrorCode::E_SUCCESS;
        }

    }
}

#endif