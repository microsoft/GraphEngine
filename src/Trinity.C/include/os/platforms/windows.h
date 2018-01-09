// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
// Including SDKDDKVer.h defines the highest available Windows platform.
// If you wish to build your application for a previous Windows platform, include WinSDKVer.h and
// set the _WIN32_WINNT macro to the platform you wish to support before including SDKDDKVer.h.
#include <SDKDDKVer.h>

#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
#define NOMINMAX                        // Disable old min/max macros that interfere with <algorithm> std::min and std::max
// Windows Header Files:
#include <Windows.h>
//#include <winnt.h>
//#include <signal.h>
#include <tchar.h>
#include <io.h>
#include <Psapi.h>
#include <assert.h>
#define TRINITY_PLATFORM_WINDOWS
#define TRINITY_COMPILER_WARNING(msg) __pragma(message("Warning: "#msg))
#define ALIGNED(x) __declspec(align(x))
#define DLL_EXPORT extern "C" __declspec(dllexport)
// note: The C++11 keyword threa_local is not available on VS2013.
#define THREAD_LOCAL __declspec(thread)
