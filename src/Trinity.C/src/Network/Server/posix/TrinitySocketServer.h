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
#include <algorithm>
#include <atomic>
#include <errno.h>
#include <fcntl.h>
#include <netdb.h>
#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/socket.h>
#include <sys/types.h>
#include <thread>
#include <unistd.h>

namespace Trinity
{
    namespace Network
    {
        typedef union
        {
            uintptr_t ident;
            int fd;            
        }ContextObjectKey;

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
        }PerSocketContextObjectSlim;

        inline bool make_nonblocking(int fd)
        {
            int status_flags = fcntl(fd, F_GETFL, 0);
            if (-1 == status_flags)
                return false;
            status_flags |= O_NONBLOCK;
            if (-1 == fcntl(fd, F_SETFL, status_flags))
                return false;
            return true;
        }

        inline PerSocketContextObjectSlim* AllocatePerSocketContextObjectSlim(int fd)
        {
            PerSocketContextObjectSlim* p = (PerSocketContextObjectSlim*)malloc(sizeof(PerSocketContextObjectSlim));

            p->RecvBuffer = (char*)malloc(UInt32_Contants::RecvBufferSize);
            p->RecvBufferLen = UInt32_Contants::RecvBufferSize;
            p->avg_RecvBufferLen = UInt32_Contants::RecvBufferSize;

            p->fd = fd;
            p->WaitingHandshakeMessage = true;

            return p;
        }

        inline void FreePerSocketContextObjectSlim(PerSocketContextObjectSlim* p)
        {
            free(p->RecvBuffer);
            free(p);
        }

        inline void ResetContextObjects(PerSocketContextObjectSlim * pContext)
        {
            free(pContext->Message);

            // Calculate average received message length with a sliding window.
            pContext->avg_RecvBufferLen = (uint32_t)(pContext->avg_RecvBufferLen * Float_Constants::AvgSlideWin_a + pContext->ReceivedMessageBodyBytes * Float_Constants::AvgSlideWin_b);
            // Make sure that average received message length is capped above default value.
            pContext->avg_RecvBufferLen = std::max(pContext->avg_RecvBufferLen, static_cast<uint32_t>(UInt32_Contants::RecvBufferSize));
            // If the average received message length drops below half of current recv buf len, adjust it.
            if (pContext->avg_RecvBufferLen < pContext->RecvBufferLen / Float_Constants::AvgSlideWin_r)
            {
                free(pContext->RecvBuffer);
                pContext->RecvBufferLen = pContext->avg_RecvBufferLen;
                pContext->RecvBuffer = (char*)malloc(pContext->RecvBufferLen);
            }
        }

        void AddPerSocketContextObject(PerSocketContextObjectSlim * pContext);
        void RemovePerSocketContextObject(int fd);
        PerSocketContextObjectSlim* GetPerSocketContextObject(int fd);

        inline void CloseClientConnection(PerSocketContextObjectSlim* pContext, bool lingering)
        {
            RemovePerSocketContextObject(pContext);
            //TODO lingering
            close(pContext->fd);
            FreePerSocketContextObjectSlim(pContext);
        }

        inline void CheckHandshakeResult(PerSocketContextObjectSlim* pContext)
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


        int InitializeEventMonitor(int sfd);

        int AcceptConnection(int sock_fd);

        bool ProcessRecv(PerSocketContextObjectSlim* pContext);

        bool RearmFD(int fd);

    }
}

#endif
