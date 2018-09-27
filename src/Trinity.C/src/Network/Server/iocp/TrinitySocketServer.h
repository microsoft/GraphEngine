// Graph Engine

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "os/os.h"
#if defined(TRINITY_PLATFORM_WINDOWS)
#include "Common.h"
#include "Network/Server/TrinityServer.h"
#include "Trinity/Threading/TrinityLock.h"
#include <unordered_set>

namespace Trinity
{
    namespace Network
    {
        using namespace Trinity::Events;

        OverlappedOpStruct* AllocateOverlappedOpStruct(worktype_t work);
        void FreeOverlappedOpStruct(OverlappedOpStruct* p);
        void ResetOverlappedStruct(OverlappedOpStruct* pOverlapped, worktype_t work);
        void ResetContextObjects(PerSocketContextObject * pContext);

        void SendAsync(PerSocketContextObject * pContext);
        void ReceiveAsync(PerSocketContextObject * pContext, bool receivePrefix);

        void ProcessSend(PerSocketContextObject * pContext, uint32_t bytesSent);
        bool ProcessRecv(PerSocketContextObject * pContext, uint32_t bytesRecvd);

        DWORD WINAPI SocketAcceptThreadProc(LPVOID lpParameter);
    }
}
#endif
