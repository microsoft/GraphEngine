// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#if defined(__linux__)
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
#include <sys/epoll.h>

#include <thread>
namespace Trinity
{
    namespace Network
    {
        void SocketAcceptThreadProc();
        void WorkerThreadProc(int tid);
        bool ProcessAccept(int epoll_fd, int sock_fd);
    }
}
#endif