// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <stdint.h>
#include <cstdlib>
#include <thread>
#include <algorithm>
#include <Network/ProtocolConstants.h>
#include <atomic>

namespace Trinity
{
    namespace Network
    {
        typedef struct // This is for data exchange between unmanaged and managed worlds
        {
            char* Buffer; // allocate after receiving the message header
            uint32_t BytesReceived;
            uint32_t BytesToSend;
        }MessageBuff;

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
        }PerSocketContextObjectSlim;

        // Should be defined in an implementation.
        extern std::atomic<size_t> g_threadpool_size;

        int StartSocketServer(uint16_t port);

        int ShutdownSocketServer();

        int InitializeEventMonitor(int sfd);

        int AcceptConnection(int sock_fd);

        void AwaitRequest(void* &pContext);

        bool ProcessRecv(PerSocketContextObjectSlim* pContext);

        bool RearmFD(int fd);

        void SendResponse(void* _pContext);

        void MessageHandler(MessageBuff * msg);

        inline void EnterSocketServerThreadPool()
        {
            ++g_threadpool_size;
        }

        inline void ExitSocketServerThreadPool()
        {
            --g_threadpool_size;
        }

        inline PerSocketContextObjectSlim* AllocatePerSocketContextObjectSlim(int fd)
        {
            //TODO handshake
            PerSocketContextObjectSlim* p = (PerSocketContextObjectSlim*)malloc(sizeof(PerSocketContextObjectSlim));

            p->RecvBuffer = (char*)malloc(UInt32_Contants::RecvBufferSize);
            p->RecvBufferLen = UInt32_Contants::RecvBufferSize;
            p->avg_RecvBufferLen = UInt32_Contants::RecvBufferSize;

            p->fd = fd;

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

        void WorkerThreadProc(int tid);
        bool StartWorkerThreadPool();
        int TrinityServerTestEntry();
    }
}