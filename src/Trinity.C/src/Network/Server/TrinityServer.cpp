// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "os/os.h"
#include "Network/Server/TrinityServer.h"
#include "Network/Server/iocp/TrinitySocketServer.h"
#include "Network/Server/posix/TrinitySocketServer.h"

#include <stdio.h>
#include <string.h>

#include <iostream>

class _handler_initializer
{
public:
    _handler_initializer()
    {
        (void)Trinity::Network::ResetMessageHandlers();
    }
};

static _handler_initializer s_hinit;

namespace Trinity
{
    namespace Network
    {
        static message_handler_t * s_message_handlers[MAX_HANDLERS_COUNT];

        void _default_handler(MessageBuff * buff)
        {
            buff->Buffer = (char *)malloc(sizeof(TrinityErrorCode));
            buff->BytesToSend = sizeof(TrinityErrorCode);
            *((TrinityErrorCode *)(buff->Buffer)) = TrinityErrorCode::E_RPC_EXCEPTION;
        }

        void _workerthread()
        {
            auto tid = std::this_thread::get_id();
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

        TrinityErrorCode RegisterMessageHandler(uint16_t msg_type, message_handler_t * handler)
        {
            Diagnostics::WriteLine(Diagnostics::Debug, "TrinityServer: Registering message handler, ID={0}, handler={1}", msg_type, (void*)handler);
            s_message_handlers[msg_type] = handler;
            return TrinityErrorCode::E_SUCCESS;
        }

        TrinityErrorCode ResetMessageHandlers()
        {
            Diagnostics::WriteLine(Diagnostics::Debug, "TrinityServer: Resetting all message handlers");
            std::fill_n(s_message_handlers, MAX_HANDLERS_COUNT, &_default_handler);
            return TrinityErrorCode::E_SUCCESS;
        }

        TrinityErrorCode StartWorkerThreadPool()
        {
            int thread_count = std::thread::hardware_concurrency() * 2;

            try
            {
                for (auto i = 0; i < thread_count; ++i)
                {
                    std::thread(_workerthread).detach();
                }
            }
            catch(...)
            {
                Diagnostics::WriteLine(Diagnostics::Error, "TrinityServer: StartWorkerThreadPool: failed to spawn worker threads");
                return TrinityErrorCode::E_FAILURE;
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
            {
                std::this_thread::sleep_for(std::chrono::seconds(5));
            }
            return 0;
        }
    }
}
