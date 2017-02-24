// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#if _DEBUG
#if !defined(__linux__)
#define __linux__
#endif
#if !defined(__unix__)
#define __unix__
#endif
#endif
#if (defined(__linux__) || defined(__unix__))
#include "Network/Client/TrinityClient.h"
#endif
