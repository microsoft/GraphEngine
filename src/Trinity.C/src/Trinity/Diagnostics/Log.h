// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#define TRINITY_DEBUG
#include "Trinity/String.h"

namespace Trinity
{
    namespace Diagnostics
    {
        /**! Should be synchronized with Log.cs */
        enum LogLevel : int32_t
        {
            /// <summary>
            /// No message is logged
            /// </summary>
            Off     = 0,

            /// <summary>
            /// Only unrecoverable system errors are logged
            /// </summary>
            Fatal   = 1,

            /// <summary>
            /// Unrecoverable system errors and application logLevel errors are logged
            /// </summary>
            Error   = 2,

            /// <summary>
            /// Fatal system error, application error and application warning are logged
            /// </summary>
            Warning = 3,

            /// <summary>
            /// All errors, warnings and notable application messages are logged
            /// </summary>
            Info    = 4,

            /// <summary>
            /// All errors, warnings, application messages and debugging messages are logged
            /// </summary>
            Debug   = 5,

            /// <summary>
            /// All messages are logged
            /// </summary>
            Verbose = 6,
        };

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


#ifdef TRINITY_DEBUG
#define FatalLog(format, ...) Trinity::Diagnostics::WriteLine(Trinity::Diagnostics::LogLevel::Fatal, __FILE__, __LINE__, format, __VA_ARGS__)
#define ErrorLog(format, ...) Trinity::Diagnostics::WriteLine(Trinity::Diagnostics::LogLevel::Error, __FILE__, __LINE__, format, __VA_ARGS__)
#define WarningLog(format, ...) Trinity::Diagnostics::WriteLine(Trinity::Diagnostics::LogLevel::Warning, __FILE__, __LINE__, format, __VA_ARGS__)
#define InfoLog(format, ...) Trinity::Diagnostics::WriteLine(Trinity::Diagnostics::LogLevel::Info, __FILE__, __LINE__, format, __VA_ARGS__)
#define DebugLog(format, ...) Trinity::Diagnostics::WriteLine(Trinity::Diagnostics::LogLevel::Debug, __FILE__, __LINE__, format, __VA_ARGS__)
#define VerboseLog(format, ...) Trinity::Diagnostics::WriteLine(Trinity::Diagnostics::LogLevel::Verbose, __FILE__, __LINE__, format, __VA_ARGS__)
#else
#define Log(level, format, ...)
#endif
    }
}