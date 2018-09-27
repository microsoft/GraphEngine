// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include <os/os.h>
#if defined(TRINITY_PLATFORM_WINDOWS)
#include "TrinitySocketServer.h"
#include <Trinity/Environment.h>
#include <Mstcpip.h>

namespace Trinity
{
    namespace Network
    {
        void SendAsync(PerSocketContextObject * pContext)
        {
            uint32_t bytesToSend = pContext->RemainingBytesToSend;

            pContext->wsaBodyBuf.buf = pContext->Message + pContext->BytesAlreadySent;
            pContext->wsaBodyBuf.len = bytesToSend;

            ResetOverlappedStruct(pContext->pOverlapped, worktype_t::Send);
            int32_t statusCode = WSASend(pContext->socket, &(pContext->wsaBodyBuf), /*buffer_cnt*/1, /*bytes_sent*/NULL, /*flags*/0, (LPWSAOVERLAPPED)(pContext->pOverlapped), /*callback*/NULL);
            if (SOCKET_ERROR == statusCode &&
                WSA_IO_PENDING != WSAGetLastError())
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "ServerSocket: Errors occur during WSASend. Error code = {0}", WSAGetLastError());
                CloseClientConnection(pContext, false);
            }
        }

        void ProcessSend(PerSocketContextObject * pContext, uint32_t bytesSent)
        {
            pContext->RemainingBytesToSend -= (int32_t)bytesSent;
            pContext->BytesAlreadySent += (int32_t)bytesSent;
            if (pContext->RemainingBytesToSend > 0)
            {
                SendAsync(pContext);
            }
            else
            {
                //Console::WriteLine("ServerSocket: Sent {0} bytes in total.", pContext->BytesAlreadySent);
                ResetContextObjects(pContext);
                ReceiveAsync(pContext, true);
            }
        }
    }
}
#endif//platform