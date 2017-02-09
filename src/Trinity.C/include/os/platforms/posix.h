// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
//  os-specific definitions shared across POSIX platforms.

#pragma once

#include <climits>
#include <cstdio>
#include <cstdint>
#include <cmath>
#include <unistd.h>
#include <sys/types.h>
#include <errno.h>
#include <sys/stat.h>
#pragma once
#include <dirent.h>
#include <sys/types.h>
#include <pthread.h>

#include "arch/cpu.h"

#define YieldProcessor _mm_pause
#define CP_UTF8 65001
#define TRUE 1
#define FALSE 0
#define IN
#define OUT
#define REF
#define INOUT
#define VOID void

#define __stdcall __attribute__((stdcall)) //GCC does not recognize __stdcall, make it an macro.

typedef int            BOOL;
typedef int16_t        WORD;
typedef unsigned char  BYTE;
typedef char           CHAR;
typedef int32_t        DWORD;
typedef uint32_t       UINT;
typedef float          float_t;
typedef double         double_t;
typedef void*          LPVOID;
typedef size_t         SIZT_T;
int GetLastError();

#define _HELPER_2(x) message("Warning: "#x)
#define _HELPER_1(x) #x
#define TRINITY_COMPILER_WARNING(msg) _Pragma(_HELPER_1(_HELPER_2(msg)))
#define ALIGNED(x) __attribute__ ((aligned(x)))
#define DLL_EXPORT extern "C" __attribute__ ((visibility ("default")))