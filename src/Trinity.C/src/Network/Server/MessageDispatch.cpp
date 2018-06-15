// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "os/os.h"
#include "Network/Server/TrinityServer.h"
#include "Network/Server/MessageDispatch.h"
#ifdef TRINITY_PLATFORM_WINDOWS
#include "Network/Server/iocp/TrinitySocketServer.h"
#else
#include "Network/Server/posix/TrinitySocketServer.h"
#endif

#include <stdio.h>
#include <string.h>

namespace Trinity {
	namespace Network {
		
		struct SyncReqResHandler_t {
			uint16_t Id;
			
		};
	};
};
