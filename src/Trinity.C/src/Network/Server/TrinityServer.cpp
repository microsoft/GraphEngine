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

#include "Trinity/Threading/TrinityLock.h"
#include <iostream>
#include <unordered_set>

namespace Trinity
{
    namespace Network
    {
        TrinityLock psco_spinlock; // psco = per socket context object
        std::unordered_set<PerSocketContextObject*> psco_set;

#pragma region PerSocketContextObject management

        void AddPerSocketContextObject(PerSocketContextObject * pContext)
        {
            psco_spinlock.lock();
            psco_set.insert(pContext);
            psco_spinlock.unlock();

        }

        void RemovePerSocketContextObject(PerSocketContextObject* psco)
        {
            psco_spinlock.lock();
            psco_set.erase(psco);
            psco_spinlock.unlock();
        }

        void CloseAllClientSockets()
        {
            psco_spinlock.lock();
            auto psco_set_shadow = psco_set;
            psco_spinlock.unlock();

            for (auto& pctx : psco_set_shadow) { CloseClientConnection(pctx, false); }
        }

#pragma endregion

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
