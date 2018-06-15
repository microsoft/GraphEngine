// Graph Engine

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "os/os.h"
#ifdef TRINITY_PLATFORM_WINDOWS
#include "Network/Server/iocp/Common.h"
#include "Network/Server/TrinityServer.h"
#include <Threading/TrinityLock.h>
#include <unordered_set>

namespace Trinity
{
    namespace Network
    {
        extern TrinityLock psco_spinlock; // psco = per socket context object
        extern std::unordered_set<PerSocketContextObject*> psco_set;

        PerSocketContextObject* AllocatePerSocketContextObject(SOCKET socket);
        void FreePerSocketContextObject(PerSocketContextObject* p);
        OverlappedOpStruct* AllocateOverlappedOpStruct(SocketAsyncOperation opType);
        void FreeOverlappedOpStruct(OverlappedOpStruct* p);
        
        void CloseClientConnection(PerSocketContextObject* context, bool lingering);

        void SendAsync(PerSocketContextObject * pContext);
        void ReceiveAsync(PerSocketContextObject * pContext, bool receivePrefix);

        void ProcessSend(PerSocketContextObject * pContext, uint32_t bytesSent);
        bool ProcessRecv(PerSocketContextObject * pContext, uint32_t bytesRecvd);

        void ResetContextObjects(PerSocketContextObject * pContext);

        void ResetOverlappedStruct(OverlappedOpStruct* pOverlapped, SocketAsyncOperation opType);

        DWORD WINAPI SocketAcceptThreadProc(LPVOID lpParameter);

        /// The following APIs are for C/CS collaboration
        TrinityErrorCode AwaitIOCompletion(OUT void * &_pContext, OUT uint32_t & _opType, OUT uint32_t & _bytesTransferred);
        /// The return value indicates whether a message handler
        bool ProcessIOCompletion(void* pContext, uint32_t opType, uint32_t bytesTransferred);
    }
}
#endif