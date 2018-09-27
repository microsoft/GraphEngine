// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include <os/os.h>
#include <io>
#if defined(TRINITY_PLATFORM_WINDOWS)
#include "TrinitySocketServer.h"
#include <Trinity/Environment.h>
#include <Mstcpip.h>
#include "Network/SocketOptionsHelper.h"
#include "Trinity/Hash/NonCryptographicHash.h"
#include "Network/ProtocolConstants.h"


namespace Trinity
{
    namespace Network
    {
        extern SOCKET socket;

        void CheckResponse(PerSocketContextObject* pContext)
        {
            int32_t messageBody = *(int32_t*)pContext->Message;
            int32_t bytesToSend = pContext->RemainingBytesToSend;
            Console::WriteLine("ServerSocket: Response: {0} {1}", messageBody, bytesToSend);
        }

        void SendResponse(void* _pContext)
        {
            PerSocketContextObject* pContext = (PerSocketContextObject*)_pContext;
            pContext->BytesAlreadySent = 0;
            SendAsync(pContext);
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
                    Diagnostics::WriteLine(Diagnostics::LogLevel::Fatal, "ServerSocket: Cannot accept incoming connection, error code {0}.", errorCode);
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
                AddPerSocketContextObject(pContext);

                // associate the accept socket with the previously created IOCP
                if (TrinityErrorCode::E_SUCCESS != EnterEventMonitor(pContext))
                {
                    CloseClientConnection(pContext, false);
                    continue;
                }

                // post the first async receive
                ReceiveAsync(pContext, true);
            }
        }
    }
}
#endif//platform