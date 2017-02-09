// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once

#if (defined(_WIN64) || defined(_WIN32))
/* Win64 */
#include "platforms/windows.h"
#endif

#if OVERRIDE_LINUX_DEF
#if !defined(__linux__)
#define __linux__
#endif
#if !defined(__unix__)
#define __unix__
#endif
#endif

#if defined(__linux__)
/* Linux */
#include "platforms/linux.h"
#endif

#if defined(__APPLE__) && defined(__MACH__)
/* Mac OS */

#include "platforms/darwin.h"
#endif

