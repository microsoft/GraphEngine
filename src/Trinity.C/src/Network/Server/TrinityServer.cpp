// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "os/os.h"
#include "Network/Server/TrinityServer.h"
#ifdef TRINITY_PLATFORM_WINDOWS
#include "Network/Server/iocp/TrinitySocketServer.h"
#else
#include "Network/Server/posix/TrinitySocketServer.h"
#endif

#include <stdio.h>
#include <string.h>

#include <iostream>

namespace Trinity
{
    namespace Network
    {
        static message_handler_t * s_message_handlers[MAX_HANDLERS_COUNT];

        DWORD RegisterMessageHandler(uint16_t msg_type, message_handler_t * handler)
        {
            s_message_handlers[msg_type] = handler;
            return true;
        }

        void _default_handler(MessageBuff * buff)
        {
            buff->Buffer = (char *)malloc(sizeof(TrinityErrorCode));
            buff->BytesToSend = sizeof(TrinityErrorCode);
            *((TrinityErrorCode *)(buff->Buffer)) = TrinityErrorCode::E_RPC_EXCEPTION;
        }

        void _workerthread(int32_t tid)
        {
            Diagnostics::WriteLine(Diagnostics::Debug, "TrinityServer: Starting worker thread #{0}", tid);
            EnterSocketServerThreadPool();
            while (true)
            {
                void* _pContext = NULL;
                AwaitRequest(_pContext);
                if (_pContext == NULL) { break; }
                MessageBuff* msg = (MessageBuff*)_pContext;
                s_message_handlers[*(uint16_t *)msg->Buffer](msg);
                SendResponse(_pContext);
            }
            Diagnostics::WriteLine(Diagnostics::Debug, "TrinityServer: Stopping worker thread #{0}", tid);
            ExitSocketServerThreadPool();
        }

        DWORD StartWorkerThreadPool()
        {
            int thread_count = std::thread::hardware_concurrency() * 2;
            std::fill_n(s_message_handlers, MAX_HANDLERS_COUNT, &_default_handler);
            for(auto i = 0; i < thread_count; ++i ) std::thread(_workerthread, i).detach();
            
            return true;
        }

        int TrinityServerTestEntry()
        {
            int sock_fd = StartSocketServer(5304);
            if (-1 == sock_fd)
            {
                fwprintf(stderr, L">>> cannot start socket server\n");
                return -1;
            }
            StartWorkerThreadPool();
            while (true)
                std::this_thread::sleep_for(std::chrono::seconds(5));
            return 0;
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

    }
}
