// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#if defined(__linux__)
#include <sys/mman.h>

#define TRINITY_PLATFORM_LINUX
#include "posix.h"
#endif
