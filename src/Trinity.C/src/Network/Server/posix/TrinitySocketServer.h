// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <os/os.h>
#if !defined(TRINITY_PLATFORM_WINDOWS)
#include "Trinity/Diagnostics/Log.h"
#include "Network/Network.h"
#include "Network/Server/TrinityServer.h"
#include <errno.h>
#include <fcntl.h>
#include <stdio.h>
#include <string.h>
#include <unistd.h>

namespace Trinity
{
    namespace Network
    {
        union ContextObjectKey
        {
            uintptr_t ident;
            int fd;            
        };

        struct PerSocketContextObject// represents a context object associated with each socket
        {
            union
            {
                struct
                {
                    char* Message; // allocate after receiving the message header
                    uint32_t ReceivedMessageBodyBytes;
                    uint32_t RemainingBytesToSend;
                };
                MessageBuff SendRecvBuff;
            }; // size: 16

            char * RecvBuffer;
            uint32_t RecvBufferLen;
            uint32_t avg_RecvBufferLen;

            union
            {
                union {
                    uintptr_t ident;
                    int fd;
                };
                ContextObjectKey Key;
            };
            bool WaitingHandshakeMessage;
        };

        PerSocketContextObject* AllocatePerSocketContextObject(int fd);
        void                    FreePerSocketContextObject(PerSocketContextObject* p);
        void                    ResetContextObjects(PerSocketContextObject * pContext);
        void                    AddPerSocketContextObject(PerSocketContextObject * pContext);
        void                    RemovePerSocketContextObject(int fd);
        PerSocketContextObject* GetPerSocketContextObject(int fd);
        void                    CloseClientConnection(PerSocketContextObject* pContext, bool lingering);
        int                     AcceptConnection(int sock_fd);
        bool                    ProcessRecv(PerSocketContextObject* pContext);
        void                    CheckHandshakeResult(PerSocketContextObject* pContext);

#pragma region Platform-specific routines
        int                     InitializeEventMonitor();   // Initialize the event monitor, and start the accept thread
        int                     UninitializeEventMonitor(); // Tear down the event monitor, and stop the accept thread; After call returns, there should be no more connections;
        int                     EnterEventMonitor(PerSocketContextObject* pContext); // Puts a new connection into the event monitor
        bool                    RearmFD(PerSocketContextObject* pContext); 
#pragma endregion
    }
}

#endif
