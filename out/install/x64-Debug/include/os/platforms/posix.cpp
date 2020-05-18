// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include <os/os.h>

#if !defined(TRINITY_PLATFORM_WINDOWS)
int GetLastError(){return errno;}
#endif