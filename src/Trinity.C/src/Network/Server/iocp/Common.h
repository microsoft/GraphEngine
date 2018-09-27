// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include "os/os.h"
#ifdef TRINITY_PLATFORM_WINDOWS

#include "TrinityCommon.h"
#include "Events/Events.h"
#include "Network/ProtocolConstants.h"
#include "Network/Server/TrinityServer.h"
#include <winsock2.h>
#include <ws2tcpip.h>
#include <Mstcpip.h>
#include <Trinity/Diagnostics/Log.h>

namespace Trinity
{
    namespace Network
    {
        struct OverlappedOpStruct
        {
            WSAOVERLAPPED Overlapped;
            Events::worktype_t work; // record the work type when issuing an async op, e.g., WSARecv, WSASend
        };

        struct PerSocketContextObject// represents a context object associated with each socket
        {
            union
            {
                struct
                {
                    // for RECV, points to RecvBuffer after a message is completely received
                    // for SEND, allocated by a message handler, freed after message is sent.
                    char* Message; 
                    uint32_t ReceivedMessageBodyBytes;
                    uint32_t RemainingBytesToSend;
                };

                Events::MessageBuff SendRecvBuff;
            }; // size: 16

            WSABUF wsaPrefixBuf;  // size: 16
            WSABUF wsaBodyBuf; // size: 16

            SOCKET socket; // size: 8
            int32_t ReceivedPrefixBytes; //size: 4
            int32_t BytesAlreadySent; //size: 4
            
            char * RecvBuffer;  //size: 8
            uint32_t RecvBufferLen; //size: 4
            uint32_t avg_RecvBufferLen; //size: 4

            OverlappedOpStruct* pOverlapped; //size: 8
            union
            {   // size: 4 
                int32_t BodyLength;
                char MessagePrefix[MessagePrefixLength];
            };
            bool WaitingHandshakeMessage;
        };

        typedef void(*PTrinityMessageHandler)(void* pParameter);

        bool InitializeNetwork();
    }
}
#endif
