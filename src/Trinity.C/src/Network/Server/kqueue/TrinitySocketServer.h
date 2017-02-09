// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#if ((!defined(__linux__)) && (defined(__unix__) || (defined(__APPLE__) && defined(__MACH__))))
#include "Network/ProtocolConstants.h"
#include "Network/Server/TrinityServer.h"
#include "Network/posix/common.h"
#include <sys/types.h>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <string.h>
#include <fcntl.h>
#include <errno.h>

#include <sys/socket.h>
#include <netdb.h>
#include <sys/event.h>
#include <thread>

#include <Network/posix/common.h>
namespace Trinity
{
    namespace Network
    {
        typedef struct // represents a context object associated with each socket
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
        }PerSocketContextObject;

        void SocketAcceptThreadProc();
        void WorkerThreadProc(int tid);
        bool ProcessAccept();
    }
}
#endif