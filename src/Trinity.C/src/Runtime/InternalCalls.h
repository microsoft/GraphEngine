// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
typedef struct 
{
    char* Name;
    void* Address;
}ICallEntry;

#ifndef TRINITY_PLATFORM_WINDOWS
extern "C" void GetInternalCallEntries(ICallEntry** pEntries, size_t* pCount);
#endif