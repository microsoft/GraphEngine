// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include <os/os.h>
#if defined(TRINITY_PLATFORM_WINDOWS)
#include "TrinitySocketServer.h"
#include <Trinity/Environment.h>
#include <Mstcpip.h>
#include "Network/SocketOptionsHelper.h"
#include "Trinity/Hash/NonCryptographicHash.h"
#include "Trinity/Configuration/TrinityConfig.h"
#include "Network/ProtocolConstants.h"


namespace Trinity
{
    namespace Network
    {
        extern SOCKET socket;
        extern HANDLE hIocp;

        void WorkerThreadProc(int tid)
        {
            while (true)
            {
                void* _pContext;
                AwaitRequest(_pContext);
                PerSocketContextObject* pContext = (PerSocketContextObject*)_pContext;
                MessageHandler((MessageBuff*)pContext);
                SendResponse(pContext);
            }
        }

        void CheckResponse(PerSocketContextObject* pContext)
        {
            int32_t messageBody = *(int32_t*)pContext->Message;
            int32_t bytesToSend = pContext->RemainingBytesToSend;
            Console::WriteLine("ServerSocket: Response: {0} {1}", messageBody, bytesToSend);
        }

        // P/Inovke
        void SendResponse(void* _pContext)
        {
            PerSocketContextObject* pContext = (PerSocketContextObject*)_pContext;
            pContext->BytesAlreadySent = 0;
            SendAsync(pContext);
        }

        // P/Inovke
        void AwaitRequest(void* &_pContext)
        {
            uint32_t opType;
            uint32_t bytesTransferred;
            while (true)
            {
                TrinityErrorCode eResult = AwaitIOCompletion(_pContext, opType, bytesTransferred);
                if (TrinityErrorCode::E_SUCCESS == eResult && ProcessIOCompletion(_pContext, opType, bytesTransferred))
                {
                    break;
                }
                if (TrinityErrorCode::E_NETWORK_SHUTDOWN == eResult)
                {
                    break;
                }
            }
        }

        TrinityErrorCode AwaitIOCompletion(OUT void* &_pContext, OUT uint32_t &_opType, OUT uint32_t &_bytesTransferred)
        {
            DWORD bytesTransferred;
            PerSocketContextObject* pContext;
            LPWSAOVERLAPPED pOverlapped;
            if (FALSE == GetQueuedCompletionStatus(hIocp, &bytesTransferred, (PULONG_PTR)&pContext, (LPOVERLAPPED *)&pOverlapped, INFINITE))
            {
                //Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "ServerSocket: Errors occur during GetQueuedCompletionStatus with error code: {0}.", GetLastError());

                if (pContext != NULL)
                {
                    CloseClientConnection(pContext, false);
                }
                return TrinityErrorCode::E_FAILURE;
            }

            OverlappedOpStruct* pOverlappedStruct = (OverlappedOpStruct*)pOverlapped;
            _pContext = pContext;
            _opType = pOverlappedStruct->OpType;
            _bytesTransferred = bytesTransferred;

            if (_opType == SocketAsyncOperation::Shutdown)
            {
                FreeOverlappedOpStruct(pOverlappedStruct);
                _pContext = NULL;
                return TrinityErrorCode::E_NETWORK_SHUTDOWN;
            }

            return TrinityErrorCode::E_SUCCESS;
        }

        void CheckHandshakeResult(PerSocketContextObject* pContext)
        {
            if (pContext->ReceivedMessageBodyBytes != HANDSHAKE_MESSAGE_LENGTH)
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "ServerSocket: Client {0} responds with invalid handshake message header.", pContext);
                goto handshake_check_fail;
            }

            if (memcmp(pContext->Message, HANDSHAKE_MESSAGE_CONTENT, HANDSHAKE_MESSAGE_LENGTH) != 0)
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "ServerSocket: Client {0} responds with invalid handshake message.", pContext);
                goto handshake_check_fail;
            }

            // handshake_check_success: acknowledge the handshake and then switch into recv mode
            pContext->WaitingHandshakeMessage = false;
            pContext->Message = (char*)malloc(sizeof(int32_t));
            pContext->RemainingBytesToSend = sizeof(int32_t);
            *(int32_t*)pContext->Message = (int32_t)TrinityErrorCode::E_SUCCESS;
            SendResponse(pContext);
            return;

        handshake_check_fail:
            CloseClientConnection(pContext, false);
            return;
        }

        // Return true if a message is received, and should be reported to the messaging system.
        bool ProcessIOCompletion(void* _pContext, uint32_t opType, uint32_t bytesTransferred)
        {
            PerSocketContextObject* pContext = (PerSocketContextObject*)_pContext;
            if (opType == (uint32_t)SocketAsyncOperation::Receive)
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

            if (opType == (uint32_t)SocketAsyncOperation::Send)
            {
                ProcessSend(pContext, bytesTransferred);
                return false;
            }
            Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "ServerSocket: IOCompletion type {0} is not recognized.", opType);
            return false;
        }



        DWORD WINAPI SocketAcceptThreadProc(LPVOID lpParameter)
        {
            /// Start accepting new connections from clients
            SOCKADDR accept_sockaddr;
            INT      accept_sockaddr_len = sizeof(accept_sockaddr);
            DWORD    accept_sockaddr_strbuf_len = 1024;
            LPWSTR   accept_sockaddr_strbuf     = (LPWSTR)malloc(accept_sockaddr_strbuf_len * sizeof(WCHAR));
            while (true)
            {
                accept_sockaddr_len = sizeof(SOCKADDR);
                SOCKET acceptSocket = WSAAccept(socket, &accept_sockaddr, &accept_sockaddr_len, NULL, 0);

                if (INVALID_SOCKET == acceptSocket)
                {
                    int errorCode = WSAGetLastError();
                    free(accept_sockaddr_strbuf);
                    if (WSAEINTR == errorCode)
                    {
                        // the accept call is interrupted. assume the server has been shutdown.
                        return 0;
                    }
                    Diagnostics::WriteLine(Diagnostics::LogLevel::Fatal, "ServerSocket: Cannot create accept socket, error code {0}.", errorCode);
                    return ShutdownSocketServer();
                }

                accept_sockaddr_strbuf_len = 1024;
                if (WSAAddressToString(&accept_sockaddr, accept_sockaddr_len, NULL, accept_sockaddr_strbuf, &accept_sockaddr_strbuf_len))
                {
                    lstrcpynW(accept_sockaddr_strbuf, L"<Unknown address>", 1024);
                }
                Diagnostics::WriteLine(Diagnostics::LogLevel::Debug, "ServerSocket: Incomming connection from {0}", String(accept_sockaddr_strbuf));

                acceptSocket = (SOCKET)SetSocketOptions(acceptSocket, /*enable_keepalive:*/true, /*disable_sendbuf:*/true);
                if (INVALID_SOCKET == acceptSocket)
                {
                    continue;
                }

                PerSocketContextObject * pContext = AllocatePerSocketContextObject(acceptSocket);

                // Add it to the client socket set
                psco_spinlock.lock();
                psco_set.insert(pContext);
                psco_spinlock.unlock();

                // associate the accept socket with the previously created IOCP
                if (NULL == CreateIoCompletionPort((HANDLE)acceptSocket, hIocp, (ULONG_PTR)pContext, 0))
                {
                    Diagnostics::WriteLine(Diagnostics::LogLevel::Fatal, "ServerSocket: Cannot create worker threads.");
                    return ShutdownSocketServer();
                }

                // post the first async receive
                ReceiveAsync(pContext, true);

            }
        }
    }
}
#endif//platform