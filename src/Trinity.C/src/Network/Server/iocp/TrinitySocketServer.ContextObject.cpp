// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include <os/os.h>
#if defined(TRINITY_PLATFORM_WINDOWS)

#include "TrinitySocketServer.h"
#include <Trinity/Environment.h>
#include <Mstcpip.h>

namespace Trinity
{
    namespace Network
    {
        TrinitySpinlock psco_spinlock; // psco = per socket context object
        std::unordered_set<PerSocketContextObject*> psco_set;

        OverlappedOpStruct* AllocateOverlappedOpStruct(SocketAsyncOperation opType)
        {
            OverlappedOpStruct* p = (OverlappedOpStruct*)malloc(sizeof(OverlappedOpStruct));
            memset(p, 0, sizeof(WSAOVERLAPPED));
            p->OpType = opType;
            return p;
        }

        void FreeOverlappedOpStruct(OverlappedOpStruct* p)
        {
            free(p);
        }

        void FreePerSocketContextObject(PerSocketContextObject* p)
        {
            FreeOverlappedOpStruct(p->pOverlapped);
            free(p->Message); 
            if (p->Message != p->RecvBuffer)
            {
                free(p->RecvBuffer);
            }

            free(p);
        }

        /// The handshake protocol:
        /// Client sends handshake signature:
        /// [4B Len|SIGNATURE_BODY]
        /// Server checks the signature and reply:
        /// [4B TrinityErrorCode]/[Disconnect]
        PerSocketContextObject* AllocatePerSocketContextObject(SOCKET socket)
        {
            PerSocketContextObject* p = (PerSocketContextObject*)malloc(sizeof(PerSocketContextObject));

            p->Message = NULL;
            p->RemainingBytesToSend = 0;
            p->BytesAlreadySent = 0;
            p->WaitingHandshakeMessage = true;
            Diagnostics::WriteLine(Diagnostics::LogLevel::Verbose, "ServerSocket: Client {0} allocated.", p);

            p->socket = socket;
            p->ReceivedPrefixBytes = 0;
            p->ReceivedMessageBodyBytes = 0;

            p->RecvBuffer = (char*)malloc(UInt32_Contants::RecvBufferSize); // RecvBuffer should be read-only after being initialized
            p->RecvBufferLen = UInt32_Contants::RecvBufferSize; // RecvBufferLen should be read-only after being initialized
            p->avg_RecvBufferLen = UInt32_Contants::RecvBufferSize;
            
            p->pOverlapped = AllocateOverlappedOpStruct(SocketAsyncOperation::Receive);
            p->BodyLength = 0;

            p->wsaPrefixBuf.buf = p->MessagePrefix;
            p->wsaPrefixBuf.len = UInt32_Contants::MessagePrefixLength;
            p->wsaBodyBuf.buf = p->RecvBuffer;
            p->wsaBodyBuf.len = p->RecvBufferLen;

            return p;
        }

        /// Resets context object so that it is ready for a receive operation.
        /// This routine is called after the buffer passed from C# is sent.
        void ResetContextObjects(PerSocketContextObject * pContext)
        {
            free(pContext->Message); //free response buffer passed from C#
            pContext->Message = NULL;
            pContext->ReceivedMessageBodyBytes = 0;
            pContext->RemainingBytesToSend = 0;

            // Calculate average received message length with a sliding window.
            pContext->avg_RecvBufferLen = (uint32_t)(pContext->avg_RecvBufferLen * Float_Constants::AvgSlideWin_a + pContext->BodyLength * Float_Constants::AvgSlideWin_b);
            // Make sure that average received message length is capped above default value.
            pContext->avg_RecvBufferLen = std::max(pContext->avg_RecvBufferLen, static_cast<uint32_t>(UInt32_Contants::RecvBufferSize));
            // If the average received message length drops below half of current recv buf len, adjust it.
            if (pContext->avg_RecvBufferLen < pContext->RecvBufferLen / Float_Constants::AvgSlideWin_r)
            {
                free(pContext->RecvBuffer);
                pContext->RecvBufferLen = pContext->avg_RecvBufferLen;
                pContext->RecvBuffer = (char*)malloc(pContext->RecvBufferLen);
            }

            pContext->wsaPrefixBuf.buf = pContext->MessagePrefix;
            pContext->wsaPrefixBuf.len = UInt32_Contants::MessagePrefixLength;
            pContext->wsaBodyBuf.buf = pContext->RecvBuffer;
            pContext->wsaBodyBuf.len = pContext->RecvBufferLen;
            
            pContext->ReceivedPrefixBytes = 0;
            pContext->BytesAlreadySent = 0;
            
            pContext->BodyLength = 0;
        }

        void ResetOverlappedStruct(OverlappedOpStruct* pOverlapped, SocketAsyncOperation opType)
        {
            memset(pOverlapped, 0, sizeof(WSAOVERLAPPED));
            pOverlapped->OpType = opType;
        }
    }
}

#endif
