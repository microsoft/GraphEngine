// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <os/os.h>
#if !defined(TRINITY_PLATFORM_WINDOWS)
#include <fcntl.h>

namespace Trinity
{
    namespace Network
    {
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
    }
}

#endif

