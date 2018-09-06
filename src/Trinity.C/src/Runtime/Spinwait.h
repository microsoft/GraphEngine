// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <cstdint>
#include "os/os.h"

namespace Runtime
{
    inline void Spinwait(int32_t duration)
    {
        for (int delay = 0; delay < duration; ++delay)
        {
#if defined (TRINITY_PLATFORM_WINDOWS)
            __noop();
#else
            asm("");
#endif
        }
    }
}
