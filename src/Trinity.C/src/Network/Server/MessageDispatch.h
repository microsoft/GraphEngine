// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <stdint.h>
#include <cstdlib>
#include <thread>
#include <algorithm>
#include <atomic>
#include "TrinityCommon.h"
#include "Network/Network.h"
#include "Network/ProtocolConstants.h"

namespace Trinity {
    namespace Network {
        namespace Messaging {
            struct SynReqArgs;
            struct SynReqRspArgs;
            struct AsynReqArgs;
            struct AsynReqRspArgs;

            typedef void(SynReqHandler)(SynReqArgs *);
            typedef void(SynReqRspHandler)(SynReqRspArgs *);
            typedef void(AsynReqHandler)(AsynReqArgs *);
            typedef void(AsynReqRspHandler)(AsynReqRspArgs *);

            struct SyncReqResHandler_t {
                uint16_t id;
                SynReqRspHandler * handler;
            };

            struct SyncReqHandler_t {
                uint16_t id;
                SynReqHandler * handler;
            };

            struct AsyncReqResHandler_t {
                uint16_t id;
                AsynReqRspHandler * handler;
            };

            class MessageHandlers {
            private:
                static MessageHandlers * _message_parser;

            public:
                MessageHandlers() {};

            };
        };
    };
};
