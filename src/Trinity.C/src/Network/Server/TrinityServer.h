// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <stdint.h>
#include <cstdlib>
#include "TrinityCommon.h"
#include "Network/Network.h"
#include "Network/ProtocolConstants.h"

namespace Trinity
{
    namespace Network
    {
        // Should be defined in an implementation.
        struct PerSocketContextObject;

        int StartSocketServer(uint16_t port);
        int ShutdownSocketServer();
        void SendResponse(void* _pContext);
        void CheckHandshakeResult(PerSocketContextObject* pContext);

        PerSocketContextObject* AllocatePerSocketContextObject(SOCKET sock);
        void FreePerSocketContextObject(PerSocketContextObject* p);
        void AddPerSocketContextObject(PerSocketContextObject * pContext);
        void RemovePerSocketContextObject(PerSocketContextObject* psco);
        // CloseClientConnection will also call RemovePerSocketContextObject
        void CloseClientConnection(PerSocketContextObject* context, bool lingering);
        void CloseAllClientSockets();

#pragma region Test purpose only
        int TrinityServerTestEntry();
#pragma endregion
    }
}
