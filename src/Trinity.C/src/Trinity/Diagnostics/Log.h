// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include "Trinity/String.h"
#include "Trinity/Log.h"

namespace Trinity
{
    namespace Diagnostics
    {
        void InitializeLogger();
        void PrintCallStack(void);
        void SetLogLevel(LogLevel level);
        void SetEchoOnConsole(bool is_set);
        void Flush();

        void _writeline_impl(LogLevel logLevel, const String& msg);
        void _writeline_impl(LogLevel logLevel, const char* srcFile, int32_t linenum, const String& msg);

        template<typename ...Args>
        inline void WriteLine(LogLevel logLevel, const String& format, Args... arguments)
        {
            _writeline_impl(logLevel, String::Format(format, arguments...));
        }

        template<typename ...Args>
        inline void WriteLine(LogLevel logLevel, const char* srcFile, int32_t lineNum, const char* format, Args... arguments)
        {
            _writeline_impl(logLevel, srcFile, lineNum, String::Format(format, arguments...));
        }

        template<typename ...Args>
        void FatalError(int32_t exit_code, const String& format, Args... arguments)
        {
            WriteLine(LogLevel::Fatal, format, arguments...);
#if defined(TRINITY_PLATFORM_WINDOWS)
            FatalExit(exit_code);
#else
            exit(exit_code);
#endif
        }

        template<typename ...Args>
        void FatalError(const String& format, Args... arguments)
        {
            FatalError(1, format, arguments...);
        }
    }
}