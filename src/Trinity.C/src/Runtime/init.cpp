// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "TrinityCommon.h"
#include "Memory/Memory.h"
#include "BackgroundThread/BackgroundThread.h"
#include "Trinity/Diagnostics/LogAutoFlushTask.h"

namespace Trinity
{
    namespace IO
    {
        namespace Path
        {
            Trinity::String g_AssemblyPath = "";
        }
    }
}

/* Should only be reached from CLR*/
//TODO if CLR not started, we should host our own
DLL_EXPORT VOID __stdcall __INIT_TRINITY_C__(u16char* pAssemblyPath)
{
#ifdef TRINITY_PLATFORM_WINDOWS
    Memory::LargePageMinimum = GetLargePageMinimum();
#endif
    Memory::GetWorkingSetSize();

    Trinity::IO::Path::g_AssemblyPath = Trinity::String::FromWcharArray(pAssemblyPath, -1);
}