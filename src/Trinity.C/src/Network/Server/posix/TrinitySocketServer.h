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
#include "Events/Events.h"
#include <errno.h>
#include <fcntl.h>
#include <stdio.h>
#include <string.h>
#include <unistd.h>

namespace Trinity
{
    namespace Network
    {
        using namespace Trinity::Events;

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

        void ResetContextObjects(PerSocketContextObject * pContext);
        bool ProcessRecv(PerSocketContextObject* pContext);
    }
    namespace Events
    {
        bool RearmFD(Network::PerSocketContextObject* pContext);
    }
}

#endif
