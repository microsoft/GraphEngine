// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <stdint.h>
#include <os/os.h>

namespace Trinity
{
    namespace Network
    {
        typedef long double float_t;

        enum TrinityMessageType : uint16_t
        {
            SYNC,
            SYNC_WITH_RSP,
            ASYNC,
            PRESERVED_SYNC,
            PRESERVED_SYNC_WITH_RSP,
            PRESERVED_ASYNC,
        };

        enum UInt32_Contants :uint32_t
        {
            MaxConn = 512,
            RecvBufferSize = 8192,
            MessagePrefixLength = 4,

            TrinityMsgTypeOffset = 0,
            TrinityMsgIdOffset = 1,
        };

        namespace Float_Constants
        {
            static float_t AvgSlideWin_a = 0.85f;
            static float_t AvgSlideWin_b = 0.15f;
            static float_t AvgSlideWin_r = 2.15f;
        }

        extern uint8_t  HANDSHAKE_MESSAGE_CONTENT[];
        extern int   HANDSHAKE_MESSAGE_LENGTH;
    }
}