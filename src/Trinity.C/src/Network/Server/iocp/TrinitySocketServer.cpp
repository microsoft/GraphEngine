// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include <os/os.h>
#if defined(TRINITY_PLATFORM_WINDOWS)
#include "TrinitySocketServer.h"
#include <Trinity/Environment.h>
#include "Network/Server/iocp/Common.h"
#include "Threading/TrinitySpinlock.h"
#include "Network/ProtocolConstants.h"
#include "Network/Server/TrinityServer.h"
#include "Network/SocketOptionsHelper.h"
#include <winsock2.h>
#include <ws2tcpip.h>
#include <Mstcpip.h>
#include <Trinity/Diagnostics/Log.h>
#include <atomic>

namespace Trinity
{
    namespace Network
    {
        std::atomic<size_t> g_threadpool_size;

        SOCKET socket;
        HANDLE hIocp;

        TrinitySpinlock spinlock;
        bool initialized = false;

        bool InitializeNetwork()
        {
            spinlock.lock();
            if (!initialized)
            {
                WSAData wsaData;
                if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0)
                {
                    Diagnostics::WriteLine(Diagnostics::LogLevel::Fatal, "Cannot initialize winsock.");
                    spinlock.unlock();
                    return false;
                }

                g_threadpool_size = 0;
                initialized = true;
            }
            spinlock.unlock();
            return true;
        }

        int StartSocketServer(uint16_t port)
        {

            if (!InitializeNetwork())
                return -1;

            /// Without further profiling, the best overall maximum value to pick for the concurrency value is the number of processors.
            /// At the same time, a good rule of thumb is to have a minimum of twice as many threads in the thread pool as there are processors.
            hIocp = CreateIoCompletionPort(INVALID_HANDLE_VALUE, NULL, NULL, 0);

            if (NULL == hIocp)
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Fatal, "Cannot create IoCompletionPort.");
                ShutdownSocketServer();
                return TrinityErrorCode::E_FAILURE;
            }

            socket = WSASocket(AF_INET, SOCK_STREAM, IPPROTO_IP, NULL, 0, WSA_FLAG_OVERLAPPED);

            if (INVALID_SOCKET == socket)
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Fatal, "Cannot create a WSA socket.");
                WSACleanup();
                return -1;
            }

            SOCKADDR_IN ipe;
            ipe.sin_family = AF_INET;
            ipe.sin_port = htons(port); // converts a u_short from host to network byte order (big-endian)
            ipe.sin_addr.S_un.S_addr = INADDR_ANY;
            if (SOCKET_ERROR == bind(socket, (sockaddr*)&ipe, sizeof(SOCKADDR_IN)))
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Fatal, "Cannot bind the socket to an IP endpoint {0}.", port);
                ShutdownSocketServer();
                return -1;
            }

            if (SOCKET_ERROR == listen(socket, SOMAXCONN))
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Fatal, "Cannot turn the socket into listening state.");
                ShutdownSocketServer();
                return -1;
            }

            socket = (SOCKET)SetSocketOptions((uint64_t)socket, /*enable_keepalive:*/true, /*disable_sendbuf:*/true);
            if (INVALID_SOCKET == socket)
            {
                ShutdownSocketServer();
                return -1;
            }

            Diagnostics::WriteLine(Diagnostics::LogLevel::Info, "Listening endpoint :{0}", port);

            Diagnostics::WriteLine(Diagnostics::LogLevel::Info, "Waiting for client connection ...");

            DWORD threadId = 0;
            if (NULL == CreateThread(NULL, 0, SocketAcceptThreadProc, NULL, 0, &threadId))
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Fatal, "Cannot create socket accept threads.");
                ShutdownSocketServer();
                return -1;
            }

            return (int)socket;
        }

        int ShutdownSocketServer()
        {
            spinlock.lock();
            if (initialized)
            {
                //  Close the listening socket so that no new connections are coming in.
                //  The accept thread will be interrupted, and quit after this call.
                closesocket(socket);

                //  Post shutdown messages to IOCP
                for (uint64_t i=0, thread_pool_cnt = g_threadpool_size; i < thread_pool_cnt; ++i)
                {
                    LPOVERLAPPED pOverlapped = (LPOVERLAPPED)AllocateOverlappedOpStruct(SocketAsyncOperation::Shutdown);
                    PostQueuedCompletionStatus(hIocp, 0, NULL, pOverlapped);
                }

                while (g_threadpool_size) { Sleep(100); }

                //  Now, all threads in the thread pool are gone.
                //  Proceed to close all existing client sockets.
                psco_spinlock.lock();
                auto psco_set_shadow = psco_set;
                psco_spinlock.unlock();

                for (auto& pctx : psco_set_shadow)
                {
                    CloseClientConnection(pctx, false);
                }

                //  All client sockets are shut down. We have no network activity now.
                //  Proceed to bring down WSA.
                WSACleanup();

                //  Also bring down the IOCP.
                if (CloseHandle(hIocp))
                {
                    hIocp = NULL;
                }
                else
                {
                    Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "Cannot close IO completion port");
                }
                initialized = false;
            }
            spinlock.unlock();
            return 0;
        }

        void CloseClientConnection(PerSocketContextObject* pContext, bool lingering)
        {
            // closesocket deallocates socket handles and free up associated resources.
            // closesocket function implicitly causes a shutdown sequence to occur if it has not already happened,
            // we can use closesocket to both initiate the shutdown sequence and deallocate the socket handle.

            // the sockets interface provides for controls by way of the socket option mechanism that
            // allow us to indicate whether the implicit shutdown sequence should be graceful or abortive.
            // and also whether the closesocket function should linger(that is not complete immediately) to
            // allow time for a graceful shutdown sequence to complete.

            // More info: https://msdn.microsoft.com/en-us/library/windows/desktop/ms738547%28v=vs.85%29.aspx

            // Remove it from the client socket set
            psco_spinlock.lock();
            psco_set.erase(pContext);
            psco_spinlock.unlock();

            if (!lingering)
            {
                LINGER _linger;
                _linger.l_onoff = 1; //The socket will remain open for a specified amount of time.
                _linger.l_linger = 0; //
                setsockopt(pContext->socket, SOL_SOCKET, SO_LINGER, (char*)&_linger, sizeof(_linger));
            }

            // closesocket function implicitly causes a shutdown sequence
            closesocket(pContext->socket);
            pContext->socket = INVALID_SOCKET;
            FreePerSocketContextObject(pContext);
        }

    }
}

#endif