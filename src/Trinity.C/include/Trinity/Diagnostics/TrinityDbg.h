// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <stdio.h>
#include <errno.h>
#include <string.h>
#include "Trinity/IO/Console.h"

#define PRINTVAR(x) {Trinity::IO::Console::WriteLine("{0:20} = {1}", #x, x);}
#define PRINTSIZEOF(x) {Trinity::IO::Console::WriteLine("sizeof({0}) = {1}", #x, sizeof(x));}
#define PRINTISPOD(x) {Trinity::IO::Console::WriteLine("is_pod({0}) = {1}", #x, std::is_pod<x>::value);}

#if defined(TRINITY_PLATFORM_WINDOWS)
inline void DisplayError(TCHAR* pszAPI, DWORD dwError)
{
    LPVOID lpvMessageBuffer = NULL;

    if (FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER |
        FORMAT_MESSAGE_FROM_SYSTEM |
        FORMAT_MESSAGE_IGNORE_INSERTS,
        NULL, dwError,
        MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
        (LPTSTR)&lpvMessageBuffer, 0, NULL) != 0)
    {
        Trinity::IO::Console::WriteLine("ERROR: API        = {0}", pszAPI);
        Trinity::IO::Console::WriteLine("  error code = {0}", dwError);
        Trinity::IO::Console::WriteLine("  message    = {0}", lpvMessageBuffer);
        LocalFree(lpvMessageBuffer);
    }
    else
    {
        Trinity::IO::Console::WriteLine("ERROR: API        = {0}", pszAPI);
        Trinity::IO::Console::WriteLine("  error code = {0}", dwError);
        Trinity::IO::Console::WriteLine("  Cannot format the error message.");
    }

    ExitProcess(GetLastError());
}
#else
inline void DisplayError(char* pszAPI, DWORD dwError) { }
#endif